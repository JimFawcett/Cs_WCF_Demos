/////////////////////////////////////////////////////////////////////////
// ICommService.cs - ICommService contract                             //
//                                                                     //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Summer 2008   //
/////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CommunicationPrototype
{
  [ServiceContract]
  public interface ICommService
  {

    [OperationContract(IsOneWay=true)]
    void PostMessage(Message msg);

    // Not a service operation so only server can call

    Message GetMessage();
  }

  [DataContract]
  public class Message
  {
    [DataMember]
    Command cmd = Command.DoThis;
    [DataMember]
    string body = "default message text";

    public enum Command
    {
      [EnumMember]
      DoThis,
      [EnumMember]
      DoThat,
      [EnumMember]
      DoAnother
    }

    [DataMember]
    public Command command
    {
      get { return cmd; }
      set { cmd = value; }
    }

    [DataMember]
    public string text
    {
      get { return body; }
      set { body = value; }
    }

  }
}
