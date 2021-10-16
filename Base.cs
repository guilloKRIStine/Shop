using Help;
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
    public partial class Standart : Form
    {
        public Standart() //метод для поддержки конструктора
        {
            InitializeComponent();

            Registration_dataGridView.Columns.Add("code", "Код товара");
            Registration_dataGridView.Columns.Add("name", "Название");
            Registration_dataGridView.Columns.Add("unit", "Единица измерения");
            Registration_dataGridView.Columns.Add("qt", "Количество");
            Registration_dataGridView.Columns.Add("data", "Дата завоза");
            Registration_dataGridView.Columns.Add("price", "Цена");
        }

        //----------------------------------------------------------------------------//
        //МЕНЮ
        //----------------------------------------------------------------------------//

        private void Buy_thing_Click(object sender, EventArgs e) // кнопка перехода из меню в покупку товара
        {
            panel1.Visible = false;
            buy.Visible = true;
        }

        private void Check_thing_Click(object sender, EventArgs e) // кнопка перехода из меню в наличие товара
        {
            panel1.Visible = false;
            buy.Visible = false;
            Check_thing_panel.Visible = true;
        }

        private void Registration_thing_Click(object sender, EventArgs e) // кнопка перехода из меню в регистрацию товара
        {
            panel1.Visible = false;
            buy.Visible = false;
            Check_thing_panel.Visible = true;

            Registration_panel.Visible = true;
        }

        //----------------------------------------------------------------------------//
        //ПЕВРАЯ ПАНЕЛЬ ПОКУПКИ
        //----------------------------------------------------------------------------//

        private void Button_menu_Click(object sender, EventArgs e) // выход в меню
        {
            buy.Visible = false;
            panel1.Visible = true;
        }

        private void info_panel1_Click(object sender, EventArgs e) // информация для покупателя
        {
            MessageBox.Show("Покупка будет проведена ТОЛЬКО когда Вы оформите чек.");
        }

        private void Help_Panel1_button_Click(object sender, EventArgs e) // ассортимент
        {
            var Form3 = new Data_base_Form();
            Form3.Show();
        }

        // ПРОВЕРКИ //

        //проверка ввода название товара
        private void Name_thing_textBox_panel1_KeyPress(object sender, KeyPressEventArgs e) //запрет на ввод специальных знаков
        {
            char number = e.KeyChar;

            //             @               #               ^               &              *                /              -
            if (number == 64 || number == 35 || number == 94 || number == 38 || number == 42 || number == 47 || number == 45)
            {
                e.Handled = true;
            }
        }

        //проверка ввода кода товара
        private void Code_thing_KeyPress(object sender, KeyPressEventArgs e) //запрет на ввод букв
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != 8 || number == 45) // цифры и клавиша BackSpace и минус
            {
                e.Handled = true;
            }
        }

        //проверка ввода кол-ва товара
        private void Qt_thing_KeyPress(object sender, KeyPressEventArgs e) //запрет на ввод букв
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != 8 ) // цифры и клавиша BackSpace и минус
            {
                e.Handled = true;
            }
        }

        //проверка ввода полученных денег
        private void Get_money_KeyPress(object sender, KeyPressEventArgs e) //запрет на ввод букв
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != 8 && number != 44 ) // цифры и клавиша BackSpace + точка
            {
                e.Handled = true;
            }
        }

        // ОСНОВА //

        private void OK_button_Click(object sender, EventArgs e) //когда нажата ок в покупке
        {
            String Code_thing_panel1 = Code_thing.Text; //переменная для кода
            String Name_thing_panel1 = Name_thing_textBox_panel1.Text; // для названия
            String Qt_thing_panel1 = Qt_thing.Text; //для кол-ва

            DB db = new DB(); //выделяем память под объект 

            DataTable table = new DataTable(); //табличка, в которой можно работать 

            DataTable table2 = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter(); //adapter позволяет выбрать данные из базы данных

            //задаем команды по нахождению цены
            MySqlCommand command = new MySqlCommand("SELECT `Price` FROM `things` WHERE `Code` = @cd AND `Name`= @nm", db.GetConnection()); //@cd - заглушка 

            //для проверки даты
            MySqlCommand command2 = new MySqlCommand("SELECT `Data` FROM `things` WHERE `Code` = @cd ", db.GetConnection());
            
            command.Parameters.Add("@cd", MySqlDbType.VarChar).Value = Code_thing_panel1;
            command.Parameters.Add("@nm", MySqlDbType.VarChar).Value = Name_thing_panel1;

            command2.Parameters.Add("@cd", MySqlDbType.VarChar).Value = Code_thing_panel1;

            //заполняем table заданной sql командой
            adapter.SelectCommand = command;
            adapter.Fill(table);

            //заполняем table2 заданной sql командой
            adapter.SelectCommand = command2;
            adapter.Fill(table2);

                if (table.Rows.Count > 0 && Convert.ToDateTime(table2.Rows[0].ItemArray[0]) <= Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    Price_to_pay.Clear(); //очищаю панельку
                    Price_to_pay.Text = Convert.ToString(Convert.ToDouble(table.Rows[0].ItemArray[0]) * Convert.ToDouble(Qt_thing_panel1));
                }
                else
                {
                    Price_to_pay.Clear(); //очищаю панельку
                    Price_to_pay.Text = "ТАКОГО ТОВАРА НЕТ ИЛИ ЕГО НЕТ В ТАКОМ КОЛИЧЕСТВЕ";

                    Code_thing.Clear();
                    Name_thing_textBox_panel1.Clear();
                    Qt_thing.Clear();
                }

        }

        // ВЫВОД ЧЕКА И ИЗМЕНЕНИЕ БАЗЫ ДАННЫХ

        private void Checklist_button_Click(object sender, EventArgs e) //вывод чека при нажатие кнопки чек
        {
            if (Price_to_pay.Text != "ТАКОГО ТОВАРА НЕТ ИЛИ ЕГО НЕТ В НАЛИЧИИ В ТАКОМ КОЛИЧЕСТВЕ. ПРОВЕРЬТЕ НАЛИЧИЕ ТОВАРА." && Price_to_pay.Text != "")
            {
                String Name_thing_panel1 = Name_thing_textBox_panel1.Text; // для названия
                String Qt_thing_panel1 = Qt_thing.Text; //для кол-ва
                String Money = Get_money.Text; //переменная для полученных денег
                String Code_thing_panel1 = Code_thing.Text; //переменная для кода

                if (Convert.ToDouble(Money) < Convert.ToDouble(Price_to_pay.Text))
                    MessageBox.Show("НЕДОСТАТОЧНО СРЕДСТВ ДЛЯ ОПЛАТЫ");
                else
                {
                    DB db = new DB(); //выделяем память под объект 

                    MySqlCommand command = new MySqlCommand("UPDATE `things` SET `Qt` = `Qt` - @qt  WHERE `Code` = @cd ", db.GetConnection());

                    command.Parameters.Add("@qt", MySqlDbType.VarChar).Value = Qt_thing.Text;
                    command.Parameters.Add("@cd", MySqlDbType.VarChar).Value = Code_thing.Text;

                    db.OpenConnection();

                    if (command.ExecuteNonQuery() == 1) //проверка на корректность работы
                    {
                        MessageBox.Show("Изменения были добавлены в базу данных");

                        Price_to_pay.Clear();
                        Name_thing_textBox_panel1.Clear();
                        Qt_thing.Clear();
                        Get_money.Clear();
                        Code_thing.Clear();
                    }
                    else
                        MessageBox.Show("Изменения НЕ были добавлены в базу данных");

                    db.CloseConnection();

                    var Form2 = new Checklist(Code_thing_panel1, Name_thing_panel1, Qt_thing_panel1, Money);
                    Form2.Show();
                }
            }
            else
                MessageBox.Show("ВЫ НИЧЕГО НЕ ВЫБРАЛИ ДЛЯ ПОКУПКИ. ВЫБЕРИТЕ ТОВАР, ЕГО КОД И НАЖМИТЕ 'ОК' ");
        }

        //----------------------------------------------------------------------------//
        //ВТОРАЯ ПАНЕЛЬ ПРОВЕРКА НАЛИЧИЯ ТОВАРА
        //----------------------------------------------------------------------------//

        private void Menu_button2_Click(object sender, EventArgs e) //выход из проверки наличия в меню
        {
            Check_thing_panel.Visible = false;
            panel1.Visible = true;
        }

        private void Help_button_panel2_Click(object sender, EventArgs e) // ассортимент
        {
            var Form3 = new Data_base_Form();
            Form3.Show();
        }

        // ПРОВЕРКИ //

        //запрет на ввод специальных знаков
        private void Name_thing_textBox1_panel2_KeyPress(object sender, KeyPressEventArgs e) 
        {
            char number = e.KeyChar;

            //             @               #               ^               &              *                /              -
            if (number == 64 || number == 35 || number == 94 || number == 38 || number == 42 || number == 47 || number == 45)
            {
                e.Handled = true;
            }
        }

        //запрет на ввод букв
        private void Qt_comboBox2_KeyPress(object sender, KeyPressEventArgs e) 
        {
            char number = e.KeyChar;
            
            if (!Char.IsDigit(number) && number != 8 ) // цифры и клавиша BackSpace и -
            {
                e.Handled = true;
            }
        }

        // ОСНОВА //

        private void OK_button2_Click(object sender, EventArgs e) //нажатие кнопки ок в проверке наличия
        {
            String Code_thing_panel2 = Qt_comboBox2.Text; //переменная для кода
            String Name_thing_panel2 = Name_thing_textBox1_panel2.Text;

            DB db = new DB(); //выделяем память под объект 

            DataTable table = new DataTable(); //табличка, в которой можно работать 

            MySqlDataAdapter adapter = new MySqlDataAdapter(); //adapter позволяет выбрать данные из базы данных

            //находим наличие по коду и имени
            MySqlCommand command = new MySqlCommand("SELECT `Qt` FROM `things` WHERE `Code` = @cd AND `Name`= @nm", db.GetConnection()); //@cd - заглушка 
            command.Parameters.Add("@cd", MySqlDbType.VarChar).Value = Code_thing_panel2;
            command.Parameters.Add("@nm", MySqlDbType.VarChar).Value = Name_thing_panel2;
            
            //заполняем table заданной sql командой
            adapter.SelectCommand = command;
            adapter.Fill(table);      

            if (table.Rows.Count > 0)
            {
                Code_thing_textBox2.Clear(); //очищаю панельку
                Code_thing_textBox2.Text = Convert.ToString(table.Rows[0].ItemArray[0]);
            }
            else
            {
                Code_thing_textBox2.Clear(); //очищаю панельку
                Code_thing_textBox2.Text = "НЕТ В НАЛИЧИИ";
            }
        }

        //----------------------------------------------------------------------------//
        //ТРЕТЬЯ ПАНЕЛЬ РЕГИСТРАЦИЯ ТОВАРА
        //----------------------------------------------------------------------------//

        private void Menu_button3_Click(object sender, EventArgs e) //выход из  регистрации в меню
        {
            Registration_panel.Visible = false;
            Check_thing_panel.Visible = false;
            panel1.Visible = true;
        }

        private void Show_thing_button_Click(object sender, EventArgs e) // показываю товары
        {
            DB db = new DB(); //выделяем память под объект 

            DataTable table = new DataTable(); //табличка, в которой можно работать 

            MySqlDataAdapter adapter = new MySqlDataAdapter(); //adapter позволяет выбрать данные из базы данных

            //задаем команды по нахождению цены
            MySqlCommand command = new MySqlCommand("SELECT `Code`,`Name`,`Unit`,`Qt`,`Data`,`Price` FROM `things`", db.GetConnection());

            //заполняем table заданной sql командой
            adapter.SelectCommand = command;
            adapter.Fill(table);

            foreach (DataRow row in table.Rows)
                Registration_dataGridView.Rows.Clear();

            foreach (DataRow row in table.Rows)
                Registration_dataGridView.Rows.Add(row.ItemArray);
        }

        // ИНФОРМАЦИЯ

        private void Code_button_panel3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Код нужно вводить ТОЛЬКО при замене или удалении товара. В регистрации он не будет учтен.");
        }

        private void Name_button_panel3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В этой графе нужно ввести НАИМЕНОВАНИЕ товара");
        }

        private void Unit_button_panel3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В этой графе нужно ввести ЕДИНИЦУ ИЗМЕРЕНИЯ товара");
        }

        private void Qt_button_panel3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В этой графе нужно ввести КОЛИЧЕСТВО товара");
        }

        private void Data_button_panel3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В этой графе нужно ввести ДАТУ ЗАВОЗА товара");
        }

        private void Price_button_panel3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В этой графе нужно ввести ЦЕНУ товара");
        }

        //ПРОВЕРКИ ВВОДА

        //код
        private void Code_textBox_panel3_KeyPress(object sender, KeyPressEventArgs e) //запрет на ввод букв
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != 8 || number == 45) // цифры и клавиша BackSpace и минус
            {
                e.Handled = true;
            }
        }

        //название
        private void Name_textBox_panel3_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            //             @               #               ^               &              *                /              -
            if (number == 64 || number == 35 || number == 94 || number == 38 || number == 42 || number == 47 || number == 45)
            {
                e.Handled = true;
            }
        }

        //ед. измерения
        private void Unit_textBox_panel3_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            //             @               #               ^               &              *                /              -
            if (Char.IsDigit(number) || number == 64 || number == 35 || number == 94 || number == 38 || number == 42 || number == 47 || number == 45)
            {
                e.Handled = true;
            }
        }

        //количество
        private void Qt_textBox_panel3_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace и -
            {
                e.Handled = true;
            }
        }

        //цена
        private void Price_textBox_panel3_TextChanged(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != 8 && number != 44) // цифры и клавиша BackSpace + точка
            {
                e.Handled = true;
            }
        }

        //дата
        public void SetMyCustomFormat(object sender, EventArgs e)
        {
            dateTimePicker.Format = DateTimePickerFormat.Custom;
            dateTimePicker.CustomFormat = "yyyy-MM-dd";
        }

        // РЕГИСТРАЦИЯ

        private void Registration_thing_button_Click(object sender, EventArgs e)
        {
            if (Name_textBox_panel3.Text != "" && Unit_textBox_panel3.Text != "" && Qt_textBox_panel3.Text != "" && Price_textBox_panel3.Text != "")
            {
                DB db = new DB(); //выделяем память под объект 

                MySqlCommand command = new MySqlCommand("INSERT INTO `things` (`Name`, `Unit`, `Qt`, `Data`, `Price`) VALUES (@name, @unit, @qt, @date, @price)", db.GetConnection());

                //название
                command.Parameters.Add("@name", MySqlDbType.VarChar).Value = Name_textBox_panel3.Text;
                //ед. измерения
                command.Parameters.Add("@unit", MySqlDbType.VarChar).Value = Unit_textBox_panel3.Text;
                //кол-во
                command.Parameters.Add("@qt", MySqlDbType.VarChar).Value = Qt_textBox_panel3.Text;
                //дата
                command.Parameters.Add("@date", MySqlDbType.VarChar).Value = dateTimePicker.Text;
                //цена
                command.Parameters.Add("@price", MySqlDbType.Double).Value = Price_textBox_panel3.Text;

                db.OpenConnection();

                if (command.ExecuteNonQuery() == 1) //проверка на корректность работы
                {
                    MessageBox.Show("Товар был добавлен в базу данных. Чтобы посмотреть изменения, нажмите снова на кнопку 'Посмотреть товары' ");

                    Name_textBox_panel3.Clear();
                    Unit_textBox_panel3.Clear();
                    Qt_textBox_panel3.Clear();
                    Price_textBox_panel3.Clear();
                }
                else
                    MessageBox.Show("Товар НЕ был добавлен в базу данных");


                db.CloseConnection(); //если не закрыть, будет большая нагрузка
            }
            else
                MessageBox.Show("Введите все данные, чтобы зарегестрировать товар.");
        }

        // ЗАМЕНА ДАННЫХ В ТОВАРЕ

        private void Change_thing_button_Click(object sender, EventArgs e)
        {

            DB db = new DB(); //выделяем память под объект 

            DataTable table = new DataTable(); //табличка, в которой можно работать 

            MySqlDataAdapter adapter = new MySqlDataAdapter(); //adapter позволяет выбрать данные из базы данных

            //задаем команды по нахождению цены
            MySqlCommand command = new MySqlCommand("SELECT `Code` FROM `things` WHERE `Code` = @cd AND `Name` = @nm", db.GetConnection()); //@cd - заглушка 
            
            command.Parameters.Add("@cd", MySqlDbType.VarChar).Value = Code_textBox_panel3.Text;
            command.Parameters.Add("@nm", MySqlDbType.VarChar).Value = Name_textBox_panel3.Text;
            
            //заполняем table заданной sql командой
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0 && (Qt_textBox_panel3.Text != "" || Price_textBox_panel3.Text != ""))  //если такой есть
            {
                MySqlCommand command2 = new MySqlCommand("UPDATE `things` SET `Name` =@name, `Unit`=@unit, `Qt`=@qt, `Data`=@date, `Price`=@price WHERE `Code` = @code ", db.GetConnection());

                //код
                command2.Parameters.Add("@code", MySqlDbType.VarChar).Value = Code_textBox_panel3.Text;
                //название
                command2.Parameters.Add("@name", MySqlDbType.VarChar).Value = Name_textBox_panel3.Text;
                //ед. измерения
                command2.Parameters.Add("@unit", MySqlDbType.VarChar).Value = Unit_textBox_panel3.Text;
                //кол-во
                command2.Parameters.Add("@qt", MySqlDbType.VarChar).Value = Qt_textBox_panel3.Text;
                //дата
                command2.Parameters.Add("@date", MySqlDbType.VarChar).Value = dateTimePicker.Text;
                //цена
                command2.Parameters.Add("@price", MySqlDbType.Double).Value = Price_textBox_panel3.Text;

                db.OpenConnection();

                if (command2.ExecuteNonQuery() == 1) //проверка на корректность работы
                {
                    MessageBox.Show("Товар был изменен");

                    Code_textBox_panel3.Clear();
                    Name_textBox_panel3.Clear();
                    Unit_textBox_panel3.Clear();
                    Qt_textBox_panel3.Clear();
                    Price_textBox_panel3.Clear();
                }
                else
                    MessageBox.Show("Товар НЕ был изменен");

                db.CloseConnection(); //если не закрыть, будет большая нагрузка
            }
            else // если нет
            {
                MessageBox.Show("ТАКОГО ТОВАРА НЕТ ИЛИ ВЫ НИЧЕГО НЕ ИЗМЕНИЛИ");
            }


        }

        // УДАЛЕНИЕ 

        private void Delete_button_Click(object sender, EventArgs e)
        {
            DB db = new DB(); //выделяем память под объект 

            DataTable table = new DataTable(); //табличка, в которой можно работать 
           
            MySqlDataAdapter adapter = new MySqlDataAdapter(); //adapter позволяет выбрать данные из базы данных

            //задаем команды по нахождению цены
            MySqlCommand command = new MySqlCommand("SELECT `Code` FROM `things` WHERE `Code` = @cd ", db.GetConnection()); //@cd - заглушка 

            command.Parameters.Add("@cd", MySqlDbType.VarChar).Value = Code_textBox_panel3.Text;

            //заполняем table заданной sql командой
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0 && Code_textBox_panel3.Text != "") //если есть такой индекс
            {
                DialogResult result = MessageBox.Show("Вы точно хотите удалить товар?", "" , MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    //задаем команды по нахождению цены
                    MySqlCommand command2 = new MySqlCommand("DELETE FROM `things` WHERE `things`.`Code` = @cd", db.GetConnection()); //@cd - заглушка 

                    command2.Parameters.Add("@cd", MySqlDbType.VarChar).Value = Code_textBox_panel3.Text;

                    db.OpenConnection();

                    if (command2.ExecuteNonQuery() == 1) //проверка на корректность работы
                    {
                        MessageBox.Show("Товар был УДАЛЕН из базы данных"); //проверка на корректность работы

                        Code_textBox_panel3.Clear();
                    }
                    else
                        MessageBox.Show("Товар НЕ был удален из базы данных");


                    db.CloseConnection(); //если не закрыть, будет большая нагрузка
                }
            }
            else
                MessageBox.Show("УДАЛЯТЬ НЕЧЕГО");
        }
    }
}
