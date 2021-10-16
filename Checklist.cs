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

namespace Shop_system
{
    public partial class Checklist : Form
    {
        public Checklist(string code, string name, string qt, string money )
        {
            InitializeComponent();

            DB db = new DB(); //выделяем память под объект 

            DataTable table = new DataTable(); //табличка, в которой можно работать 

            MySqlDataAdapter adapter = new MySqlDataAdapter(); //adapter позволяет выбрать данные из базы данных

            //задаем команды по нахождению цены
            MySqlCommand command = new MySqlCommand("SELECT `Price` FROM `things` WHERE `Code` = @cd AND `Name`= @nm", db.GetConnection()); //@cd - заглушка 
            command.Parameters.Add("@cd", MySqlDbType.VarChar).Value = code;
            command.Parameters.Add("@nm", MySqlDbType.VarChar).Value = name;

            //заполняем table заданной sql командой
            adapter.SelectCommand = command;
            adapter.Fill(table);

            //вывод товара
            Name_Check_textBox.Text = name;

            //выврд кол-ва
            qt_CheckList_textBox.Text = qt;

            //вывод цены
            price_CheckList_textBox.Text = Convert.ToString(Convert.ToDouble(table.Rows[0].ItemArray[0]));

            //вывод даты покупки
            Data_CheckList_textBox.Text = Convert.ToString(DateTime.Now.Date);

            //вывод полученных денег
            get_money_CheckLiest_textBox.Text = money;

            //вывод сдачи
            Cash_CheckList_textBox.Text = Convert.ToString(Convert.ToDouble(money) - Convert.ToDouble(qt)*Convert.ToDouble(table.Rows[0].ItemArray[0]));
            
            //вывод итога
            result_CheckList_textBox.Text = Convert.ToString(Convert.ToDouble(table.Rows[0].ItemArray[0]) * Convert.ToDouble(qt));
        }
    }
}
