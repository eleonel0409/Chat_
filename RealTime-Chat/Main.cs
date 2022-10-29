using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading;
using System.Data.SqlClient;

namespace RealTime_Chat
{
    public partial class Main : Form
    {
        Random r = new Random();
        bool suruklenmedurumu = false;
        Point ilkkonum;
        int logId, logStatus;
        string logUsername, logFullname;
        public SqlConnection db = new SqlConnection("Password=GLAP8Le9mb;Persist Security Info=True;User ID=SCA;Initial Catalog=SCA;Data Source=ws1832;connection timeout=300");
        public SqlCommand cmd = new SqlCommand();
        public SqlDataAdapter adtr;
        public SqlDataReader dr;
        public DataSet ds;

        public Main()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage(txtMsg.Text);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            db.Close();
            cmd = new SqlCommand();
            db.Open();
            cmd.Connection = db;
            cmd.CommandText = "Update user_chat set status='" + 0 + "' where id=" + logId + "";
            cmd.ExecuteNonQuery();
            db.Close();
            Application.Exit();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (suruklenmedurumu)
            {
                this.Left = e.X + this.Left - (ilkkonum.X);
                this.Top = e.Y + this.Top - (ilkkonum.Y);
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            suruklenmedurumu = true;
            this.Cursor = Cursors.SizeAll;
            ilkkonum = e.Location; 
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            suruklenmedurumu = false;
            this.Cursor = Cursors.Default; 
        }
      
        private void Main_Load(object sender, EventArgs e)
        {
            logId = Properties.Settings.Default.id;
            logUsername = Properties.Settings.Default.username;
            logFullname = Properties.Settings.Default.fullname;
            logStatus = Properties.Settings.Default.status;
            OnlineLists();
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread sohbet = new Thread(new ThreadStart(AllMessageLists));
            sohbet.IsBackground = true;
            sohbet.Start();
        }

        private void OnlineLists()
        {
            try
            {
                db.Close();
                db.Open();
                cmd.Connection = db;
                cmd.CommandText = "SELECT * FROM user_chat where status = 1";
                cmd.ExecuteNonQuery();
                dr = cmd.ExecuteReader();
                lstOnline.Items.Clear();
                while (dr.Read())
                {
                    lstOnline.Items.Add(dr["username"].ToString() + "  (" + dr["fullname"].ToString() +")");
                }
                //db.Dispose();
                db.Close();
            }
            catch (Exception ex)
            { MessageBox.Show("lstOnline Error : " + ex.Message.ToString() ); }
        }

        private void AllMessageLists()
        {
            while (true)
            {
                try
                {
                    db.Close();
                    db.Open();
                    cmd.Connection = db;
                    cmd.CommandText = "SELECT * FROM message_chat ORDER BY id ASC";
                    cmd.ExecuteNonQuery();
                    dr = cmd.ExecuteReader();
                    txtAll.Clear();
                    while (dr.Read())
                    {
                        txtAll.Text += "(" + dr["time"].ToString() + " / " + dr["fullname"].ToString() + ") :  " + dr["msg"].ToString() +"\n\n";

                    }
                    txtAll.SelectionStart = txtAll.TextLength;
                    txtAll.ScrollToCaret();
                    //db.Dispose();
                    db.Close();
                    OnlineLists();
                    Thread.Sleep(10000);

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private void SendMessage(String mssg)
        {
            try
            {
                db.Close();
                cmd.Connection = db;
                cmd.CommandText = "Insert Into message_chat(time,fullname,msg) Values (getdate()" + ",'" + logFullname + "','" + mssg + "')";
                db.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                db.Close();
                txtMsg.Text = "";

                    
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString());
            }
        }
        /*
          
       
         * 
         * rivate void Form2_Load(object sender, EventArgs e)
        {

            Thread threadim = new Thread(new ThreadStart(listele));
            threadim.Start();
        }
         */
    }
}
