/////////////////////////////////////////////////////////////////////
// Window1.xaml.cs - WPF User Interface for WCF Communicator       //
// ver 2.2                                                         //
// Jim Fawcett, CSE681 - Software Modeling & Analysis, Summer 2008 //
/////////////////////////////////////////////////////////////////////
/*
 * Maintenance History:
 * ====================
 * ver 2.2 : 30 Oct 11
 * - added send thread to keep UI from freezing on slow sends
 * - added more comments to clarify what code is doing
 * ver 2.1 : 16 Oct 11
 * - cosmetic changes, posted to the college server but not
 *   distributed in class
 * ver 2.0 : 06 Nov 08
 * - fixed bug that had local and remote ports swapped
 * - made Receive thread background so it would not keep 
 *   application alive after close button is clicked
 * - now closing sender and receiver channels on window
 *   unload
 * ver 1.0 : 17 Jul 07
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace WPF_GUI
{
  public partial class Window1 : Window
  {
    WCF_Peer_Comm.Receiver recvr;
    WCF_Peer_Comm.Sender sndr;
    string rcvdMsg = "";
    int MaxMsgCount = 100;

    Thread rcvThrd = null;
    delegate void NewMessage(string msg);
    event NewMessage OnNewMessage;

    //----< receive thread processing >------------------------------

    void ThreadProc()
    {
      while (true)
      {
        // get message out of receive queue - will block if queue is empty
        rcvdMsg = recvr.GetMessage();

        // call window functions on UI thread
        this.Dispatcher.BeginInvoke(
          System.Windows.Threading.DispatcherPriority.Normal,
          OnNewMessage,
          rcvdMsg);
      }
    }

    //----< called by UI thread when dispatched from rcvThrd >-------

    void OnNewMessageHandler(string msg)
    {
      listBox2.Items.Insert(0, msg);
      if (listBox2.Items.Count > MaxMsgCount)
        listBox2.Items.RemoveAt(listBox2.Items.Count - 1);
    }

    //----< subscribe to new message events >------------------------

    public Window1()
    {
      InitializeComponent();
      Title = "Peer Comm";
      OnNewMessage += new NewMessage(OnNewMessageHandler);
      ConnectButton.IsEnabled = false;
      SendButton.IsEnabled = false;
    }
    //----< start listener >-----------------------------------------

    private void ListenButton_Click(object sender, RoutedEventArgs e)
    {
      string localPort = LocalPortTextBox.Text;
      string endpoint = "http://localhost:" + localPort + "/ICommunicator";

      try
      {
        recvr = new WCF_Peer_Comm.Receiver();
        recvr.CreateRecvChannel(endpoint);

        // create receive thread which calls rcvBlockingQ.deQ() (see ThreadProc above)
        rcvThrd = new Thread(new ThreadStart(this.ThreadProc));
        rcvThrd.IsBackground = true;
        rcvThrd.Start();
        ConnectButton.IsEnabled = true;
        ListenButton.IsEnabled = false;
      }
      catch (Exception ex)
      {
        Window temp = new Window();
        StringBuilder msg = new StringBuilder(ex.Message);
        msg.Append("\nport = ");
        msg.Append(localPort.ToString());
        temp.Content = msg.ToString();
        temp.Height = 100;
        temp.Width = 500;
        temp.Show();
      }
    }
    //----< connect to remote listener >-----------------------------

    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
      string remoteAddress = RemoteAddressTextBox.Text;
      string remotePort = RemotePortTextBox.Text;
      string endpoint = remoteAddress + ":" + remotePort + "/ICommunicator";

      sndr = new WCF_Peer_Comm.Sender(endpoint);
      SendButton.IsEnabled = true;
    }
    //----< send message to connected listener >---------------------

    private void SendMessageButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        listBox1.Items.Insert(0, SendMsgTextBox.Text);
        if (listBox1.Items.Count > MaxMsgCount)
          listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
        sndr.PostMessage(SendMsgTextBox.Text);
      }
      catch (Exception ex)
      {
        Window temp = new Window();
        temp.Content = ex.Message;
        temp.Height = 100;
        temp.Width = 500;
      }
    }

    private void Window_Unloaded(object sender, RoutedEventArgs e)
    {
      sndr.PostMessage("quit");
      sndr.Close();
      recvr.Close();
    }
  }
}
