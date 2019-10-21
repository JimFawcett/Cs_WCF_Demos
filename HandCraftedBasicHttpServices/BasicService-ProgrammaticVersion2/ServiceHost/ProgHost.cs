/////////////////////////////////////////////////////////////////////////
// ProgHost.cs - Service Host for Programmatic BasicService demo       //
//                                                                     //
//   Uses BasicHttpBinding                                             //
//                                                                     //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2010     //
/////////////////////////////////////////////////////////////////////////
//
// - Started with C# Console Application Project
// - Made reference to .Net System.ServiceModel
// - Added using System.ServiceModel
// - Made reference to IService dll
// - Made reference to Service dll
// - Added code to create communication channel

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;


namespace HandCraftedService
{
  class Host
  {
    static ServiceHost CreateChannel(string url)
    {
      BasicHttpBinding binding = new BasicHttpBinding();
      Uri address = new Uri(url);
      Type service = typeof(BasicService);
      ServiceHost host = new ServiceHost(service, address);
      host.AddServiceEndpoint(typeof(IBasicService), binding, address);
      return host;
    }
    static void Main(string[] args)
    {
      Console.Title = "BasicHttp Service Host";
      Console.Write("\n  Starting Programmatic Basic Service");
      Console.Write("\n =====================================\n");

      ServiceHost host = null;
      try
      {
        host = CreateChannel("http://localhost:8080/BasicService");
        host.Open();
        Console.Write("\n  Started BasicService - Press key to exit:\n");
        Console.ReadKey();
      }
      catch (Exception ex)
      {
        Console.Write("\n\n  {0}\n\n", ex.Message);
        return;
      }
      host.Close();
    }
  }
}
