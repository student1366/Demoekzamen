using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace УчетЗаявокНаРемонт
{
    internal class Request  //Класс отвечающий за запросы к SQL, в нем содержатся методы, которые автоматически формируют запросы к SQL
    {                       //На основе данных, передаваемых в эти методы

        Database database = new Database();

        public void Delete(string table, string item, string property, string value) //Запрос удаления записи из таблицы БД
        {
            var deleteQuery = $"delete from \"{table}\" where [{item}] {property} '{value}'";
            var command = new SqlCommand(deleteQuery, database.GetConnection());
            command.ExecuteNonQuery();
        }

        public SqlDataReader SelectAll(string table) //Запрос получения всех записей из таблицы БД
        {
            string querystring = $"select * from \"{table}\"";
            SqlCommand command = new SqlCommand(querystring, database.GetConnection());
            database.openConnection();
            return command.ExecuteReader();
        }

        public SqlDataReader SelectOneItem(string table, string item, string property, string value) //Запрос отбора записей таблицы по одному полю
        {
            string searchString = $"select * from \"{table}\" where [{item}]{property} '{value}'";
            SqlCommand command = new SqlCommand(searchString, database.GetConnection());
            database.openConnection();
            return command.ExecuteReader();
        }

        public SqlDataReader SelectConcat(string[] mas, string table, string value) //Запрос набора записей таблицы по нескольким полям (Контекстный поиск)
        {
            string searchString = $"select * from {table} where concat (";
            for(int i = 0; i < mas.Length; i++)
            {
                searchString += mas[i];
                if (i < mas.Length - 1)
                {
                    searchString += ",";
                }
            }
            searchString += ") like '%" + value + "%'";
            SqlCommand command = new SqlCommand(searchString, database.GetConnection());
            database.openConnection();
            return command.ExecuteReader();
        }

        public bool Update(string[] rows, string[] values,string table) //Запрос на обновление таблицы в программе, данными из БД
        {
            try    //Обработка исключительной ситуации, которая произойдет при неправильном выполнении запроса
            {
                var changeQuery = $"update {table} set ";
                for(var i = 1; i < rows.Length; i++)
                {
                    changeQuery += $"{rows[i]} = '{values[i]}'";
                    if(i < rows.Length - 1)
                    {
                        changeQuery += ",";
                    }
                }
                changeQuery += $"where {rows[0]} = '{values[0]}'";
                var command = new SqlCommand(changeQuery, database.GetConnection());
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)  //Действия которые будут выполнены при возникновении исключительной ситуации
            {
                MessageBox.Show("При выполнении запроса произошла ошибка, обратитесь к администратору программы", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public bool Insert(string[] rows, string[] values, string table) //Запрос добавления записи в таблицу БД
        {
            try  //Обработка исключительной ситуации, которая произойдет при неправильном выполнении запроса
            {
                database.openConnection();
                var addQuery = $"insert into {table} (";
                for(int i = 1; i < rows.Length; i++)
                {
                    addQuery += $"{rows[i]}";
                    if(i < rows.Length - 1)
                    {
                        addQuery += ",";

                    }
                }
                addQuery += ") values(";
                for (int i = 0; i < values.Length; i++)
                {
                    addQuery += $"'{values[i]}'";
                    if (i < values.Length - 1)
                    {
                        addQuery += ",";

                    }
                }
                addQuery += ")";
                var command = new SqlCommand(addQuery, database.GetConnection());
                command.ExecuteNonQuery();
                database.closeConnection();
                return true;
            }
            catch (Exception ex)        //Действия которые будут выполнены при возникновении исключительной ситуации
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
    }
}
