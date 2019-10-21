///////////////////////////////////////////////////////////////////////
// StreamService.cs - WCF StreamService in Self Hosted Configuration //
//                                                                   //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Summer 2009 //
///////////////////////////////////////////////////////////////////////
/*
 * Note:
 * - Uses Programmatic configuration, no app.config file used.
 * - Uses ChannelFactory to create proxy programmatically. 
 * - Expects to find ToSend directory under application with files
 *   to send.
 * - Will create SavedFiles directory if it does not already exist.
 */

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CSE681
{
  [ServiceBehavior(IncludeExceptionDetailInFaults=true)]
  public class StreamService : IStreamService
  {
    string filename;
    string savePath = "..\\SavedFiles";
    string ToSendPath = "..\\ToSend";
    int BlockSize = 1024;
    byte[] block;

    StreamService()
    {
      block = new byte[BlockSize];
    }

    public void upLoadFile(FileTransferMessage msg)
    {
      HRTimer.HiResTimer hrt = new HRTimer.HiResTimer();
      hrt.Start();
      filename = msg.filename;
      string rfilename = Path.Combine(savePath, filename);
      if (!Directory.Exists(savePath))
        Directory.CreateDirectory(savePath);
      using (var outputStream = new FileStream(rfilename, FileMode.Create))
      {
        while (true)
        {
          int bytesRead = msg.transferStream.Read(block, 0, BlockSize);
          if (bytesRead > 0)
            outputStream.Write(block, 0, bytesRead);
          else
            break;
        }
      }
      hrt.Stop();
      Console.Write("\n  Received file \"{0}\"", filename);
    }

    public Stream downLoadFile(string filename)
    {
      string sfilename = Path.Combine(ToSendPath, filename);
      FileStream outStream = null;
      if (File.Exists(sfilename))
      {
        outStream = new FileStream(sfilename, FileMode.Open);
        Console.Write("\n  Sending File \"{0}\"", filename);
      }
      else
        throw new Exception("open failed for \"" + filename + "\"");
      return outStream;
    }

    static ServiceHost CreateServiceChannel(string url)
    {
      // Can't configure SecurityMode other than none with streaming.
      // This is the default for BasicHttpBinding.
      //   BasicHttpSecurityMode securityMode = BasicHttpSecurityMode.None;
      //   BasicHttpBinding binding = new BasicHttpBinding(securityMode);

      BasicHttpBinding binding = new BasicHttpBinding();
      binding.TransferMode = TransferMode.Streamed;
      binding.MaxReceivedMessageSize = 50000000;
      Uri baseAddress = new Uri(url);
      Type service = typeof(CSE681.StreamService);
      ServiceHost host = new ServiceHost(service, baseAddress);
      host.AddServiceEndpoint(typeof(IStreamService), binding, baseAddress);
      return host;
    }

    public static void Main()
    {
      ServiceHost host = CreateServiceChannel("http://localhost:8000/StreamService");

      host.Open();

      Console.Write("\n  SelfHosted File Stream Service started");
      Console.Write("\n ========================================\n");
      Console.Write("\n  Press key to terminate service:\n");
      Console.ReadKey();
      Console.Write("\n");
      host.Close();
    }
  }
}
