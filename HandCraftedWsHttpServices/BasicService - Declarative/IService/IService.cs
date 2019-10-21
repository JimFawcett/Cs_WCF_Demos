/////////////////////////////////////////////////////////////////////////
// IService.cs - Service interface for BasicService demo               //
//                                                                     //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2010     //
/////////////////////////////////////////////////////////////////////////
//
// - Started with C# Class Library Project
// - Made reference to .Net System.ServiceModel
// - Added using System.ServiceModel

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace HandCraftedService
{
  [ServiceContract(Namespace="HandCraftedService")]
  public interface IBasicService
  {
    [OperationContract]
    void sendMessage(string msg);

    [OperationContract]
    string getMessage();
  }
}
