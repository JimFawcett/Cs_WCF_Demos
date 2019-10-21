/////////////////////////////////////////////////////////////////////////
// CommService.svc.cs - Implementation of ICommService contract        //
//                                                                     //
// Jim Fawcett, CSE775 - Distributed Objects, Spring 2009              //
/////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using SWTools;
using System.Runtime.InteropServices;

namespace CommunicationPrototype
{
  // PerSession activation creates an instance of the service for each
  // client.  That instance lives for a pre-determined lease time.  
  // - If the creating client calls back within the lease time, then
  //   the lease is renewed and the object stays alive.  Otherwise it
  //   is invalidated for garbage collection.
  // - This behavior is a reasonable compromise between the resources
  //   spent to create new objects and the memory allocated to persistant
  //   objects.
  // 

  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]

  public class CommService : ICommService
  {
    [DllImport("USER32.DLL", SetLastError = true) ]
    public static extern void SetWindowPos(
      IntPtr hwnd, IntPtr order, 
      int xpos, int ypos, int width, int height, 
      uint flags
    );
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow();

    // We want the queue to be shared by all clients and the server,
    // so make it static.

    static BlockingQueue<Message> BlockingQ = null;
    ServiceHost host = null;

    CommService()
    {
      // Only one service, the first, should create the queue

      if(BlockingQ == null)
        BlockingQ = new BlockingQueue<Message>();

      SetWindowPos(GetConsoleWindow(), (IntPtr)0, 100, 100, 400, 600, 0);
      Console.Title = "CommService";
    }

    public void PostMessage(Message msg)
    {
      //IdentifyClient();
      BlockingQ.enQ(msg);
    }

    public void IdentifyClient()
    {
      OperationContext context = OperationContext.Current;
      MessageProperties messageProperties = context.IncomingMessageProperties;
      RemoteEndpointMessageProperty endpointProperty =
          messageProperties[RemoteEndpointMessageProperty.Name]
          as RemoteEndpointMessageProperty;

      Console.Write(
        "\n  IP address is {0} and port is {1}", 
        endpointProperty.Address, 
        endpointProperty.Port
      );
    }

    // Since this is not a service operation only server can call

    public Message GetMessage()
    {
      return BlockingQ.deQ();
    }

    // Method for server's child thread to run to process messages.
    // It's virtual so you can derive from this service and define
    // some other server functionality.

    protected virtual void ThreadProc()
    {
       while (true)
       {
         Message msg = this.GetMessage();
         string cmdStr = "";
         switch (msg.command)
         {
           case Message.Command.DoThis:
             cmdStr = "DoThis";
             break;
           case Message.Command.DoThat:
             cmdStr = "DoThat";
             break;
           case Message.Command.DoAnother:
             cmdStr = "DoAnother";
             break;
           default:
             cmdStr = "unknown command";
             break;
         }
         Console.Write("\n  received: {0}\t{1}",cmdStr,msg.text);
         if (msg.text == "quit")
           break;
       } 
    }

    public static void Main()
    {
      Console.Write("\n  Communication Server Starting up");
      Console.Write("\n ==================================\n");

      try
      {
        CommService service = new CommService();

        // - We're using WSHttpBinding and NetTcpBinding so digital certificates
        //   are required.
        // - Both these bindings support ordered delivery of messages by default.

        BasicHttpBinding binding0 = new BasicHttpBinding();
        WSHttpBinding binding1 = new WSHttpBinding();
        NetTcpBinding binding2 = new NetTcpBinding();
        Uri address0 = new Uri("http://localhost:4030/ICommService/BasicHttp");
        Uri address1 = new Uri("http://localhost:4040/ICommService/WSHttp");
        Uri address2 = new Uri("net.tcp://localhost:4050/ICommService/NetTcp");

        using (service.host = new ServiceHost(typeof(CommService), address1))
        {
          service.host.AddServiceEndpoint(typeof(ICommService), binding0, address0);
          service.host.AddServiceEndpoint(typeof(ICommService), binding1, address1);
          service.host.AddServiceEndpoint(typeof(ICommService), binding2, address2);
          service.host.Open();

          Console.Write("\n  CommService is ready.");
          Console.Write("\n    Maximum BasicHttp message size = {0}", binding0.MaxReceivedMessageSize);
          Console.Write("\n    Maximum WSHttp message size    = {0}", binding1.MaxReceivedMessageSize);
          Console.Write("\n    Maximum NetTcp message size    = {0}", binding2.MaxReceivedMessageSize);
          Console.WriteLine();

          Thread child = new Thread(new ThreadStart(service.ThreadProc));
          child.Start();
          child.Join();

          Console.Write("\n\n  Press <ENTER> to terminate service.\n\n");
          Console.ReadLine();
        }
      }
      catch (Exception ex)
      {
        Console.Write("\n  {0}\n\n", ex.Message);
      }
    }
  }
}
