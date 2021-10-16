using MySql.Data.MySqlClient;
using ShopServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Help
{
    public partial class Data_base_Form : Form
    {
        public Data_base_Form()
        {
            InitializeComponent();

            DB db = new DB(); //выделяем память под объект 

            DataTable table = new DataTable(); //табличка, в которой можно работать 

            MySqlDataAdapter adapter = new MySqlDataAdapter(); //adapter позволяет выбрать данные из базы данных

            //задаем команды по нахождению цены
            MySqlCommand command = new MySqlCommand("SELECT `Code`,`Name`,`Price` FROM `things`", db.GetConnection());

            //заполняем table заданной sql командой
            adapter.SelectCommand = command;
            adapter.Fill(table);

            dataGridView1.Columns.Add("code", "Код");
            dataGridView1.Columns.Add("name", "Название");
            dataGridView1.Columns.Add("price", "Цена");

            foreach (DataRow row in table.Rows)
                dataGridView1.Rows.Add(row.ItemArray);
        }

    }
}
