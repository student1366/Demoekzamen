﻿using System;
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
    public partial class Users : Form
    {
        enum RowState
        {
            Existed,
            New,
            Modified,
            ModifiedNew,
            Deleted
        }

        string[] rows = new string[] { "Код", "Логин", "Пароль", "Роль" };
        string table = "Пользователи";

        Database database = new Database();
        Request req = new Request();

        int selectedRow;

        public Users()
        {
            InitializeComponent();
        }

        private void order_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }

        #region Вспомогательные функции
        private void clearTextBoxes()
        {
            textBox1.Clear();
            textBox6.Clear();
            textBox8.Clear();
            textBox9.Clear();
            
        }

        private void visibleTextBoxes(bool b)
        {
            if (b)
            {
                button5.Visible = false;
                button2.Visible = false;
                button1.Visible = false;
                button3.Visible = false;
                button6.Visible = true;
                button7.Visible = true;
            }
            else
            {
                button5.Visible = true;
                button2.Visible = true;
                button1.Visible = true;
                button3.Visible = true;
                button6.Visible = false;
                button7.Visible = false;
            }
        }

        private bool checkTextBoxes()
        {
            if (String.IsNullOrWhiteSpace(textBox6.Text) || String.IsNullOrWhiteSpace(textBox8.Text) || String.IsNullOrWhiteSpace(textBox9.Text))
            {
                MessageBox.Show("Данные введены неверно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
        #endregion

        #region дата грид

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("number", "Код");
            dataGridView1.Columns.Add("name", "Логин");
            dataGridView1.Columns.Add("phone", "Пароль");
            dataGridView1.Columns.Add("adres", "Роль");
            dataGridView1.Columns.Add("isNew", String.Empty);
            dataGridView1.Columns["isNew"].Visible = false;
            
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetString(3), RowState.ModifiedNew);
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            SqlDataReader reader = req.SelectAll(table);
            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }
        #endregion

        #region поиск
        private void SearchContext(DataGridView dgw)
        {
            dgw.Rows.Clear();

            SqlDataReader read = req.SelectConcat(rows,table,textBox7.Text);
            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
        }
        #endregion

        #region работа с бд
        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            dataGridView1.Rows[index].Visible = false;
            dataGridView1.Rows[index].Cells[4].Value = RowState.Deleted;
        }

        private void update()
        {
            database.openConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[4].Value;

                switch (rowState)
                {
                    case RowState.Existed:
                        break;
                    case RowState.Deleted:
                        var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                        req.Delete(table, "Код", "=", $"{id}");
                        break;
                    case RowState.Modified:
                        var number = dataGridView1.Rows[index].Cells[0].Value.ToString();
                        var name = dataGridView1.Rows[index].Cells[1].Value.ToString();
                        var phone = dataGridView1.Rows[index].Cells[2].Value.ToString();
                        var adres = dataGridView1.Rows[index].Cells[3].Value.ToString();
                        string[] values = new string[] {number,name,phone,adres };
                        if(!req.Update(rows,values, table))
                        {
                            MessageBox.Show("Не удалось изменить", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            RefreshDataGrid(dataGridView1);
                        }
                        else
                        {
                            MessageBox.Show("Данные изменены", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;
                }
            }
        }

        private void Change()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            database.openConnection();

            string number;
            string name;
            string phone;
            string adres;



            


            if (checkTextBoxes())
            {
                number = textBox1.Text;
                name = textBox6.Text;
                phone = textBox8.Text;
                adres = textBox9.Text;
                
                dataGridView1.Rows[selectedRowIndex].SetValues(number, name, phone, adres);
                dataGridView1.Rows[selectedRowIndex].Cells[4].Value = RowState.Modified;
            }

        }
        #endregion

        #region Обработчики
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox1.Text = row.Cells[0].Value.ToString();
                textBox6.Text = row.Cells[1].Value.ToString();
                textBox8.Text = row.Cells[2].Value.ToString();
                textBox9.Text = row.Cells[3].Value.ToString();
                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            visibleTextBoxes(true);
            clearTextBoxes();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            visibleTextBoxes(false);
            clearTextBoxes();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string number;
            string name;
            string phone;
            string adres;


            if (checkTextBoxes())
            {
                number = textBox1.Text;
                name = textBox6.Text;
                phone = textBox8.Text;
                adres = textBox9.Text;

                string[] values = new string[] { name, phone, adres };
                if(req.Insert(rows, values, table))
                {
                    MessageBox.Show("Запись создана", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    visibleTextBoxes(false);
                    clearTextBoxes();
                }
                else
                {
                    MessageBox.Show("Запись не создана", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            SearchContext(dataGridView1);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            deleteRow();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Change();
        }

        

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            textBox7.Clear();
        }

        #endregion

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
