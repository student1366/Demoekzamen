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

namespace УчетЗаявокНаРемонт
{
    public partial class login : Form
    {
        Database database = new Database();
        public login()
        {
            InitializeComponent();
            
        }



        private void button1_Click(object sender, EventArgs e)
        {
            var userLogin = textBox2.Text;
            var userPass = textBox1.Text;

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();

            string querystring = $"select Логин, Пароль, Роль, Код from Пользователи where Логин = '{userLogin}' and Пароль = '{userPass}'";

            SqlCommand command = new SqlCommand(querystring, database.GetConnection());

            adapter.SelectCommand = command;
            adapter.Fill(table);
            string role = Role.role;
            if (table.Rows.Count == 1)
            {
                MessageBox.Show("Вы успешно зашли!","Успех!", MessageBoxButtons.OK,MessageBoxIcon.Information);
                Form1 frm1 = new Form1();
                database.openConnection();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    role = (string)reader.GetValue(2);
                }
                reader.Close();
                database.closeConnection();
                Role.role = role;
                this.Hide();
                frm1.ShowDialog();
            }
            else MessageBox.Show("Такого аккаунта не существует!", "Аккаунта не существует!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void login_Load(object sender, EventArgs e)
        {
            
            pictureBox3.Visible = false;
            textBox1.MaxLength = 50;
            textBox2.MaxLength = 50;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            textBox1.UseSystemPasswordChar = false;
            pictureBox2.Visible = false;
            pictureBox3.Visible = true;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            textBox1.UseSystemPasswordChar = true;
            pictureBox2.Visible = true;
            pictureBox3.Visible = false;
        }
    }
}
