using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopServer
{
    class DB
    {
        MySqlConnection connection = new MySqlConnection("server=localhost; port=3306;username=root;password=root;database=товары с кодом");

        public void OpenConnection() //открываем соединение с базой данных
        {
            if (connection.State == System.Data.ConnectionState.Closed) 
                connection.Open();
        }

        public void CloseConnection() //закрываем соединение с базой данных
        {
            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }

        public MySqlConnection GetConnection () //возвращает соединение 
        {
            return connection;
        }
    }
}
