using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;

// /run/lirc/lircd1 = Sender
// /run/lirc/lircd = Receiver

namespace RPi_LED_Display
{
  public class IrReceiver
    {
        public delegate void IrKeyPressHandler(LircEventArgs e);

        public event IrKeyPressHandler IrKeyPress;

        Thread receiveThread;
        bool receiveRunning = false;

        public void Start()
        {
            receiveThread = new Thread(new ThreadStart(receiveThreadRunner));
            receiveThread.Name = "CSharpLircReceiver";

            receiveRunning = true;
            receiveThread.Start();
        }

        public void Stop()
        {
            receiveRunning = false;
            receiveThread.Join();
        }

        private void receiveThreadRunner()
        {
            //Unix socket name
            string lircSocketName = "/run/lirc/lircd1";

            byte[] buffer = new byte[500];
            Socket socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
            UnixDomainSocketEndPoint unixEp = new UnixDomainSocketEndPoint(lircSocketName);
            socket.Connect(unixEp);

            while (receiveRunning)
            {
                if (socket.Poll(1000, SelectMode.SelectRead)) // poll socket once per second
                {
                    int size = socket.Receive(buffer);
                    string command = Encoding.ASCII.GetString(buffer, 0, size);
                    string[] commandParts = command.Split(' ');
                    if (IrKeyPress != null)
                    {
                        IrKeyPress(new LircEventArgs(commandParts[2], commandParts[3]));
                    }
                }
            }
            socket.Close();
        }

    }

    public class IrSender
    {
        //Unix socket name
        string lircSocketName = "/run/lirc/lircd";
        Socket senderSocket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
        UnixDomainSocketEndPoint unixEp;
        Thread commandResultThread;
        bool commandComplete = true;
        bool receiving = false;
        public IrSender()
        {
            unixEp = new UnixDomainSocketEndPoint(lircSocketName);
            senderSocket.Connect(unixEp);
            senderSocket.ReceiveTimeout = 0;
            senderSocket.SendTimeout = 250;
            commandResultThread = new Thread(new ThreadStart(receiverThread));
            commandResultThread.Name = "CommandResultThread";
            commandResultThread.Start();
            
            //SendCommand(Keys.Power, Remotes.Jumbo, 500);
        }

        public enum Keys{Power,bt1,bt2,bt3,bt4,bt5,bt6,bt7,bt8,bt9,bt0,Up,Down,Left,Right,Enter,ChannelUp,ChannelDown,VolumeUp,VolumeDown,Input,Ok,Play,Forward,Backward,Stop,Pause,Record,Info,Clear,Guide,Menu,GoBack,Mute}

        public enum Remotes{ Jumbo, RCA }

        private void receiverThread()
        {
            int count = 0;
            int byteReceived = 0;
            string message;
            while (true)
            {
                try
                {
                    //if (senderSocket.Poll(100, SelectMode.SelectRead))
                    if(commandComplete == false)
                    {
                        Thread.Sleep(50);
                        byte[] bytes = new byte[500];
                        receiving = true;
                        byteReceived = senderSocket.Receive(bytes);
                        receiving = false;
                        message = Encoding.ASCII.GetString(bytes);
                        count += 1;
                        Console.WriteLine(message + "\n" + count.ToString());
                        string[] splitedMessage = message.Split('\n');
                        if(splitedMessage[3] == "END")
                        {
                            commandComplete = true;
                        }
                        else
                        {
                            senderSocket.Send(Encoding.ASCII.GetBytes("\n"));
                            commandComplete = true;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(100);
            }
        }

        public void SendCommand(Keys key, Remotes remote, int count)
        {
            if (!senderSocket.Connected)
                senderSocket.Connect(unixEp);
            string command = "";
            string rem = "";
            switch (remote)
            {
                case Remotes.Jumbo:
                    rem = "Jumbo";
                    break;
                case Remotes.RCA:
                    rem = "RCA";
                    break;
            }
            switch (key)
            {
                case Keys.Power:
                    command = "SEND_ONCE " + rem + " Power " + count.ToString() + "\n";
                    break;
                case Keys.bt1:
                    command = "SEND_ONCE " + rem + " bt-1 " + count.ToString() + "\n";
                    break;
                case Keys.bt2:
                    command = "SEND_ONCE " + rem + " bt-2 " + count.ToString() + "\n";
                    break;
                case Keys.bt3:
                    command = "SEND_ONCE " + rem + " bt-3 " + count.ToString() + "\n";
                    break;
                case Keys.bt4:
                    command = "SEND_ONCE " + rem + " bt-4 " + count.ToString() + "\n";
                    break;
                case Keys.bt5:
                    command = "SEND_ONCE " + rem + " bt-5 " + count.ToString() + "\n";
                    break;
                case Keys.bt6:
                    command = "SEND_ONCE " + rem + " bt-6 " + count.ToString() + "\n";
                    break;
                case Keys.bt7:
                    command = "SEND_ONCE " + rem + " bt-7 " + count.ToString() + "\n";
                    break;
                case Keys.bt8:
                    command = "SEND_ONCE " + rem + " bt-8 " + count.ToString() + "\n";
                    break;
                case Keys.bt9:
                    command = "SEND_ONCE " + rem + " bt-9 " + count.ToString() + "\n";
                    break;
                case Keys.bt0:
                    command = "SEND_ONCE " + rem + " bt-" + rem + " " + count.ToString() + "\n";
                    break;
                case Keys.Up:
                    command = "SEND_ONCE " + rem + " Up " + count.ToString() + "\n";
                    break;
                case Keys.Down:
                    command = "SEND_ONCE " + rem + " Down " + count.ToString() + "\n";
                    break;
                case Keys.Left:
                    command = "SEND_ONCE " + rem + " Left " + count.ToString() + "\n";
                    break;
                case Keys.Right:
                    command = "SEND_ONCE " + rem + " Right " + count.ToString() + "\n";
                    break;
                case Keys.Enter:
                    command = "SEND_ONCE " + rem + " Enter " + count.ToString() + "\n";
                    break;
                case Keys.ChannelUp:
                    command = "SEND_ONCE " + rem + " Channel-up " + count.ToString() + "\n";
                    break;
                case Keys.ChannelDown:
                    command = "SEND_ONCE " + rem + " Channel-down " + count.ToString() + "\n";
                    break;
                case Keys.VolumeUp:
                    command = "SEND_ONCE " + rem + " Volume-up " + count.ToString() + "\n";
                    break;
                case Keys.VolumeDown:
                    command = "SEND_ONCE " + rem + " Volume-Down " + count.ToString() + "\n";
                    break;
                case Keys.Input:
                    command = "SEND_ONCE " + rem + " Input " + count.ToString() + "\n";
                    break;
                case Keys.Ok:
                    command = "SEND_ONCE " + rem + " Ok " + count.ToString() + "\n";
                    break;
                case Keys.Play:
                    command = "SEND_ONCE " + rem + " Play " + count.ToString() + "\n";
                    break;
                case Keys.Forward:
                    command = "SEND_ONCE " + rem + " Forward " + count.ToString() + "\n";
                    break;
                case Keys.Backward:
                    command = "SEND_ONCE " + rem + " Backward " + count.ToString() + "\n";
                    break;
                case Keys.Stop:
                    command = "SEND_ONCE " + rem + " Stop " + count.ToString() + "\n";
                    break;
                case Keys.Pause:
                    command = "SEND_ONCE " + rem + " Pause " + count.ToString() + "\n";
                    break;
                case Keys.Record:
                    command = "SEND_ONCE " + rem + " Record " + count.ToString() + "\n";
                    break;
                case Keys.Info:
                    command = "SEND_ONCE " + rem + " Info " + count.ToString() + "\n";
                    break;
                case Keys.Clear:
                    command = "SEND_ONCE " + rem + " Clear " + count.ToString() + "\n";
                    break;
                case Keys.Guide:
                    command = "SEND_ONCE " + rem + " Guide " + count.ToString() + "\n";
                    break;
                case Keys.Menu:
                    command = "SEND_ONCE " + rem + " Menu " + count.ToString() + "\n";
                    break;
                case Keys.GoBack:
                    command = "SEND_ONCE " + rem + " Go-back " + count.ToString() + "\n";
                    break;
                case Keys.Mute:
                    command = "SEND_ONCE " + rem + " Mute " + count.ToString() + "\n";
                    break;
            }
            if(command != string.Empty)
            {
                //command = "SEND_ONCE Jumbo Power 600 \n";
                try
                {
                    if (commandComplete)
                    {
                        //commandComplete = false;
                        if (senderSocket.Poll(100, SelectMode.SelectWrite))
                        {
                            int byteSent = 0;
                            byteSent = senderSocket.Send(Encoding.ASCII.GetBytes(command));
                            commandComplete = false;
                        }
                    }
                    else if(receiving == true && commandComplete == false)
                    {
                        senderSocket.Send(Encoding.ASCII.GetBytes("\n"));
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

    public class LircEventArgs : EventArgs
    {
        public string KeyCode { get; private set; }
        public string RemoteName { get; private set; }

        public LircEventArgs(string keyCode, string remoteName)
        {
            KeyCode = keyCode;
            RemoteName = remoteName;
        }

    }

}

