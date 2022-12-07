using System;
using System.Data.SqlClient;
//using System.Diagnostics;
using System.Linq;

namespace Pharmacy_test_task
{
    class Program
    {
        public static void Help() //команды
        {
            Console.WriteLine("/товар - Создание товара с названием");
            Console.WriteLine("/аптека - Создание аптеки с названием, адресом и телефоном");
            Console.WriteLine("/склад - Создание склада, привязанного к аптеке, с названием");
            Console.WriteLine("/партия - Создание партии товара, поступающего на склад, с количеством");
            Console.WriteLine("/удалить товар - Удалить товар по названию или коду и партии этого товара");
            Console.WriteLine("/удалить аптеку - Удалить аптеку по названию или коду и все склады и партии этой аптеки");
            Console.WriteLine("/удалить склад - Удалить склад по названию или коду и все партии этого склада");
            Console.WriteLine("/удалить партию - Удалить партию");
            Console.WriteLine("/товары - Показать товары и их количество в выбранной аптеке");

        }
        public static string connection_string = @"Data Source=SqlServer;Initial Catalog = Pharmacy_data_base; Integrated Security = True"; //строка подключения
        public static SqlConnection con = new SqlConnection(connection_string); //используется SQLServer 2022, язык запросов - SQL
        public static SqlCommand com = new SqlCommand(); //для команд
        public static string local_sql = string.Empty; //для текста команд
        //функции для формирования текста SQL-запроса. В каждом Debug для отладки
        public static string Delete_Sql(string table_name, int code) //удалить по коду
        {
            string sql = $"Delete From {table_name} Where {table_name}.[Id] = '{code}'";
            //Debug.WriteLine($"Сформировал: {sql}");
            return sql;
        }
        public static string Delete_Sql(string table_name, string column_name, string name) //удалить по названию
        {
            string sql = $"Delete From {table_name} Where [{column_name}] = '{name}'";
            //Debug.WriteLine($"Сформировал: {sql}");
            return sql;
        }
        public static string Insert_Product_Sql(string product_name) //добавить товар
        {
            string sql = $"Insert into Product ([Product_name]) values ('{product_name}')";
            //Debug.WriteLine($"Сформировал: {sql}");
            return sql;
        }
        public static string Insert_Pharmacy_Sql(string pharmacy_name, string pharmacy_adress, string pharmacy_phone) //добавить аптеку
        {
            string sql = $"Insert into Pharmacy ([Pharmacy_name], [Pharmacy_adress], [Pharmacy_phone]) " +
                         $"Values ('{pharmacy_name}', '{pharmacy_adress}', '{pharmacy_phone}')";
            //Debug.WriteLine($"Сформировал: {sql}");
            return sql;
        }
        public static string Insert_Shipment_Sql(int id_product, int id_storehouse, int quantity) //добавить партию
        {
            string sql = $"Insert into Shipment ([Id_Product], [Id_Storehouse], [Quantity_in_shipment]) " +
                         $"Values ('{id_product}', '{id_storehouse}', '{quantity}')";
            //Debug.WriteLine($"Сформировал: {sql}");
            return sql;
        }
        public static string Insert_Storehouse_Sql(int id_pharmacy, string stotehouse_name) //добавить склад
        {
            string sql = $"Insert into Shipment ([Id_pharmacy], [Stotehouse_name]) " +
                         $"Values ('{id_pharmacy}', '{stotehouse_name}')";
            //Debug.WriteLine($"Сформировал: {sql}");
            return sql;
        }
        public static string Select_data_sql(int id_pharmacy) //получить товары по коду аптеки
        {
            string sql = $"Select Product.[Product_name], Shipment.[Quantity_in_shipment], Pharmacy.[Pharmacy_name] " +
                         $"From Product " +
                         $"Inner Join Shipment on Product.[Id] = Shipment.[Id_Product] " +
                         $"Inner Join Storehouse on Shipment.[Id_Storehouse] = Storehouse.[Id] " +
                         $"Inner Join Pharmacy on Storehouse.[Id_pharmacy] = Pharmacy.[Id] " +
                         $"Where Pharmacy.[Id] = '{id_pharmacy}';";
            //Debug.WriteLine($"Сформировал: {sql}");
            return sql;
        }
        public static string Select_data_sql(string pharmacy_name) //получить товары по названию аптеки
        {
            string sql = $"Select Product.[Product_name], Shipment.[Quantity_in_shipment], Pharmacy.[Pharmacy_name] " +
                         $"From Product " +
                         $"Inner Join Shipment on Product.[Id] = Shipment.[Id_Product] " +
                         $"Inner Join Storehouse on Shipment.[Id_Storehouse] = Storehouse.[Id] " +
                         $"Inner Join Pharmacy on Storehouse.[Id_pharmacy] = Pharmacy.[Id] " +
                         $"Where Pharmacy.[Pharmacy_name] = '{pharmacy_name}';";
            //Debug.WriteLine($"Сформировал: {sql}");
            return sql;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать!");
            bool program = true;
            while (program) //механизм повторных запросов
            {
                Console.WriteLine("Введите /помощь для вывода команд или введите команду:");
                switch (Console.ReadLine().ToLower())
                {
                    case "/помощь": Help();
                        break;
                    case "/выход":
                        {
                            Console.WriteLine("Вы точно хотите выйти? Да - для выхода");
                            if (Console.ReadLine().ToLower() == "да")
                            {
                                program = false;
                            }
                        }
                        break;
                    case "/товар":
                        {
                            Console.WriteLine("Введите название товара или '/' для выхода");
                            Console.Write("Название: ");
                            string name = Console.ReadLine();
                            if (name == "/")
                            {
                                Console.WriteLine("Выхожу");
                            }
                            else
                            {
                                local_sql = Insert_Product_Sql(name);
                                con.Open();
                                com = new SqlCommand(local_sql, con);
                                com.ExecuteNonQuery();
                                Console.WriteLine($"Добвлен товар: {name}");
                                con.Close();
                            }
                        }
                        break;
                    case "/аптека":
                        {
                            Console.WriteLine("Введите название аптеки, адрес и телефон или '/' для выхода");
                            Console.Write("Название: ");
                            string name = Console.ReadLine();
                            if (name == "/")
                            {
                                Console.WriteLine("Выхожу");
                            }
                            else
                            {
                                Console.Write("Адрес: ");
                                string adress = Console.ReadLine();
                                if (adress == "/")
                                {
                                    Console.WriteLine("Выхожу");
                                }
                                else
                                {
                                    Console.Write("Телефон: ");
                                    string phone = Console.ReadLine();
                                    if (phone == "/")
                                    {
                                        Console.WriteLine("Выхожу");
                                    }
                                    else
                                    {
                                        local_sql = Insert_Pharmacy_Sql(name, adress, phone);
                                        con.Open();
                                        com = new SqlCommand(local_sql, con);
                                        com.ExecuteNonQuery();
                                        Console.WriteLine($"Добвлена аптека: {name}");
                                        con.Close();
                                    }
                                }
                            }
                        }
                        break;
                    case "/склад":
                        {
                            Console.WriteLine("Введите код аптеки и название или '/' для выхода");
                            Console.Write("Код аптеки: ");
                            string local = Console.ReadLine();
                            if (local == "/")
                            {
                                Console.WriteLine("Выхожу");
                            }
                            else
                            {
                                if (!int.TryParse(local, out int pharmacy_code))
                                {
                                    Console.WriteLine("Введена не цифра");
                                }
                                else
                                {
                                    {
                                        Console.Write("Название: ");
                                        string name = Console.ReadLine();
                                        if (name == "/")
                                        {
                                            Console.WriteLine("Выхожу");
                                        }
                                        else
                                        {
                                            local_sql = Insert_Storehouse_Sql(pharmacy_code, name);
                                            con.Open();
                                            com = new SqlCommand(local_sql, con);
                                            com.ExecuteNonQuery();
                                            Console.WriteLine($"Добвлен склад: {name}");
                                            con.Close();
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "/партия":
                        {
                            Console.WriteLine("Введите код товара, код склада и количество или '/' для выхода");
                            Console.Write("Код товара: ");
                            string local = Console.ReadLine();
                            if (local == "/")
                            {
                                Console.WriteLine("Выхожу");
                            }
                            else
                            {
                                if (!int.TryParse(local, out int product_code))
                                {
                                    Console.WriteLine("Введена не цифра");
                                }
                                else
                                {
                                    {
                                        Console.Write("Код склада: ");
                                        local = Console.ReadLine();
                                        if (local == "/")
                                        {
                                            Console.WriteLine("Выхожу");
                                        }
                                        else
                                        {
                                            if (!int.TryParse(local, out int storehouse_code))
                                            {
                                                Console.WriteLine("Введена не цифра");
                                            }
                                            else
                                            {
                                                Console.Write("Количество: ");
                                                local = Console.ReadLine();
                                                if (local == "/")
                                                {
                                                    Console.WriteLine("Выхожу");
                                                }
                                                else
                                                {
                                                    if (!int.TryParse(local, out int quantity))
                                                    {
                                                        Console.WriteLine("Введена не цифра");
                                                    }
                                                    else
                                                    {
                                                        local_sql = Insert_Shipment_Sql(product_code, storehouse_code, quantity);
                                                        con.Open();
                                                        com = new SqlCommand(local_sql, con);
                                                        com.ExecuteNonQuery();
                                                        Console.WriteLine($"Добвлена партия товара {product_code} на склад {storehouse_code} в количестве {quantity}");
                                                        con.Close();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "/удалить товар":
                        {
                            Console.WriteLine("Введите код товара или его название или '/' для выхода");
                            Console.Write("Код товара или название: ");
                            string local = Console.ReadLine();
                            if (local == "/")
                            {
                                Console.WriteLine("Выхожу");
                            }
                            else
                            {
                                if (int.TryParse(local, out int code))
                                {
                                    local_sql = Delete_Sql("Product", code);
                                    con.Open();
                                    com = new SqlCommand(local_sql, con);
                                    com.ExecuteNonQuery();
                                    Console.WriteLine($"Удалён товар под кодом {code}");
                                    con.Close();
                                }
                                else
                                {
                                    local_sql = Delete_Sql("Product", "Product_name", local);
                                    con.Open();
                                    com = new SqlCommand(local_sql, con);
                                    com.ExecuteNonQuery();
                                    Console.WriteLine($"Удалён товар {local}");
                                    con.Close();
                                }
                            }
                        }
                        break;
                    case "/удалить аптеку":
                        {
                            Console.WriteLine("Введите код аптеки или её название или '/' для выхода");
                            Console.Write("Код аптеки или название: ");
                            string local = Console.ReadLine();
                            if (local == "/")
                            {
                                Console.WriteLine("Выхожу");
                            }
                            else
                            {
                                if (int.TryParse(local, out int code))
                                {
                                    local_sql = Delete_Sql("Pharmacy", code);
                                    con.Open();
                                    com = new SqlCommand(local_sql, con);
                                    com.ExecuteNonQuery();
                                    Console.WriteLine($"Удалёна аптека под кодом {code}");
                                    con.Close();
                                }
                                else
                                {
                                    local_sql = Delete_Sql("Pharmacy", "Pharmacy_name", local);
                                    con.Open();
                                    com = new SqlCommand(local_sql, con);
                                    com.ExecuteNonQuery();
                                    Console.WriteLine($"Удалёна аптека {local}");
                                    con.Close();
                                }
                            }
                        }
                        break;
                    case "/удалить склад":
                        {
                            Console.WriteLine("Введите код склада или его название или '/' для выхода");
                            Console.Write("Код склада или название: ");
                            string local = Console.ReadLine();
                            if (local == "/")
                            {
                                Console.WriteLine("Выхожу");
                            }
                            else
                            {
                                if (!int.TryParse(local, out int code))
                                {
                                    local_sql = Delete_Sql("Storehouse", "Storehouse_name", local);
                                    con.Open();
                                    com = new SqlCommand(local_sql, con);
                                    com.ExecuteNonQuery();
                                    Console.WriteLine($"Удалён склад {local}");
                                    con.Close();
                                }
                                else
                                {
                                    local_sql = Delete_Sql("Storehouse", code);
                                    con.Open();
                                    com = new SqlCommand(local_sql, con);
                                    com.ExecuteNonQuery();
                                    Console.WriteLine($"Удалён склад под кодом {code}");
                                    con.Close();
                                }
                            }
                        }
                        break;
                    case "/удалить партию":
                        {
                            Console.WriteLine("Введите код партии или '/' для выхода");
                            Console.Write("Код партии: ");
                            string local = Console.ReadLine();
                            if (local == "/")
                            {
                                Console.WriteLine("Выхожу");
                            }
                            else
                            {
                                if (!int.TryParse(local, out int code))
                                {
                                    Console.WriteLine("Введена не цифра");
                                }
                                else
                                {
                                    local_sql = Delete_Sql("Shipment", code);
                                    con.Open();
                                    com = new SqlCommand(local_sql, con);
                                    com.ExecuteNonQuery();
                                    Console.WriteLine($"Удалена партия под кодом {code}");
                                    con.Close();
                                }
                            }
                        }
                        break;
                    case "/товары":
                        {
                            Console.WriteLine("Введите код аптеки или название или '/' для выхода");
                            Console.Write("Код аптеки или название: ");
                            SqlDataReader reader;
                            string local = Console.ReadLine();

                            if (local == "/")
                            {
                                Console.WriteLine("Выхожу");
                            }
                            else
                            {
                                if (!int.TryParse(local, out int code))
                                {
                                    local_sql = Select_data_sql(local);
                                }
                                else
                                {
                                    local_sql = Select_data_sql(code);
                                }
                                con.Open();
                                com = new SqlCommand(local_sql, con);
                                com = new SqlCommand(local_sql, con);
                                reader = com.ExecuteReader();
                                Console.WriteLine(String.Format("|{0,20}|{1,20}|{2,20}|", "Название товара", "Количество", "Аптека"));
                                Console.WriteLine(string.Concat(Enumerable.Repeat("_", 64)));
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        Console.WriteLine(String.Format("|{0,20}|{1,20}|{2,20}|", reader[0], reader[1], reader[2]));
                                        //Console.WriteLine("{reader[0]} |\t{reader[1]} |\t{reader[2]}");
                                    }
                                    reader.Close();
                                }
                                con.Close();
                            }
                        }
                        break;
                }
            }
        }
    }
}
