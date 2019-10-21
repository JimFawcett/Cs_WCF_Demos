/////////////////////////////////////////////////////////////////////////
// ProgClient.cs - Service Client for Programmatic BasicService demo   //
//   version 2 - added ServiceRetryWrapper                             //
//   Uses BasicHttpBinding                                             //
//                                                                     //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2010     //
/////////////////////////////////////////////////////////////////////////
//
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
    IBasicService svc;

    ProgClient(string url)
    {
      svc = CreateProxy<IBasicService>(url);
    }
    //----< returns a proxy of type T >----------------------------------
    /*
     * Proxy is a local object that transparently uses
     * communication channel to converse with server
     * using contract C.
     * 
     * Channel doesn't attempt to connect to server
     * until first service call.
     */
    static C CreateProxy<C>(string url)
    {
      BasicHttpBinding binding = new BasicHttpBinding();
      EndpointAddress address = new EndpointAddress(url);
      ChannelFactory<C> factory = new ChannelFactory<C>(binding, address);
      return factory.CreateChannel();
    }
    //----< Wrapper attempts to call service method several times >------
    /*
     *  Func<string> is a delegate that invokes functions
     *  which take no arguments and return strings
     */
    string ServiceRetryWrapper(Func<string> fnc)
    {
      int count = 0;
      string msg;
      while(true)
      {
        try
        {
          msg = fnc.Invoke();
          break;
        }
        catch(Exception exc)
        {
          if (count > 4)
          {
            return "Max retries exceeded";
          }
          Console.Write("\n  {0}", exc.Message);
          Console.Write("\n  service failed {0} times - trying again", ++count);
          Thread.Sleep(100);
          continue;
        }
      }
      return msg;
    }

    void SendMessage(string msg)
    {
      // input string is captured in body of functor
      // Func<string> return string is discarded

      Func<string> fnc = () => { svc.sendMessage(msg); return "service succeeded"; };
      ServiceRetryWrapper(fnc);
    }

    string GetMessage()
    {
      string msg;
      Func<string> fnc = () => { msg = svc.getMessage(); return msg; };
      return ServiceRetryWrapper(fnc);
    }
    static void Main(string[] args)
    {
      Console.Title = "BasicHttp Client";
      Console.Write("\n  Starting Programmatic Basic Service Client");
      Console.Write("\n ============================================\n");

      string url = "http://localhost:8080/BasicService";
      ProgClient client = new ProgClient(url);

      string msg = "This is a test message from client";
      client.SendMessage(msg);

      msg = client.GetMessage();
      Console.Write("\n  Message recieved from Service: {0}\n\n", msg);
    }
  }
}
