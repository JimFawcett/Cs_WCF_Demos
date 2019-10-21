/////////////////////////////////////////////////////////////////////
// ICommunicator.cs - Peer-To-Peer Communicator Service Contract   //
// ver 2.0                                                         //
// Jim Fawcett, CSE681 - Software Modeling & Analysis, Summer 2011 //
/////////////////////////////////////////////////////////////////////
/*
 * Maintenance History:
 * ====================
 * ver 2.0 : 10 Oct 11
 * - removed [OperationContract] from GetMessage() so only local client
 *   can dequeue messages
 * ver 1.0 : 14 Jul 07
 * - first release
 */

using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WCF_Peer_Comm
{
  [ServiceContract]
  public interface ICommunicator
  {
    [OperationContract(IsOneWay = true)]
    void PostMessage(string msg);

    // used only locally so not exposed as service method

    string GetMessage();
  }
}
