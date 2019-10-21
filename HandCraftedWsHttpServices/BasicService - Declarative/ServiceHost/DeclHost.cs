/////////////////////////////////////////////////////////////////////////
// DeclHost.cs - Service Host for Declarative BasicService demo        //
//                                                                     //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2010     //
/////////////////////////////////////////////////////////////////////////
//
// - Started with C# Console Application Project
// - Made reference to .Net System.ServiceModel
// - Added using System.ServiceModel
// - Made reference to IService dll
// - Made reference to Service dll
// - Project->Add New Item->Application Configuration File
// - Added config settings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace HandCraftedService
{
  class Host
  {
    static void Main(string[] args)
    {
      Console.Write("\n  Starting Declarative BasicService");
      Console.Write("\n ===================================\n");

      try
      {
        ServiceHost host;
        using (host = new ServiceHost(typeof(BasicService)))
        {
          host.Open();
          Console.Write("\n  Started BasicService - Press key to exit:\n");
          Console.ReadKey();
          host.Close();
        }
      }
      catch (Exception ex)
      {
        Console.Write("\n\n  {0}\n\n", ex.Message);
      }
    }
  }
}
