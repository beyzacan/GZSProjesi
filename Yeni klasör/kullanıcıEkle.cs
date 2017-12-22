using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;

namespace gzsProjesi
{
    public partial class kullanıcıEkle : Form
    {
        private Login lg;
        string sonKayıtId;
        public kullanıcıEkle(Login lg)
        {
            this.lg = lg;
            InitializeComponent();
        }

        private void kullanıcıEkle_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\gen\Documents\Login.mdf;Integrated Security=True;Connect Timeout=30;");
            con.Open();

            SqlCommand ekle = new SqlCommand("insert into [kullanıcı] (adi,soyadi,yas,cinsiyet) values ('"+textBox1.Text+"','"+textBox2.Text+"','"+textBox3.Text+"','"+textBox4.Text+"')SELECT SCOPE_IDENTITY()");
            ekle.Connection = con;
            //ekle.ExecuteNonQuery();
            sonKayıtId = ekle.ExecuteScalar().ToString();

            MessageBox.Show("Yeni Kayıt Başarıyla Eklendi.");

           
            MessageBox.Show("Son kayıt olan kişinin ID numarası : " + sonKayıtId);
            lg.sonEklenenId = Convert.ToInt32(sonKayıtId);

            this.Hide();
            lg.Show();
        }
    }
}
