///////////////////////////////////////////////////////////////////////
// Client.cs - client of self-hosted file transfer service           //
//                                                                   //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2010   //
///////////////////////////////////////////////////////////////////////
/*
 * You need to run both FileService and Client with administrator 
 * priviledges.  You do that by running Visual Studio as administrator
 * or right-clicking on the FileService.exe and Client.exe and selecting
 * run as administrator.
 * This service is configured with WSHttpBinding.
 * - That has the advantage that messages, when sent across the network
 *   by default arrive in the order sent.
 * - It has the disadvantage that machines that use or host the service
 *   need to have digital certificates installed.
 * - Visual Studio 2010 installs the requireded certificate,
 */
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace FileTransferService
{
  class Client
  {
    static IFileService CreateChannel(string url)
    {
      WSHttpBinding binding = new WSHttpBinding();
      EndpointAddress address = new EndpointAddress(url);
      ChannelFactory<IFileService> factory = new ChannelFactory<IFileService>(binding, address);
      return factory.CreateChannel();
    }
    static bool SendFile(IFileService service, string file)
    {
      long blockSize = 32;
      try
      {
        string filename = Path.GetFileName(file);
        service.OpenFileForWrite(filename);
        FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read);
        int bytesRead = 0;
        while (true)
        {
          long remainder = (int)(fs.Length - fs.Position);
          if (remainder == 0)
            break;
          long size = Math.Min(blockSize, remainder);
          byte[] block = new byte[size];
          bytesRead = fs.Read(block, 0, block.Length);
          service.WriteFileBlock(block);
        }
        fs.Close();
        service.CloseFile();
        return true;
      }
      catch(Exception ex)
      {
        Console.Write("\n  can't open {0} for writing - {1}", file, ex.Message);
        return false;
      }
    }
    static void Main(string[] args)
    {
      string url = "http://localhost:8080/FileService";

      Console.Write("\n  Client of File Transfer Service");
      Console.Write("\n =================================\n");

      IFileService fs = null;
      int count = 0;
      while (true)
      {
        try
        {
          fs = CreateChannel(url);
          break;
        }
        catch 
        {
          Console.Write("\n  connection to service failed {0} times - trying again", ++count);
          Thread.Sleep(500);
          continue; 
        }
      }
      Console.Write("\n  Connected to {0}\n", url);
      string relativeFilePath = "FilesToSend";
      if(args.Length > 0)
        relativeFilePath = args[0];

      string filepath = Path.GetFullPath(relativeFilePath);
      Console.Write("\n  retrieving files from\n  {0}\n", filepath);
      string[] files = Directory.GetFiles(filepath);
      foreach (string file in files)
      {
        string filename = Path.GetFileName(file);
        Console.Write("\n  sending file {0}", filename);
        
        if (!SendFile(fs, file))
          Console.Write("\n  could not send file");
      }
      Console.Write("\n\n");
    }
  }
}
