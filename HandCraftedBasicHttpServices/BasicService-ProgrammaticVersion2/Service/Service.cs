/////////////////////////////////////////////////////////////////////////
// Service.cs - Programmatic BasicService demo                         //
//                                                                     //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2010     //
/////////////////////////////////////////////////////////////////////////
//
// - Started with C# Class Library Project
// - Made reference to .Net System.ServiceModel
// - Added using System.ServiceModel
// - Made reference to IService dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace HandCraftedService
{
  /*
   * InstanceContextMode determines the activation policy, e.g.:
   * 
   *   PerCall    - remote object created for each call
   *              - runs on thread dedicated to calling client
   *              - this is default activation policy
   *   PerSession - remote object created in session on first call
   *              - session times out unless called again within timeout period
   *              - runs on thread dedicated to calling client
   *   Singleton  - remote object created in session on first call
   *              - session times out unless called again within timeout period
   *              - runs on one thread so all clients see same instance
   *              - access must be synchronized
   */
  [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerCall)]
  public class BasicService : IBasicService
  {
    public void sendMessage(string msg)
    {
      Console.Write("\n  Service received message {0}", msg);
    }

    public string getMessage()
    {
      return "new message from Service";
    }
  }
}
