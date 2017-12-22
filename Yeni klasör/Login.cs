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
using GZSProjesi;
using InTheHand.Net.Sockets;

namespace gzsProjesi
{
    public partial class Login : Form
    {
        arduinoController ac;
        private int sayac = 0;
        private int sayac1 = 0;
        public int w = 0;
        int nabiz = 0;
        float sicaklik = 0;

        public int h = 0;
        public int w1 = 0;
        public int h1 = 0;
        public int sonEklenenId { get; set; }
        public String ad, soyad, yas, cinsiyet;
        public BluetoothServer bs;
        public BluetoothClientController bcc;
        public Login()
        {
            this.sonEklenenId = -1;
            InitializeComponent();
            ac = new arduinoController(this);
            bs = new BluetoothServer(this);
            bcc = new BluetoothClientController(this);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            kullanıcıEkle kullanıcı = new kullanıcıEkle(this);
            kullanıcı.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            kullanıcıAra kAra = new kullanıcıAra(this);
            kAra.Show();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ac.gonder("2");
            ac.sayac = 0;
            ac.isNabizDinle = true;
            timer1.Start();
            sayac = 0;
            
            

            button3.Enabled = false;
            button5.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
           
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ac.gonder("1");
            ac.isSicaklikDinle = true;
            sayac1 = 0;
            timer2.Start();
            ac.list = new List<int>();


            button5.Enabled = false;
            button3.Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ac.gonder("1");
            button5.Enabled = true;
            button6.Enabled = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            baglanti baglan = new baglanti();
            baglan.Show();
        }
        public void updateList(List<String> list)
        {
            comboBox1.Items.Clear();
            for (int i = 0;i < list.Count;i++)
            {
                comboBox1.Items.Add(list[i]);
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            ac.scan();
            timer1.Tick += new EventHandler(timer_Tick);
            timer1.Interval = (1000) * 1;
            //timer1.Enabled = true;
            w = pictureBox3.Width;
            h = pictureBox3.Height;
            timer2.Tick += new EventHandler(timer2_Tick);
            timer2.Interval = (1000) * 1;
            w1 = pictureBox4.Width;
            h1 = pictureBox4.Height;
            bs.ConnectAsServer();


        }

        private void button8_Click(object sender, EventArgs e)
        {
            String secilen = comboBox1.Text;
            ac.baglan(secilen);
        }
        void timer_Tick(object sender, EventArgs e)
        {
            sayac += 1;
            if (sayac % 2 == 1)
            {
                pictureBox3.Width = w - 5;
                pictureBox3.Height = h - 10;

            }
            else
            {
                pictureBox3.Width = w;
                pictureBox3.Height = h;
            }
            if (sayac == 30)
            {
                
                    
                timer1.Stop();
                ac.gonder("2");
                ac.isNabizDinle = false;
                button3.Enabled = true;
                button5.Enabled = true;
                MessageBox.Show("Nabız Sayısı:" + (ac.sayac*2));
                nabiz = (ac.sayac * 2);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            sayac1 += 1;
            if(sayac1 %2 == 1)
            {
                pictureBox4.Width = w1 - 5;
                pictureBox4.Height = h1 - 10;
            }
            else
            {
                pictureBox4.Width = w1;
                pictureBox4.Height = h1;

            }
            if (sayac1 == 30)
            {
                timer2.Stop();
                ac.gonder("1");
                ac.isSicaklikDinle = false;
                button5.Enabled = true;
                button3.Enabled = true;
                List<int> gecici = ac.list;
                int toplam = 0;
                foreach(int d in gecici)
                {
                    toplam += d;
                }
                MessageBox.Show("Sıcaklık Değeri:"+(toplam/gecici.Count));
                sicaklik =( toplam / gecici.Count);
            }
            

        }
        public void degerEkle(int nabiz, float sicaklik)
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\gen\Documents\Login.mdf;Integrated Security=True;Connect Timeout=30;");
            con.Open();

            SqlCommand ekle = new SqlCommand("insert into [durum] (Id,nabiz,sicaklik) values (" + sonEklenenId+ "," +nabiz + "," +sicaklik+ ")");
            ekle.Connection = con;
            ekle.ExecuteNonQuery();

            MessageBox.Show("Değerler Eklendi.");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            degerEkle(nabiz, sicaklik);
            BluetoothClient sonGiren = bs.connessioniServer[0];
            String[] gelen=idYeGoreBilgi(sonEklenenId);
        
            String saglik_durumu = "";
            if ((nabiz <= 100 && nabiz >= 60) && (sicaklik >= 25 & sicaklik < 35))
            {
                saglik_durumu = "Normal";
            }
            else
                saglik_durumu = "Riskli";
            String message = gelen[0] + "-" + gelen[1] + "-" + gelen[2] + "-" + gelen[3] + "-" + nabiz.ToString() + "-"
            + sicaklik.ToString()+"-"+saglik_durumu;
            bcc.sendMessageToClient(message, sonGiren);

          
        }

        public String[] idYeGoreBilgi(int id)
        {
            String[] donecek = new String[4];
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\gen\Documents\Login.mdf;Integrated Security=True;Connect Timeout=30;");
            con.Open();

           
            SqlCommand sorgu = new SqlCommand("Select * From [kullanıcı] where Id='" + id + "'", con);
            SqlDataReader dr = sorgu.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(dr);
            ad = dt.Rows[0][1].ToString();
            soyad = dt.Rows[0][2].ToString();
            yas= dt.Rows[0][3].ToString();
            cinsiyet = dt.Rows[0][4].ToString();
            donecek[0] = ad;
            donecek[1] = soyad;
            donecek[2] = yas;
            donecek[3] = cinsiyet;
        
            return donecek;
        }
    }
}
