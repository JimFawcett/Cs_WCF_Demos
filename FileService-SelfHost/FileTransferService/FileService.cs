///////////////////////////////////////////////////////////////////////
// FileService.cs - Self-hosted file transfer service                //
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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using System.IO;

namespace FileTransferService
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
  class FileService : IFileService
  {
    string filePath = ".\\SentFiles";
    string fileSpec = "";
    FileStream fs = null;  // remove static for WSHttpBinding

    public void SetServerFilePath(string path)
    {
      filePath = path;
    }
    public bool OpenFileForWrite(string name)
    {
      if (!Directory.Exists(filePath))
        Directory.CreateDirectory(filePath);

      fileSpec = filePath + "\\" + name;
      try
      {
        fs = File.Open(fileSpec, FileMode.Create, FileAccess.Write);
        Console.Write("\n  {0} opened", fileSpec);
        return true;
      }
      catch 
      {
        Console.Write("\n  {0} filed to open", fileSpec);
        return false; 
      }
    }
    public bool WriteFileBlock(byte[] block)
    {
      try
      {
        Console.Write("\n  writing block with {0} bytes", block.Length);
        fs.Write(block, 0, block.Length);
        fs.Flush();
        return true;
      }
      catch { return false; }
    }
    public bool CloseFile()
    {
      try
      {
        fs.Close();
        Console.Write("\n  {0} closed", fileSpec);
        return true;
      }
      catch { return false; }
    }
    static ServiceHost CreateChannel(string url)
    {
      WSHttpBinding binding = new WSHttpBinding();
      Uri baseAddress = new Uri(url);
      Type service = typeof(FileTransferService.FileService);
      ServiceHost host = new ServiceHost(service, baseAddress);
      host.AddServiceEndpoint(typeof(FileTransferService.IFileService), binding, baseAddress);
      return host;
    }
    static void Main(string[] args)
    {
      Console.Write("\n  File Transfer Service running:");
      Console.Write("\n ================================\n");

      ServiceHost host = CreateChannel("http://localhost:8080/FileService");
      host.Open();
      Console.Write("\n  Press key to terminate service:\n");
      Console.ReadKey();
      Console.Write("\n");
      host.Close();
    }
  }
}
