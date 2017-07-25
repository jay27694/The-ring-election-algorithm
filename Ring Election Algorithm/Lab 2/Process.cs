//JAY AMISHKUMAR PATEL
//1001357017
//https://msdn.microsoft.com/en-us/library/ms171728(VS.80).aspx 
//https://www.youtube.com/watch?v=cHq2lYLA4XY&list=PLAC179D21AF94D28F&index=7
//https://www.youtube.com/watch?v=p8Nlxtj0sV4&index=8&list=PLAC179D21AF94D28F
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// maintain the existed process like its next and previous processes and its socket information 
/// </summary>
namespace Lab_2
{
    class Process
    {
        /// <summary>
        /// local variable for storing the infomation about process
        /// </summary>
        public string ID
        {
            get;
            private set;
        }
        public string Process_Id
        {
            get;
            set;
        }
        public Process Next_Proc
        {
            get;
            set;
        }
        public Process Prev_Proc
        {
            get;
            set;
        }
        public IPEndPoint EndPoint
        {
            get;
            private set;
        }
        public string Co_ord
        {
            get;
            set;
        }
        public int interval
        {
            get;
            set;
        }
        public Socket sck;

        /// <summary>
        /// initialize the process and some local varibles
        /// </summary>
        /// <param name="accepted"></param>
        public Process(Socket accepted)
        {
            sck = accepted;
            ID = Guid.NewGuid().ToString();
            Prev_Proc = null;
            Next_Proc = null;         
            EndPoint = (IPEndPoint)sck.RemoteEndPoint;
            sck.BeginReceive(new byte[] { 0 }, 0, 0, 0, callback, null);
        }

        /// <summary>
        /// start receiving the data from the socket for particular process
        /// </summary>
        /// <param name="ar"></param>
        private void callback(IAsyncResult ar)
        {
            try
            {
                sck.EndReceive(ar);
            
                byte[] buf = new byte[8192];
                int rec = sck.Receive(buf, buf.Length, 0);
                if (rec < buf.Length)
                {
                    Array.Resize<byte>(ref buf, rec);
                }

                var str = System.Text.Encoding.Default.GetString(buf);
                      
                //register mesaage comes, event happen and called the delagate                    
                if (str[0].ToString().Equals("R"))
                {
                    str = str.Substring(1, str.Length - 1);                  
                    if (Registered != null)
                    {
                        Registered(this, str);
                    }
                }
                //coordinator mesaage comes, event happen and called the delagate 
                else if (str[0].ToString().Equals("C"))
                {
                    //str = str.Substring(1, str.Length - 1);
                    if (Co_ordinator != null)
                    {
                        Co_ordinator(this, str);
                    }
                }
                //election mesaage comes, event happen and called the delagate 
                else
                {
                    if (Received != null)
                    {
                        Received(this, str);
                    }
                }
                //continue receiving 
                sck.BeginReceive(new byte[] { 0 }, 0, 0, 0, callback, null);
            }
            catch (Exception e)
            {
                close();
                if (Disconnected != null)
                {
                    Disconnected(this);
                }
            }
        }

        /// <summary>
        /// handle when process crashes
        /// </summary>
        public void close()
        {
            sck.Close();
            sck.Dispose();       
        }

        //event-delegate concept which make server multi threaded
        //different event-delegate used for purposes like receive register message, 
        //coordinato message, election message, and diconnect message
       
        /// <summary>
        /// delagatefinction poiners) is called perticular event will happen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public delegate void ProcessReceivedHandeler(Process sender, string data);
        public delegate void ProcessDisconnected(Process sender);
        public delegate void ProcessRegisterHandler(Process sender, string data);
        public delegate void ProcessCo_ordinatorHandler(Process sender, string data);
      
        /// <summary>
        /// respected events of delegates 
        /// </summary>
        public event ProcessReceivedHandeler Received;
        public event ProcessDisconnected Disconnected;
        public event ProcessRegisterHandler Registered;
        public event ProcessCo_ordinatorHandler Co_ordinator;
    

    }
}
