///////////////////////////////////////////////////////////////////////
// Client.cs - WCF SelfHosted File StreamService client              //
//                                                                   //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Summer 2008 //
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
  class Client
  {
    IStreamService channel;
    string ToSendPath = "..\\..\\ToSend";
    string SavePath = "..\\..\\SavedFiles";
    int BlockSize = 1024;
    byte[] block;

    Client()
    {
      block = new byte[BlockSize];
    }
    static IStreamService CreateServiceChannel(string url)
    {
      BasicHttpSecurityMode securityMode = BasicHttpSecurityMode.None;

      BasicHttpBinding binding = new BasicHttpBinding(securityMode);
      binding.TransferMode = TransferMode.Streamed;
      binding.MaxReceivedMessageSize = 500000000;
      EndpointAddress address = new EndpointAddress(url);

      ChannelFactory<IStreamService> factory
        = new ChannelFactory<IStreamService>(binding, address);
      return factory.CreateChannel();
    }

    void uploadFile(string filename)
    {
      Console.Write("\n  sending file \"{0}\"",filename);
      string fqname = Path.Combine(ToSendPath, filename);
      using (var inputStream = new FileStream(fqname, FileMode.Open))
      {
        FileTransferMessage msg = new FileTransferMessage();
        msg.filename = filename;
        msg.transferStream = inputStream;
        channel.upLoadFile(msg);
      }
    }

    void download(string filename)
    {
      try
      {
        Stream strm = channel.downLoadFile(filename);
        string rfilename = Path.Combine(SavePath, filename);
        if (!Directory.Exists(SavePath))
          Directory.CreateDirectory(SavePath);
        using (var outputStream = new FileStream(rfilename, FileMode.Create))
        {
          while (true)
          {
            int bytesRead = strm.Read(block, 0, BlockSize);
            if (bytesRead > 0)
              outputStream.Write(block, 0, bytesRead);
            else
              break;
          }
        }
        Console.Write("\n  Received file \"{0}\"", filename);
      }
      catch (Exception ex)
      {
        Console.Write("\n  {0}\n",ex.Message);
      }
    }

    static void Main()
    {
      Console.Write("\n  Client of SelfHosted File Stream Service");
      Console.Write("\n ==========================================\n");

      Client clnt = new Client();
      clnt.channel = CreateServiceChannel("http://localhost:8000/StreamService");

      clnt.uploadFile("test.txt");
      clnt.uploadFile("Channel_Client_Copy.exe");
      clnt.uploadFile("FileStreaming.zip");

      Console.WriteLine();

      clnt.download("test.txt");
      clnt.download("FileStreaming.zip");
      clnt.download("foobar");

      Console.Write("\n\n  Press key to terminate client");
      Console.ReadKey();
      Console.Write("\n\n");
      ((IChannel)clnt.channel).Close();
    }
  }
}
