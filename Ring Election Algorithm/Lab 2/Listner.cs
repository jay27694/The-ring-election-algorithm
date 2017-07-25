//JAY AMISHKUMAR PATEL
//1001357017
//https://msdn.microsoft.com/en-us/library/ms171728(VS.80).aspx 
//https://www.youtube.com/watch?v=cHq2lYLA4XY&list=PLAC179D21AF94D28F&index=7
//https://www.youtube.com/watch?v=p8Nlxtj0sV4&index=8&list=PLAC179D21AF94D28F
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

/// <summary>
/// listens the incoming request of process to server
/// </summary>
namespace Lab_2
{
    class Listner
    {
        //local variables to initialize the incoming process
        //like socket and port 
        Socket s;
        public int Port
        {
            get;
            private set;
        }
   
        public bool listening
        {
            get;
            private set;
        }
      
        public Listner(int port)
        {
            Port = port;
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        //start accepting the new incoming request
        public void start()
        {
            if (listening)
            {
                return;
            }
            s.Bind(new IPEndPoint(0, Port));
            s.Listen(0);
            s.BeginAccept(callback, null);
            listening = true;
        }

        //stop tolisten incoming request
        public void stop()
        {
            if (!listening)
                return;
            s.Close();
            s.Dispose();
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        //continue listing the socket by triggering an delegate when it founds the new request
        void callback(IAsyncResult ar)
        {
            try
            {
                Socket s = this.s.EndAccept(ar);

                if (SocketAccepted != null)
                {
                    SocketAccepted(s);
                }
                this.s.BeginAccept(callback, null);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        //event-delegate to handle the incoming request
        public delegate void SocketAcceptedHandler(Socket e);
        public event SocketAcceptedHandler SocketAccepted;
    }
}
