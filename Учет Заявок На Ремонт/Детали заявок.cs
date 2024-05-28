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
    public partial class Детали_заявок : Form
    {
        enum RowState
        {
            Existed,
            New,
            Modified,
            ModifiedNew,
            Deleted
        }

        string[] rows = new string[] { "Код", "Начальная_дата", "[Тип авто]", "[Модель авто]", "Описание_проблемы", "Статус_заявки", "[Дата выполнения]", "Запчасти","[Фамилия сотрудника]", "[Имя сотрудника]", "[Фамилия клиента]", "[Имя клиента]" };
        string table = "View_1";

        Database database = new Database();
        Request req = new Request();

        int selectedRow;

        public Детали_заявок()
        {
            InitializeComponent();
        }

        private void order_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }


        #region дата грид

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("order_number", "Код");
            dataGridView1.Columns.Add("client_number", "Начальная_дата");
            dataGridView1.Columns.Add("tovar_number", "Тип_авто");
            dataGridView1.Columns.Add("postavschik_number", "Модель_авто");
            dataGridView1.Columns.Add("count", "Описание_проблемы");
            dataGridView1.Columns.Add("date", "Статус_заявки");
            dataGridView1.Columns.Add("cost", "Дата_выполнения");
            dataGridView1.Columns.Add("cost1", "Запчасти");
            dataGridView1.Columns.Add("cost1", "Фамилия сотрудника");
            dataGridView1.Columns.Add("cost1", "Имя сотрудника");
            dataGridView1.Columns.Add("cost1", "Фамилия клиента");
            dataGridView1.Columns.Add("cost1", "Имя клиента");
            dataGridView1.Columns.Add("isNew", String.Empty);
            dataGridView1.Columns["isNew"].Visible = false;
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetString(3), record.GetString(4), record.GetString(5), record.GetString(6), record.GetString(7), record.GetString(8), record.GetString(9), record.GetString(10), record.GetString(11), RowState.ModifiedNew);
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
            dataGridView1.Rows[index].Cells[12].Value = RowState.Deleted;
        }

        private void update()
        {
            database.openConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[12].Value;

                switch (rowState)
                {
                    case RowState.Existed:
                        break;
                    case RowState.Deleted:

                        break;
                    case RowState.Modified:
                        break;
                }
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

        }

        private void button4_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            SearchContext(dataGridView1);

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            update();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            textBox7.Clear();
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
