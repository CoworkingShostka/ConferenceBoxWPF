using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConferenceBoxWPF.Models
{
    public class UserListLoad : NotificationBase
    {

        private static ObservableCollection<Person> _people = new ObservableCollection<Person>();
        public ObservableCollection<Person> People
        {
            get { return _people; }
            set { SetProperty(ref _people, value); }
        }

        public void LoadPeople (string id)
        {
            People.Clear();

            string connStr = "server=shostka.mysql.ukraine.com.ua;user=shostka_conf;database=shostka_conf;port=3306;password=Cpu1234Pro;SslMode=None;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "SELECT users.*, conference_" + id + ".is_visited FROM conference_" + id + " LEFT JOIN users ON conference_" + id + ".user_id = users.id";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    //cmd.Parameters.AddWithValue("@id",_id);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            People.Add(new Person
                            {
                                Id = rdr.GetInt32("id"),
                                Firstname = rdr.GetString("firstname"),
                                Lastname = rdr.GetString("lastname"),
                                Email = rdr.GetString("email"),
                                Barcode = rdr.GetString("barcode"),
                                Notes = rdr.GetString("notes"),
                                IsVisited = rdr.GetInt32("is_visited")
                            });
                        }
                        rdr.Close();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
