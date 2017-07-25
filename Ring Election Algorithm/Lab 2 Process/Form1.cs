//JAY AMISHKUMAR PATEL
//1001357017
//https://msdn.microsoft.com/en-us/library/ms171728(VS.80).aspx 
//https://www.youtube.com/watch?v=cHq2lYLA4XY&list=PLAC179D21AF94D28F&index=7
//https://www.youtube.com/watch?v=p8Nlxtj0sV4&index=8&list=PLAC179D21AF94D28F

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

/// <summary>
/// process code
/// </summary>
namespace Lab_2_Process
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// local variables like socket,
        /// event-deleget (continuous ready to receive the data on process side and triggers the event when receives)
        /// </summary>
        Socket sck = null;       
        public delegate void DataAcceptedHandler(Socket e, byte[] data);
        public event DataAcceptedHandler DataAccepted;

        /// <summary>
        /// initialization of windows form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            DataAccepted += Form1_DataAccepted;
        }

        /// <summary>
        /// handaling the data received from the other process
        /// </summary>
        /// <param name="e"></param>
        /// <param name="data"></param>
        private void Form1_DataAccepted(Socket e, byte[] data)
        {
            string s = Encoding.Default.GetString(data);
            Invoke((MethodInvoker)delegate
            {              
                listbox.Items.Add(s);
            });
            sck.Send(Encoding.Default.GetBytes(s + "," + txt_id.Text));          
        }

        /// <summary>
        /// connect the process socket to server socket
        /// </summary>
        /// <param name="sender">pre-defined parameter</param>
        /// <param name="e">pre-defined parameter</param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                sck.Connect("127.0.0.1", 8080);
                //MessageBox.Show("connected");
                Start();
                Invoke((MethodInvoker)delegate
                {
                    btnRegister.Enabled = true;
                });
            }
            catch
            {
                MessageBox.Show("Problem to connect with given server");
                sck = null;
            }
        }

        /// <summary>
        /// process starts receiving the data
        /// </summary>
        private void Start()
        {
            sck.BeginReceive(new byte[] { 0 }, 0, 0, 0, callback, null);
        }

        /// <summary>
        /// handles the data coming froms the other process
        /// </summary>
        /// <param name="ar">pre-defined parameter</param>
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

                //if the coordinator message comes, it will display it 
                //and pass it to the next process if its not the initiator
                if (str[0].ToString() == "C")
                {
                    str = str.Substring(1, str.Length - 1);
                    string s1 = new string(str.Skip(1).ToArray());
                    Invoke((MethodInvoker)delegate
                    {                                               
                        listbox.Items.Add("Co-ordinator is "+ s1);                   
                    });
                    if (str[0].ToString() != txt_id.Text)
                        sck.Send(Encoding.Default.GetBytes("C" + str));                  
                }

                //if it's coordinator,and was crashed, that it will initiate the election mesaage
                //initiates for thr first time if timer finishes 
                else if (str[0].ToString() == "B")
                {
                    sck.Send(Encoding.Default.GetBytes(txt_id.Text));
                } 
                    
                //  if it receives the election mesaage and it's not the initiator of that election message, then it will display and append its id and pass it to the next process
                // if it is initiator of this message , then it will find co-ordinator with maximum process id. and generate Co-ordinator message          
                else if (DataAccepted != null)
                {                   
                    string[] allid = str.Split(',');                    
                    Invoke((MethodInvoker)delegate
                    {
                        if (allid.Contains(txt_id.Text))
                        {                          
                            Invoke((MethodInvoker)delegate
                            {
                                Thread.Sleep(2000);
                                listbox.Items.Add(str);
                            });                            
                            List<int> items = new List<int>();
                            foreach (string xx in allid)
                            {
                                int x = 0;
                                Int32.TryParse(xx, out x);
                                items.Add(x);                                                             
                            }
                            int max = items.Max();                           
                            sck.Send(Encoding.Default.GetBytes("C"+txt_id.Text+ max));
                        }
                        else
                        {
                            DataAccepted(sck, buf);
                        }
                    });                                    
                }
                sck.BeginReceive(new byte[] { 0 }, 0, 0, 0, callback, null);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + ": SERVER WENT OFF");
            }
        }

        /// <summary>
        /// register message send to server with process id
        /// </summary>
        /// <param name="sender">default parameter</param>
        /// <param name="e">default parameter</param>
        private void btnRegister_Click(object sender, EventArgs e)
        {            
            int s = sck.Send(Encoding.Default.GetBytes("R" + txt_id.Text));            
            Invoke((MethodInvoker)delegate
            {
                this.Text = txt_id.Text;
            });           
        }

        /// <summary>
        /// to start manual election message
        /// </summary>
        /// <param name="sender">default parameter</param>
        /// <param name="e">default parameter</param>
        private void btnGenEle_Click(object sender, EventArgs e)
        {
            sck.Send(Encoding.Default.GetBytes(txt_id.Text));
        }
    }
}
