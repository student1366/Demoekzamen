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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Reflection.Emit;

namespace УчетЗаявокНаРемонт
{
    public partial class Заявки : Form
    {
        enum RowState
        {
            Existed,
            New,
            Modified,
            ModifiedNew,
            Deleted
        }

        string[] rows = new string[] { "Код", "Начальная_дата", "Тип_авто", "Модель_авто", "Описание_проблемы", "Статус_заявки", "Дата_выполнения", "Запчасти", "Код_сотрудника", "Код_клиента" };
        string table = "Заявки";

        Database database = new Database();
        Request req = new Request();

        int selectedRow;

        public Заявки()
        {
            InitializeComponent();
        }

        private void order_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);

            if (Role.role == "Заказчик")
            {

                button1.Visible = false;
                button2.Visible = false;
                button3.Visible = false;

            }
            else if (Role.role == "Оператор")
            {

              
            }
            else if (Role.role == "Автомеханик")
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
            else if (Role.role == "Менеджер")
            {

            }
        }

        #region Вспомогательные функции
        private void clearTextBoxes()
        {
            textBox5.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox6.Clear();
            textBox9.Clear();
            textBox10.Clear();
            textBox11.Clear();
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
            if (String.IsNullOrWhiteSpace(textBox1.Text) || String.IsNullOrWhiteSpace(textBox2.Text) || String.IsNullOrWhiteSpace(textBox3.Text) || String.IsNullOrWhiteSpace(textBox4.Text)|| String.IsNullOrWhiteSpace(textBox6.Text) || String.IsNullOrWhiteSpace(textBox9.Text) || String.IsNullOrWhiteSpace(textBox10.Text) || String.IsNullOrWhiteSpace(textBox11.Text))
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
            dataGridView1.Columns.Add("num", "Код");
            dataGridView1.Columns.Add("name", "Начальная_дата");
            dataGridView1.Columns.Add("adres", "Тип_авто");
            dataGridView1.Columns.Add("phone", "Модель_авто");
            dataGridView1.Columns.Add("count", "Описание_проблемы");
            dataGridView1.Columns.Add("otdel", "Статус_заявки");
            dataGridView1.Columns.Add("otdel1", "Дата_выполнения");
            dataGridView1.Columns.Add("otdel2", "Запчасти");
            dataGridView1.Columns.Add("otdel3", "Код_сотрудника");
            dataGridView1.Columns.Add("otdel4", "Код_клиента");
            dataGridView1.Columns.Add("isNew", String.Empty);
            dataGridView1.Columns["isNew"].Visible = false;
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetInt32(2), record.GetInt32(3), record.GetString(4), record.GetString(5), record.GetString(6), record.GetString(7), record.GetInt32(8), record.GetInt32(9), RowState.ModifiedNew);
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
        #endregion

        #region работа с бд
        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            dataGridView1.Rows[index].Visible = false;
            dataGridView1.Rows[index].Cells[10].Value = RowState.Deleted;
        }

        private void update()
        {
            database.openConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[10].Value;

                switch (rowState)
                {
                    case RowState.Existed:
                        break;
                    case RowState.Deleted:
                        var i = dataGridView1.Rows[index].Cells[0].Value.ToString();
                        req.Delete(table, "Код", "=", $"{i}");
                        break;
                    case RowState.Modified:
                        var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                        var name = dataGridView1.Rows[index].Cells[1].Value.ToString();
                        var adres = dataGridView1.Rows[index].Cells[2].Value.ToString();
                        var phone = dataGridView1.Rows[index].Cells[3].Value.ToString();
                        var count = dataGridView1.Rows[index].Cells[4].Value.ToString();
                        var otdel = dataGridView1.Rows[index].Cells[5].Value.ToString();
                        var otdel1 = dataGridView1.Rows[index].Cells[6].Value.ToString();
                        var otdel2 = dataGridView1.Rows[index].Cells[7].Value.ToString();
                        var otdel3 = dataGridView1.Rows[index].Cells[8].Value.ToString();
                        var otdel4 = dataGridView1.Rows[index].Cells[9].Value.ToString();
                        string[] values = new string[] { id, name, adres, phone, count, otdel, otdel1, otdel2, otdel3, otdel4 };
                        if (!req.Update(rows,values, table))
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

            string id;
            string name;
            string adres;
            string phone;
            string count;
            string otdel;
            string otdel1;
            string otdel2;
            string otdel3;
            string otdel4;




            if (checkTextBoxes())
            {
                id = textBox5.Text;
                name = textBox1.Text;
                adres = textBox2.Text;
                phone = textBox3.Text;
                count = textBox4.Text;
                otdel = textBox6.Text;
                otdel1 = textBox9.Text;
                otdel2 = textBox10.Text;
                otdel3 = textBox11.Text;
                otdel4 = textBox12.Text;

                dataGridView1.Rows[selectedRowIndex].SetValues(id, name, adres, phone, count, otdel, otdel1, otdel2, otdel3, otdel4);
                dataGridView1.Rows[selectedRowIndex].Cells[10].Value = RowState.Modified;
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

                textBox5.Text = row.Cells[0].Value.ToString();
                textBox1.Text = row.Cells[1].Value.ToString();
                textBox2.Text = row.Cells[2].Value.ToString();
                textBox3.Text = row.Cells[3].Value.ToString();
                textBox4.Text = row.Cells[4].Value.ToString();
                textBox6.Text = row.Cells[5].Value.ToString();
                textBox9.Text = row.Cells[6].Value.ToString();
                textBox10.Text = row.Cells[7].Value.ToString();
                textBox11.Text = row.Cells[8].Value.ToString();
                textBox12.Text = row.Cells[9].Value.ToString();
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
            string id;
            string name;
            string adres;
            string phone;
            string count;
            string otdel;
            string otdel1;
            string otdel2;
            string otdel3;
            string otdel4;


            if (checkTextBoxes())
            {
                name = textBox1.Text;
                adres = textBox2.Text;
                phone = textBox3.Text;
                count = textBox4.Text;
                otdel = textBox6.Text;
                otdel1 = textBox9.Text;
                otdel2 = textBox10.Text;
                otdel3 = textBox11.Text;
                otdel4 = textBox12.Text;
                string[] values = new string[] { name, adres, phone, count, otdel, otdel1, otdel2, otdel3, otdel4 };
                if (!req.Insert(rows, values, table))
                {
                    MessageBox.Show("Запись не создана", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Запись создана", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    visibleTextBoxes(false);
                    clearTextBoxes();
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

        private void button9_Click(object sender, EventArgs e)
        {
            SearchData(dataGridView1,"Статус_заявки", textBox8.Text,"=");
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            textBox8.Clear();
        }
        #endregion
    }
}
