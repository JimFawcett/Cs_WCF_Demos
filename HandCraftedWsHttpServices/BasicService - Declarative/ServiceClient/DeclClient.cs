/////////////////////////////////////////////////////////////////////////
// DeclClient.cs - Service Client for Declarative BasicService demo    //
//                                                                     //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2010     //
/////////////////////////////////////////////////////////////////////////
//
// - Started with C# Console Application Project
// - Made reference to .Net System.ServiceModel
// - Added using System.ServiceModel
// - Made reference to IService dll
// - Project->Add New Item->Application Configuration File
// - Added config settings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HandCraftedService;
using System.Threading;

namespace ServiceClient
{
  // This class is similar to wizard generated proxy, but 
  // has eliminated a lot of unused code

  public class proxy : 
    System.ServiceModel.ClientBase<HandCraftedService.IBasicService>, 
    HandCraftedService.IBasicService
  {
    // uses address, binding, and contract from App.Config
    public proxy() { }

    // uses binding and contract from named endpoint in App.Config
    // uses address specified in client code
    public proxy(string endpointName, EndpointAddress address)
      : base(endpointName, address) { }

    public void sendMessage(string msg) { base.Channel.sendMessage(msg); }

    public string getMessage() { return base.Channel.getMessage(); }
  }
  class DeclClient
  {
    static void Main(string[] args)
    {
      Console.Write("\n  Starting Declarative BasicService Client");
      Console.Write("\n ==========================================\n");

      Console.Write("\n  using default proxy");
      Console.Write("\n ---------------------");

      proxy defaultProxy = new proxy();
      
      string msg = "This is a test message from default client";
      defaultProxy.sendMessage(msg);

      msg = defaultProxy.getMessage();
      Console.Write("\n  Message received from Service: {0}\n", msg);

      Console.Write("\n  using proxy configured with programmatic address");
      Console.Write("\n --------------------------------------------------");

      string url = "http://localhost:8080/BasicService";
      System.ServiceModel.EndpointAddress address = new EndpointAddress(url);

      proxy configuredProxy = new proxy("wsHttpBinding_IBasicService", address);
      msg = "This is a test message from configured client";
      configuredProxy.sendMessage(msg);

      msg = configuredProxy.getMessage();
      Console.Write("\n  Message received from Service: {0}\n\n", msg);
    }
  }
}
