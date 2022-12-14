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
    public partial class Forgot : Form
    {
        Random r = new Random();
        bool suruklenmedurumu = false;
        Point ilkkonum;
        public SqlConnection db = new SqlConnection("Password=GLAP8Le9mb;Persist Security Info=True;User ID=SCA;Initial Catalog=SCA;Data Source=ws1832;connection timeout=300");
        public SqlCommand cmd = new SqlCommand();
        public SqlDataAdapter adtr;
        public SqlDataReader dr;
        public DataSet ds;

        public Forgot()
        {
            InitializeComponent();
        }

        #region MouseMove
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
       
        }
        #endregion

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                db.Close();
                db.Open();
                cmd = new SqlCommand("Select *From user_chat where username  ='" + txtUsername.Text + "' AND email = '"+txtEmail.Text +"' AND secretanswer ='"+txtSecretanswer.Text+"'", db);
                dr = cmd.ExecuteReader();
                Ping ping = new Ping();
                PingReply pingStatus = ping.Send(IPAddress.Parse("10.10.4.125"));
                if (pingStatus.Status == IPStatus.Success)
                {
                    if (dr.Read())
                    {
                        if (txtUsername.Text.ToString() == dr["username"].ToString())
                        {
                            if (txtEmail.Text.ToString() == dr["email"].ToString())
                            {
                                if (txtSecretanswer.Text.ToString() == dr["secretanswer"].ToString())
                                {
                                    MessageBox.Show("Your Password : " + dr["password"].ToString());
                                    db.Close();
                                    Login log = new Login();
                                    log.Show();
                                    this.Close();
                                    
                                }
                            }
                           
                        }
                        
                    }
                }
                else
                {
                    MessageBox.Show("Check your Internet connection.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    db.Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtEmail.Focus();
            }
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtSecretanswer.Focus();
            }
        }
    }
}
