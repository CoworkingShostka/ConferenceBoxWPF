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
    public class UserListControll : NotificationBase
    {

        private static ObservableCollection<Person> _people = new ObservableCollection<Person>();
        public ObservableCollection<Person> People
        {
            get { return _people; }
            set { SetProperty(ref _people, value); }
        }

        string connStr = "server=shostka.mysql.ukraine.com.ua;user=shostka_conf;database=shostka_conf;port=3306;password=Cpu1234Pro;SslMode=None;";

        public void LoadPeople (string id)
        {
            People.Clear();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "SELECT users.*, conference_" + id + ".is_visited FROM conference_" + id + " LEFT JOIN users ON conference_" + id + ".user_id = users.id";
                    //string sql = "SELECT users.*, @confVisited FROM @conf LEFT JOIN users ON @confUserID = users.id";
                    //string sql = "SELECT users.*, @confVisited FROM conference_" + id + " LEFT JOIN users ON conference_" + id + ".user_id = users.id";
                    //string sql = "SELECT users.*, conference_@id.is_visited FROM conference_@id LEFT JOIN users ON conference_@id.user_id = users.id";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    //cmd.Parameters.AddWithValue("@id", id);
                    //cmd.Parameters.Add("@confVisited", MySqlDbType.String).Value = "conference_" + id + ".is_visited";
                    //cmd.Parameters.AddWithValue("@conf", "conference_"+id);
                    //cmd.Parameters.AddWithValue("@confVisited", "conference_" + id + ".is_visited");
                    //cmd.Parameters.AddWithValue("@confUserID", "conference_" + id + ".user_id");

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
                                //ColorMode = "PrimaryLight"
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

        public void MarkPerson(string barcode, int conferenceID)
        {
            try
            {
                var item = People.FirstOrDefault(b => b.Barcode == barcode);

                if (item != null)
                {
                    item.IsVisited = 1;
                    item.ColorMode = "Accent";

                    using (MySqlConnection conn = new MySqlConnection(connStr))
                    {
                        conn.Open();

                        string sql = "UPDATE conference_" + conferenceID + " SET is_visited = 1 WHERE user_id =" + item.Id;
                        MySqlCommand cmd = new MySqlCommand(sql, conn);

                        cmd.ExecuteNonQuery();
                        conn.Clone();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
