///////////////////////////////////////////////////////////////////
// Strings.cs - WCF Strings Service in Self Hosted Configuration //
//                                                               //
// Jim Fawcett, CSE775 - Distributed Objects, Spring 2009        //
///////////////////////////////////////////////////////////////////
/*
 * Note:
 *   Uses Programmatic configuration, no app.config file used.
 *   Uses ChannelFactory to create proxy programmatically.   
 */

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CSE775
{
  [ServiceBehavior]
  public class Strings : IStrings
  {
    string str_;

    public void putString(string str)
    {
      Console.Write("\n  received, in putString, \"{0}\"", str);
      str_ = str;
    }

    public void putRefString(ref string str)
    {
      Console.Write("\n  received, in putRefString, \"{0}\"", str);
      str = "ref string";
      Console.Write("\n  modified string to \"{0}\"", str);
    }

    public string getString()
    {
      Console.Write("\n  sending, from getString, \"{0}\"",str_);
      return str_;
    }

    public void getOutString(out string str)
    {
      str = "out string";
      Console.Write("\n  sending, from getOutString, \"{0}\"",str);
    }

    static ServiceHost CreateServiceChannel(string url)
    {
      // SecurityMode options are:
      // - None
      // - Message (default)
      // - Transport
      // - TransportWithMessageCredential

      SecurityMode securityMode = SecurityMode.Message;

      // ReliableSession is a property provided by bindings that support
      // transport-level session.  If set to true, the service will 
      // terminate after 10 minutes of inactivity.

      bool reliableSession = false;

      WSHttpBinding binding = new WSHttpBinding(securityMode, reliableSession);
      Uri baseAddress = new Uri(url);
      Type service = typeof(CSE775.Strings);
      ServiceHost host = new ServiceHost(service, baseAddress);
      host.AddServiceEndpoint(typeof(IStrings), binding, baseAddress);
      return host;
    }

    public static void Main()
    {
      ServiceHost host = CreateServiceChannel("http://localhost:8080/Strings");

      host.Open();

      Console.Write("\n  SelfHosted Strings Service started");
      Console.Write("\n ====================================\n");
      Console.Write("\n  Press key to terminate service:\n");
      Console.ReadKey();
      Console.Write("\n");
      host.Close();
    }
  }
}
