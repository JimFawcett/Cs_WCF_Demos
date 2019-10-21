/////////////////////////////////////////////////////////////////////////
// Service.cs - Service for BasicService demo                          //
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
  [ServiceBehavior]
  public class BasicService : IBasicService
  {
    public void sendMessage(string msg)
    {
      Console.Write("\n  Service received message {0}", msg);
    }

    public string getMessage()
    {
      return "a new message from Service";
    }
  }
}
