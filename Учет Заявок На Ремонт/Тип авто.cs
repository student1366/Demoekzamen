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
    public partial class Тип_авто : Form
    {
        enum RowState
        {
            Existed,
            New,
            Modified,
            ModifiedNew,
            Deleted
        }

        string[] rows = new string[] { "Код", "Название"};
        string table = "Типы_автомобилей";

        Database database = new Database();
        Request req = new Request();

        int selectedRow;
        public Тип_авто()
        {
            InitializeComponent();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string number;
            string name;
          if (checkTextBoxes())
            {
                number = textBox1.Text;
                name = textBox2.Text;

                string[] values = new string[] { name };
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

        private void Test_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
            if (Role.role == "Заказчик" || Role.role == "Специалист")
            {

                button1.Visible = false;
                button2.Visible = false;
                button3.Visible = false;
                button5.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                label7.Visible = false;
                groupBox2.Visible = false;

            }
        }

        private void clearTextBoxes()
        {
            textBox1.Clear();
            textBox2.Clear();


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
            if (String.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Данные введены неверно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }


        private void CreateColumns()
        {
            dataGridView1.Columns.Add("number", "Код");
            dataGridView1.Columns.Add("name", "Название");
            dataGridView1.Columns.Add("isNew", String.Empty);
            dataGridView1.Columns["isNew"].Visible = false;

        }


        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), RowState.ModifiedNew);
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

        private void SearchData(DataGridView dgw, string name, string value, string property)
        {
            dgw.Rows.Clear();

            SqlDataReader read = req.SelectOneItem(table, $"{name}", $"{property}", $"{value}");

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
            dataGridView1.Rows[index].Cells[2].Value = RowState.Deleted;
        }

        private void update()
        {
            database.openConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[2].Value;

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
                        string[] values = new string[] { number, name };
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

            int number;
            string name;
 



            if (checkTextBoxes())
            {
                number = Convert.ToInt32(textBox1.Text);
                name = textBox2.Text;

                dataGridView1.Rows[selectedRowIndex].SetValues(number, name);
                dataGridView1.Rows[selectedRowIndex].Cells[2].Value = RowState.Modified;
            }

        }

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
                textBox2.Text = row.Cells[1].Value.ToString();

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

        private void button9_Click(object sender, EventArgs e)
        {
            SearchData(dataGridView1, "Название", textBox8.Text, "=");
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            textBox8.Clear();
        }
    }
}
