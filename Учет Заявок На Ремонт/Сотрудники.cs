using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace УчетЗаявокНаРемонт
{
    public partial class Сотрудники : Form
    {
        enum RowState
        {
            Existed,
            New,
            Modified,
            ModifiedNew,
            Deleted
        }

        string[] rows = new string[] { "Код", "Фамилия", "Имя", "Отчество", "Должность", "Телефон", "Пользователь" };
        string table = "Сотрудники";

        Database database = new Database();
        Request req = new Request();

        int selectedRow;
        public Сотрудники()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void Сотрудники_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }

        private void clearTextBoxes()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox8.Clear();


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
            if (String.IsNullOrWhiteSpace(textBox1.Text) || String.IsNullOrWhiteSpace(textBox2.Text) || String.IsNullOrWhiteSpace(textBox3.Text) || String.IsNullOrWhiteSpace(textBox4.Text) || String.IsNullOrWhiteSpace(textBox6.Text) || String.IsNullOrWhiteSpace(textBox8.Text))
            {
                MessageBox.Show("Данные введены неверно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("number", "Код");
            dataGridView1.Columns.Add("name", "Фамилия");
            dataGridView1.Columns.Add("phone", "Имя");
            dataGridView1.Columns.Add("adres1", "Отчество");
            dataGridView1.Columns.Add("adres2", "Должность");
            dataGridView1.Columns.Add("adres3", "Телефон");
            dataGridView1.Columns.Add("adres4", "Пользователь");
            dataGridView1.Columns.Add("isNew", String.Empty);
            dataGridView1.Columns["isNew"].Visible = false;

        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetString(3), record.GetString(4), record.GetString(5), record.GetInt32(6), RowState.ModifiedNew);
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

        private void SearchContext(DataGridView dgw)
        {
            dgw.Rows.Clear();

            SqlDataReader read = req.SelectConcat(rows, table, textBox7.Text);
            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
        }

        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            dataGridView1.Rows[index].Visible = false;
            dataGridView1.Rows[index].Cells[7].Value = RowState.Deleted;
        }

        private void update()
        {
            database.openConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[7].Value;

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
                        var adres1 = dataGridView1.Rows[index].Cells[3].Value.ToString();
                        var adres2 = dataGridView1.Rows[index].Cells[4].Value.ToString();
                        var adres3 = dataGridView1.Rows[index].Cells[5].Value.ToString();
                        var adres4 = dataGridView1.Rows[index].Cells[6].Value.ToString();

                        string[] values = new string[] { number, name, phone, adres1, adres2, adres3, adres4 };
                        if (!req.Update(rows, values, table))
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
            string adres1;
            string adres2;
            string adres3;
            string adres4;






            if (checkTextBoxes())
            {
                number = textBox1.Text;
                name = textBox2.Text;
                phone = textBox3.Text;
                adres1 = textBox4.Text;
                adres2 = textBox8.Text;
                adres3 = textBox5.Text;
                adres4 = textBox6.Text;

                dataGridView1.Rows[selectedRowIndex].SetValues(number, name, phone, adres1, adres2, adres3, adres4);
                dataGridView1.Rows[selectedRowIndex].Cells[7].Value = RowState.Modified;
            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox1.Text = row.Cells[0].Value.ToString();
                textBox2.Text = row.Cells[1].Value.ToString();
                textBox3.Text = row.Cells[2].Value.ToString();
                textBox4.Text = row.Cells[3].Value.ToString();
                textBox8.Text = row.Cells[4].Value.ToString();
                textBox5.Text = row.Cells[5].Value.ToString();
                textBox6.Text = row.Cells[6].Value.ToString();

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            visibleTextBoxes(true);
            clearTextBoxes();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string number;
            string name;
            string phone;
            string adres1;
            string adres2;
            string adres3;
            string adres4;


            if (checkTextBoxes())
            {
                number = textBox1.Text;
                name = textBox2.Text;
                phone = textBox3.Text;
                adres1 = textBox4.Text;
                adres2 = textBox8.Text;
                adres3 = textBox5.Text;
                adres4 = textBox6.Text;

                string[] values = new string[] { name, phone, adres1, adres2, adres3, adres4 };
                if (req.Insert(rows, values, table))
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

        private void button2_Click(object sender, EventArgs e)
        {
            deleteRow();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Change();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            visibleTextBoxes(false);
            clearTextBoxes();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            update();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            textBox7.Clear();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            SearchContext(dataGridView1);
        }
    }
}
