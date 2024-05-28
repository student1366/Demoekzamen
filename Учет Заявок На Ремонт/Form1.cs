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
    public partial class Form1 : Form
    {
        Database database = new Database();
        public Form1()
        {
            InitializeComponent();
             
        }

        private void clients_Click(object sender, EventArgs e)
        {
            Комментарии clients = new Комментарии();
            clients.Show();
            this.Hide();
        }

        private void firms_Click(object sender, EventArgs e)
        {
            Заявки firms = new Заявки();
            firms.Show();
            this.Hide();
        }

        private void tovari_Click(object sender, EventArgs e)
        {
            Модель_авто tovari = new Модель_авто();
            tovari.Show();
            this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("Вы хотите выйти из приложения?", "Выход из приложения", MessageBoxButtons.YesNo);
            if (d == DialogResult.Yes) Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Users postav = new Users();
            postav.Show();
            this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Role.role == "Заказчик")
            {

                groupBox3.Visible = false;

                label2.Visible = false;
                clients.Visible = false;

                label9.Visible = false;
                pictureBox1.Visible = false;

                label14.Visible = false;
                pictureBox7.Visible = false;

                label15.Visible = false;
                pictureBox8.Visible = false;

                label6.Visible = false;
                pictureBox2.Visible = false;

                label10.Visible = false;
                pictureBox5.Visible = false;

            }
            else if (Role.role == "Оператор")
            {

                label15.Visible = false;
                pictureBox8.Visible = false;

                label9.Visible = false;
                pictureBox1.Visible = false;
                groupBox3.Visible = false;
            }
            else if (Role.role == "Автомеханик")
            {

                label6.Visible = false;
                pictureBox2.Visible = false;

                label10.Visible = false;
                pictureBox5.Visible = false;

                label15.Visible = false;
                pictureBox8.Visible = false;
                label9.Visible = false;
                pictureBox1.Visible = false;
                groupBox3.Visible = false;
            }
            else if (Role.role == "Менеджер")
            {

                label9.Visible = false;
                pictureBox1.Visible = false;
                groupBox3.Visible = false;
            }
            else if (Role.role == "Администратор")
            {



            }

        }


        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Детали_заявок postav = new Детали_заявок();
            postav.Show();
            this.Hide();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Представ_комм postav = new Представ_комм();
            postav.Show();
            this.Hide();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Представ_польз postav = new Представ_польз();
            postav.Show();
            this.Hide();
        }


        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Тип_авто tovari = new Тип_авто();
            tovari.Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Клиенты postav = new Клиенты();
            postav.Show();
            this.Hide();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Сотрудники postav = new Сотрудники();
            postav.Show();
            this.Hide();
        }
    }
}
