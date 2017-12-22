using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace gzsProjesi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\gen\Documents\Login.mdf;Integrated Security=True;Connect Timeout=30;");
            SqlDataAdapter sda = new SqlDataAdapter("Select Count (*) From [Table] where kAdi='"+textBox1.Text+"'and sifre='"+textBox2.Text+"'",con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows[0][0].ToString() == "1")
            {
                this.Hide();
                Login login = new Login();
                login.Show();
            }
            else
            {
                MessageBox.Show("Lütfen Kullanıcı Adı ve Şifrenizi Kontrol Edin.");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
