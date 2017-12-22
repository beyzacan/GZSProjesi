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
using System.Data.Sql;


namespace gzsProjesi
{
    public partial class kullanıcıAra : Form

    {
        private Login lg;
        public kullanıcıAra(Login lg)
        {
            this.lg = lg;
            InitializeComponent();
        }
       
        
        private void kullanıcıAra_Load(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\gen\Documents\Login.mdf;Integrated Security=True;Connect Timeout=30;");
            con.Open();
            SqlCommand veri = new SqlCommand("Select * From [kullanıcı]", con);
            SqlDataReader dr = veri.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(dr);
            

            dataGridView1.DataSource = dt;
            con.Close();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\gen\Documents\Login.mdf;Integrated Security=True;Connect Timeout=30;");
            con.Open();
            SqlCommand sorgu = new SqlCommand("Select * From [kullanıcı] where adi='" + textBox1.Text + "'", con);
            SqlDataReader dr = sorgu.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(dr);

            dataGridView1.DataSource = dt;
            con.Close();

            
        }

        private void ListeTiklandi(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            lg.sonEklenenId = id;

            this.Hide();
            lg.Show();
        }
        
    }
}
