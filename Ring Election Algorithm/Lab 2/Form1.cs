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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// server side code
/// </summary>
namespace Lab_2
{
    public partial class Form1 : Form
    {
        //local variables: listner- to listen new connection, Random: to select two random process        
        Listner listner;
        int count = 0;        
        int backthread = 0;
        List<string> listOfProcess = new List<string>();
        static Random rnd = new Random();

        /// <summary>
        /// initialization of server socket, and calling the socket delegate 
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            lblcounter.Text = count.ToString();    
            listner = new Listner(8080);
            listner.SocketAccepted += L_SocketAccepted;
            Load += new EventHandler(Server_Load);
        }

        /// <summary>
        /// start server listener and background thread
        /// </summary>
        /// <param name="sender">pre-defined parameter</param>
        /// <param name="e">pre-defined parameter</param>
        private void Server_Load(object sender, EventArgs e)
        {
            listner.start();
            backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// initiate the first election message 
        /// here used delay because I am starting the process manually 
        /// </summary>
        private void TokenMessage()
        {            
            Thread.Sleep(100000);
            Process p = null;
            if (backthread == 0)
            {
                List<int> timer = new List<int>();
                Invoke((MethodInvoker)delegate
                {
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        p = listView1.Items[i].Tag as Process;
                        timer.Add(int.Parse(p.interval.ToString()));
                    }
                    int min = timer.Min();
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        p = listView1.Items[i].Tag as Process;
                        if (int.Parse(p.interval.ToString()) == min)
                        {
                            break;
                        }
                    }
                });
                Thread.Sleep(timer.Min());
                byte[] backup = Encoding.Default.GetBytes("B");
                p.sck.Send(backup, 0, backup.Length, 0);
                backthread++;   
            }                                               
        }

        /// <summary>
        /// start multiple election simultaneously by selectioj two random processes which are alive 
        /// </summary>
        private void MultipleElection()
        {
            Invoke((MethodInvoker)delegate
            {
                if (listView1.Items.Count >= 2)
                {
                    List<string> listofids = new List<string>();
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        Process process = listView1.Items[i].Tag as Process;
                        listofids.Add(process.Process_Id);
                    }

                    int r = rnd.Next(listofids.Count);
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {

                        Process process = listView1.Items[i].Tag as Process;
                        if (process.Process_Id == (string)listofids[r])
                        {
                            byte[] backup = Encoding.Default.GetBytes("B");
                            process.sck.Send(backup, 0, backup.Length, 0);
                            Invoke((MethodInvoker)delegate
                            {
                                count++;
                                lblcounter.Text = count.ToString();
                            });
                            //MessageBox.Show((string)listofids[r] + " " + process.Process_Id);
                        }

                    }

                    listofids.Remove((string)listofids[r]);
                    r = rnd.Next(listofids.Count);
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {

                        Process process = listView1.Items[i].Tag as Process;
                        if (process.Process_Id == (string)listofids[r])
                        {
                            byte[] backup = Encoding.Default.GetBytes("B");
                            process.sck.Send(backup, 0, backup.Length, 0);
                            Invoke((MethodInvoker)delegate
                            {
                                count++;
                                lblcounter.Text = count.ToString();
                            });
                            //MessageBox.Show((string)listofids[r] + " " + process.Process_Id);
                        }

                    }
                }

            });
        }

        /// <summary>
        /// call the tocken message method used for first election message
        /// </summary>
        /// <param name="sender">pre-defined parameter</param>
        /// <param name="e">pre-defined parameter</param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {              
                TokenMessage();              
            }
        }
 
        /// <summary>
        /// accept the new process, maintain the ring and server side list
        /// </summary>
        /// <param name="e">process socket</param>
        private void L_SocketAccepted(System.Net.Sockets.Socket e)
        {
            Process p = new Process(e);
                     
            p.Registered += P_Registered;
            p.Received += P_Received;
            p.Disconnected += P_Disconnected;
            p.Co_ordinator += P_Co_ordinator;
          
            Invoke((MethodInvoker)delegate
            {
                ListViewItem l = new ListViewItem();
                l.Tag = p;
                l.Text = p.EndPoint.ToString();
                l.SubItems.Add(p.ID); //item1 - id
                l.SubItems.Add("XX"); //item2 - next proc
                l.SubItems.Add("XX"); //item3 - prev proc
                listView1.Items.Add(l);
            });
        }
       
        /// <summary>
        /// receives the elected coordinator from the process and passed to the next process
        /// </summary>
        /// <param name="sender">process which send this message</param>
        /// <param name="data">selected co-ordinator at particular time</param>
        private void P_Co_ordinator(Process sender, string data)
        {
            string strnew = data.Substring(1, data.Length - 1);
            string s1 = new string(strnew.Skip(1).ToArray());

            Invoke((MethodInvoker)delegate
            {
                lblcoord.Text = s1;
            });

            Invoke((MethodInvoker)delegate
            {
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    Process process = listView1.Items[i].Tag as Process;
                    if (process.ID == sender.ID)
                    {
                        new Thread(() =>
                        {
                            send_message_co(process, data);
                        }).Start();
                        break;
                    }
                }
            });       
        }

        /// <summary>
        /// sends the co-ordinator to next process 
        /// </summary>
        /// <param name="p">sender process</param>
        /// <param name="data">data which it wants to send (coordinator number)</param>
        private void send_message_co(Process p, string data)
        {
            Thread.Sleep(1000);
            byte[] data1 = Encoding.Default.GetBytes(data);          
            p.Next_Proc.sck.Send(data1, 0, data.Length, 0);
            Invoke((MethodInvoker)delegate
            {
                count++;
                lblcounter.Text = count.ToString();
            });
        }

        /// <summary>
        /// receives the election message from the sending process and pass it to the next process in ring
        /// </summary>
        /// <param name="sender">sending process</param>
        /// <param name="data">election message</param>
        private void P_Received(Process sender, string data)
        {
            Invoke((MethodInvoker)delegate
            {
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    Process process = listView1.Items[i].Tag as Process;
                    if (process.ID == sender.ID)
                    {                                               
                        new Thread(() =>
                        {
                            send_message(process, data);
                        }).Start();
                        break;
                    }
                }
            });
        }

        /// <summary>
        /// send election message to next process in the ring
        /// </summary>
        /// <param name="p">sender</param>
        /// <param name="data">election message</param>
        private void send_message(Process p, string data)
        {           
            Thread.Sleep(p.Next_Proc.interval);
            byte[] data1 = Encoding.Default.GetBytes(data);
            p.Next_Proc.sck.Send(data1, 0, data.Length, 0);
            Invoke((MethodInvoker)delegate
            {
                count++;
                lblcounter.Text = count.ToString();
            });
        }

        /// <summary>
        /// mannually registering the processes with its ids
        /// </summary>
        /// <param name="sender">process which wants be registerd</param>
        /// <param name="data">its Id</param>
        private void P_Registered(Process sender, string data)
        {
                
            int? I = null;
            Invoke((MethodInvoker)delegate
            {
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    Process process = listView1.Items[i].Tag as Process;
                    if (process.ID == sender.ID)
                    {
                        process.Process_Id = data;
                        process.interval = int.Parse(process.Process_Id) * 1000;                   
                        listView1.Items[i].SubItems[1].Text = process.Process_Id;
                        I = i;                     
                        break;
                    }
                }              
            });
            makeRing();
            if (!listOfProcess.Contains(data))
                listOfProcess.Add(data);
            else
            {               
                byte[] backup = Encoding.Default.GetBytes("B");
                sender.sck.Send(backup, 0, backup.Length, 0);
                Invoke((MethodInvoker)delegate
                {
                    count++;
                   lblcounter.Text = count.ToString();
                });
            }               
        }

        /// <summary>
        /// crash the process and generate election if coordinator crashes  
        /// </summary>
        /// <param name="sender">process which disconneted</param>
        private void P_Disconnected(Process sender)
        {           
            Invoke((MethodInvoker)delegate
            {
                Process dis_connected = null, process = null;
                process = sender.Next_Proc;
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    dis_connected = listView1.Items[i].Tag as Process;
                    if (dis_connected.ID == sender.ID)
                    {
                        listView1.Items.RemoveAt(i);
                        break;
                    }
                }
                
                makeRing();
                if (lblcoord.Text == sender.Process_Id && listView1.Items.Count>0)
                {
                    MultipleElection();                  
                }
                if (listView1.Items.Count == 0)
                {
                    lblcoord.Text = "-";
                }           
            });   
        }

        /// <summary>
        /// maintain the ring every time when new process comes or existed process crashes
        /// </summary>
        private void makeRing()
        {
            Invoke((MethodInvoker)delegate
            {
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    Process process = listView1.Items[i].Tag as Process;
                    if (i == 0)
                    {
                        if (listView1.Items.Count == 1)
                        {
                            process.Next_Proc = process;
                            process.Prev_Proc = process;
                            listView1.Items[0].SubItems[2].Text = process.Process_Id;
                            listView1.Items[0].SubItems[3].Text = process.Process_Id;
                        }
                        else
                        {
                            process.Next_Proc = listView1.Items[1].Tag as Process;
                            process.Prev_Proc = listView1.Items[listView1.Items.Count - 1].Tag as Process;
                            listView1.Items[0].SubItems[2].Text = process.Next_Proc.Process_Id;
                            listView1.Items[0].SubItems[3].Text = process.Prev_Proc.Process_Id;
                        }
                    }
                    else if (i == listView1.Items.Count - 1)
                    {
                        process.Next_Proc = listView1.Items[0].Tag as Process;
                        process.Prev_Proc = listView1.Items[i - 1].Tag as Process;
                        listView1.Items[i].SubItems[2].Text = process.Next_Proc.Process_Id;
                        listView1.Items[i].SubItems[3].Text = process.Prev_Proc.Process_Id;
                    }
                    else
                    {
                        process.Next_Proc = listView1.Items[i + 1].Tag as Process;
                        process.Prev_Proc = listView1.Items[i - 1].Tag as Process;
                        listView1.Items[i].SubItems[2].Text = process.Next_Proc.Process_Id;
                        listView1.Items[i].SubItems[3].Text = process.Prev_Proc.Process_Id;
                    }

                }
            });
        }

       
    }
}
