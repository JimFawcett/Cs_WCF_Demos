/////////////////////////////////////////////////////////////////////////
// ProgClient.cs - Service Client for Programmatic BasicService demo   //
//                                                                     //
//   Uses BasicHttpBinding                                             //
//                                                                     //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2010     //
/////////////////////////////////////////////////////////////////////////
// - Started with C# Console Application Project
// - Made reference to .Net System.ServiceModel
// - Added using System.ServiceModel
// - Added code to create communication channel

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HandCraftedService;
using System.Threading;

namespace ServiceClient
{
  class ProgClient
  {
    static IBasicService CreateProxy(string url)
    {
      BasicHttpBinding binding = new BasicHttpBinding();
      EndpointAddress address = new EndpointAddress(url);
      ChannelFactory<IBasicService> factory = new ChannelFactory<IBasicService>(binding, address);
      return factory.CreateChannel();
    }
    static void Main(string[] args)
    {
      Console.Write("\n  Starting Programmatic Basic Service Client");
      Console.Write("\n ============================================\n");

      string url = "http://localhost:8080/BasicService";
      IBasicService svc = null;
      svc = CreateProxy(url);
      int count = 0;
      while (true)
      {
        try
        {
          for(int i=0; i<5; ++i)
          {
            string cmsg = String.Format("message #{0} from client", i + 1);
            svc.sendMessage(cmsg);
          }
          svc.sendMessage("quit");
          break;
        }
        catch
        {
          Console.Write("\n  connection to service failed {0} times - trying again", ++count);
          Thread.Sleep(100);
          if(count >= 10)
          {
            Console.Write("\n  could not reach server in {0} attempts -- quitting\n\n", count);
            return;
          }
          continue;
        }
      }
      string msg = "";
      do
      {
        msg = svc.getMessage();
        Console.Write("\n  Message recieved from Service: {0}", msg);
      } while (msg != "quit");
      Console.Write("\n\n");
    }
  }
}
