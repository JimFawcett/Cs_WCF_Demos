///////////////////////////////////////////////////////////////
// Client.cs - WCF SelfHosted Strings Service client         //
//                                                           //
// Jim Fawcett, CSE775 - Distributed Objects, Spring 2008    //
///////////////////////////////////////////////////////////////
/*
 * Note:
 *   Uses Programmatic configuration, no app.config file used.
 *   Uses ChannelFactory to create proxy programmatically.   
 */

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace CSE775
{
  class Client
  {
    static IStrings CreateServiceChannel(string url)
    {
      // See notes in Strings.cs concerning securityMode and
      // reliableSession parameters

      SecurityMode securityMode = SecurityMode.Message;
      bool reliableSession = false;

      WSHttpBinding binding = new WSHttpBinding(securityMode, reliableSession);
      EndpointAddress address = new EndpointAddress(url);

      ChannelFactory<IStrings> factory
        = new ChannelFactory<IStrings>(binding, address);
      return factory.CreateChannel();
    }
    static void Main()
    {
      Console.Write("\n  Client of SelfHosted Strings service");
      Console.Write("\n ======================================\n");

      Thread.Sleep(2500);  // give server time to start

      IStrings channel = CreateServiceChannel("http://localhost:8080/Strings");

      string str = "a not very important message";
      Console.Write("\n  sending:  \"{0}\"", str);
      channel.putString(str);
      Console.Write("\n  received: \"{0}\"", channel.getString());

      str = "a modifiable string";
      Console.Write("\n  sending \"{0}\"", str);
      channel.putRefString(ref str);
      Console.Write("\n  string modified to: \"{0}\"", str);
      
      channel.getOutString(out str);
      Console.Write("\n  received out parameter: \"{0}\"", str);
      
      Console.Write("\n\n  Press key to terminate client");
      Console.ReadKey();
      Console.Write("\n\n");
      ((IChannel)channel).Close();
    }
  }
}
