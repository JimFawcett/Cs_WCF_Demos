/////////////////////////////////////////////////////////////////////
// Communication.svc.cs - Peer-To-Peer WCF Communicator            //
// ver 2.1                                                         //
// Jim Fawcett, CSE681 - Software Modeling & Analysis, Summer 2011 //
/////////////////////////////////////////////////////////////////////
/*
 * Maintenance History:
 * ====================
 * ver 2.2 : 01 Nov 11
 * - Removed unintended local declaration of ServiceHost in Receiver's 
 *   CreateReceiveChannel function
 * ver 2.1 : 10 Oct 11
 * - removed [OperationContract] from GetMessage() so only local client
 *   can dequeue messages
 * - added send thread to keep clients from blocking on slow sends
 * - added retries when creating Communication channel proxy
 * - added comments to clarify what code is doing
 * ver 2.0 : 06 Nov 08
 * - added close functions that close the service and receive channel
 * ver 1.0 : 14 Jul 07
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SWTools;

namespace WCF_Peer_Comm
{
  /////////////////////////////////////////////////////////////
  // Receiver hosts Communication service used by other Peers

  public class Receiver : ICommunicator
  {
    static BlockingQueue<string> rcvBlockingQ = null;
    ServiceHost service = null;

    public Receiver()
    {
      if (rcvBlockingQ == null)
        rcvBlockingQ = new BlockingQueue<string>();
    }

    public void Close()
    {
      service.Close();
    }

    //  Create ServiceHost for Communication service

    public void CreateRecvChannel(string address)
    {
      BasicHttpBinding binding = new BasicHttpBinding();
      Uri baseAddress = new Uri(address);
      service = new ServiceHost(typeof(Receiver), baseAddress);
      service.AddServiceEndpoint(typeof(ICommunicator), binding, baseAddress);
      service.Open();
    }

    // Implement service method to receive messages from other Peers

    public void PostMessage(string msg)
    {
      rcvBlockingQ.enQ(msg);
    }

    // Implement service method to extract messages from other Peers.
    // This will often block on empty queue, so user should provide
    // read thread.

    public string GetMessage()
    {
      return rcvBlockingQ.deQ();
    }
  }
  ///////////////////////////////////////////////////
  // client of another Peer's Communication service

  public class Sender
  {
    ICommunicator channel;
    string lastError = "";
    BlockingQueue<string> sndBlockingQ = null;
    Thread sndThrd = null;
    int tryCount = 0, MaxCount = 10;

    // Processing for sndThrd to pull msgs out of sndBlockingQ
    // and post them to another Peer's Communication service

    void ThreadProc()
    {
      while (true)
      {
        string msg = sndBlockingQ.deQ();
        channel.PostMessage(msg);
        if (msg == "quit")
          break;
      }
    }

    // Create Communication channel proxy, sndBlockingQ, and
    // start sndThrd to send messages that client enqueues

    public Sender(string url)
    {
      sndBlockingQ = new BlockingQueue<string>();
      while (true)
      {
        try
        {
          CreateSendChannel(url);
          tryCount = 0;
          break;
        }
        catch (Exception ex)
        {
          if (++tryCount < MaxCount)
            Thread.Sleep(100);
          else
          {
            lastError = ex.Message;
            break;
          }
        }
      }
      sndThrd = new Thread(ThreadProc);
      sndThrd.IsBackground = true;
      sndThrd.Start();
    }

    // Create proxy to another Peer's Communicator

    public void CreateSendChannel(string address)
    {
      EndpointAddress baseAddress = new EndpointAddress(address);
      BasicHttpBinding binding = new BasicHttpBinding();
      ChannelFactory<ICommunicator> factory
        = new ChannelFactory<ICommunicator>(binding, address);
      channel = factory.CreateChannel();
    }

    // Sender posts message to another Peer's queue using
    // Communication service hosted by receipient via sndThrd

    public void PostMessage(string msg)
    {
      sndBlockingQ.enQ(msg);
    }

    public string GetLastError()
    {
      string temp = lastError;
      lastError = "";
      return temp;
    }

    public void Close()
    {
      ChannelFactory<ICommunicator> temp = (ChannelFactory<ICommunicator>)channel;
      temp.Close();
    }
    static void Main(string[] args)
    {
      Console.Write("\n  Comm Service Running:");
      Console.Write("\n  Press key to quit");
      Console.ReadKey();
      Console.Write("\n\n");
    }
  }
}
