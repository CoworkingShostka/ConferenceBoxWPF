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
    /// <summary>
    /// Controls Conference List and users load
    /// </summary>
    public class MainWindowViewModel : NotificationBase
    {
        public MainWindowViewModel()
        {
            //conferenceList.Clear();
            ConferenceListLoad();
        }

        private string _TitleText = "ConferenceBox";
        public string TitleText
        {
            get { return _TitleText; }
            set { SetProperty(ref _TitleText, value); }
        }

        private ObservableCollection<ConferenceItem> _conferenceList = new ObservableCollection<ConferenceItem>();
        public ObservableCollection<ConferenceItem> conferenceList
        {
            get { return _conferenceList; }
            set { SetProperty(ref _conferenceList, value); }
        }

        string connStr = "server=shostka.mysql.ukraine.com.ua;user=shostka_conf;database=shostka_conf;port=3306;password=Cpu1234Pro;";

        public void ConferenceListLoad()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "SELECT * FROM conferences ORDER BY id DESC";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            conferenceList.Add(new ConferenceItem
                            {
                                Id = rdr.GetInt32("id"),
                                Name = rdr.GetString("name")
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
