/////////////////////////////////////////////////////////////////////////
// ProgClient.cs - Service Client for Programmatic BasicService demo   //
//                                                                     //
//   Uses WSHttpBinding                                                //
//                                                                     //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2010     //
/////////////////////////////////////////////////////////////////////////
//
// - Started with C# Console Application Project
// - Made reference to .Net System.ServiceModel
// - Added using System.ServiceModel
// - Made reference to IService dll
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
      WSHttpBinding binding = new WSHttpBinding();
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
      int count = 0;
      while (true)
      {
        try
        {
          svc = CreateProxy(url);
          break;
        }
        catch
        {
          Console.Write("\n  connection to service failed {0} times - trying again", ++count);
          Thread.Sleep(100);
          continue;
        }
      }
      string msg = "This is a test message from client";
      svc.sendMessage(msg);

      msg = svc.getMessage();
      Console.Write("\n  Message recieved from Service: {0}\n\n", msg);
    }
  }
}
