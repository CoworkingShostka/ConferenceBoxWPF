using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.Practices.Prism.Commands;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ZXing;
using System.ComponentModel;
using MaterialDesignThemes.Wpf;

namespace ConferenceBoxWPF.Models
{
    /// <summary>
    /// Controls Conference List and users load
    /// </summary>
    public class MainWindowViewModel : NotificationBase
    {
        public struct Device
        {
            public int Index;
            public string Name;
            public override string ToString()
            {
                return Name;
            }
        }

        VideoCaptureDevice LocalWebCam; //object for camera controll
        public FilterInfoCollection LocalWebCamsCollection; //list of awaliable cameras
        private Bitmap currentBitmapForDecoding;    //Image send to barcode decoder
        private readonly Thread decodingThread; //Thread to decode imgs
        private Result currentResult;   //decoding result
        //private readonly System.Drawing.Pen resultRectPen;
        static ManualResetEvent CameraEvent = new ManualResetEvent(false);  //event to controll decodingThread 
        bool CameraStarted = false; //flag shows that camera statrted
        int _conferenceID;  //current conference ID

        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            ConferenceListLoad();   //load conference list from DB
            ConferenceLoadCommand = new DelegateCommand<ConferenceItem>(x => LoadConference(_selectedItem.Id)); //load user from chosen conference

            userList = new UserListControll(snackbarMessageQueue);  //object to 

            MainWindow_Loaded();
            decodingThread = new Thread(DecodeBarcode);
            decodingThread.IsBackground = true;
            decodingThread.Start();

        }

        public ICommand ConferenceLoadCommand { get; set; }
        public UserListControll userList { get; set; }

        private ConferenceItem _selectedItem;
        public ConferenceItem SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        private string _TitleText = "Виберіть конфернцію";
        public string TitleText
        {
            get { return _TitleText; }
            set { SetProperty(ref _TitleText, value); }
        }

        private Device _menuComboBoxSelectedItem;
        public Device MenuComboBoxSelectedItem
        {
            get { return _menuComboBoxSelectedItem; }
            set
            {
                SetProperty(ref _menuComboBoxSelectedItem, value);
                if (!_menuComboBoxSelectedItem.Equals(null))
                {
                    CameraButtonEnable = true;
                    //ConferenceBox.Properties.Settings.Default.CameraIndx = _menuComboBoxSelectedItem.Index;
                }
            }
        }

        private bool _cameraButtonEnable;
        public bool CameraButtonEnable
        {
            get { return _cameraButtonEnable; }
            set { SetProperty(ref _cameraButtonEnable, value); }
        }

        //private Person _userItemForEdit;
        //public Person UserItemToEdit
        //{
        //    get { return _userItemForEdit; }
        //    set { SetProperty(ref _userItemForEdit, value); }
        //}

        private ObservableCollection<ConferenceItem> _conferenceList = new ObservableCollection<ConferenceItem>();
        public ObservableCollection<ConferenceItem> conferenceList
        {
            get { return _conferenceList; }
            set { SetProperty(ref _conferenceList, value); }
        }

        RelayCommand _cameraOpenCommand;
        public ICommand CameraOpenCommand
        {
            get
            {
                if (_cameraOpenCommand == null)
                {
                    _cameraOpenCommand = new RelayCommand(param => this.CameraOpen());
                }
                return _cameraOpenCommand;
            }
        }

        RelayCommand _cameraCloseCommand;
        public ICommand CameraCloseCommand
        {
            get
            {
                if (_cameraCloseCommand == null)
                {
                    _cameraCloseCommand = new RelayCommand(param => this.CameraClose());
                }
                return _cameraCloseCommand;
            }
        }

        RelayCommand _userEditCommand;
        public ICommand UserEditCommand
        {
            get
            {
                if (_userEditCommand == null)
                {
                    _userEditCommand = new RelayCommand((param) => this.UserEdit((Person)param));
                }
                return _userEditCommand;
            }
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

        public void LoadConference(int id)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            _conferenceID = id;

            userList.LoadPeople(id.ToString());

            TitleText = conferenceList.FirstOrDefault(i => i.Id == id).Name;

            MainWindow.Current.MenuToggleButton.IsChecked = false;
            //CameraCheck();
            MainWindow.Current.MenuPopupBox.IsPopupOpen = true;
        }

        void MainWindow_Loaded()
        {
            LocalWebCamsCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            for (int ind = 0; ind < LocalWebCamsCollection.Count; ind++)
            {
                MainWindow.Current.MenuComboBox.Items.Add(new Device
                {
                    Index = ind,
                    Name = LocalWebCamsCollection[ind].Name
                });
            }

        }

        //void CameraCheck()
        //{
        //    if (ConferenceBox.Properties.Settings.Default.CameraIndx == 999)
        //    {
        //        MainWindow.Current.MenuPopupBox.IsPopupOpen = true;
        //        return;
        //    }
        //    else { MainWindow.Current.MenuComboBox.SelectedIndex = ConferenceBox.Properties.Settings.Default.CameraIndx;
        //    }
        //}

        public void CameraOpen()
        {
            try
            {
                MainWindow.Current.MainDrawer.IsRightDrawerOpen = true;

                LocalWebCam = new VideoCaptureDevice(LocalWebCamsCollection[MenuComboBoxSelectedItem.Index].MonikerString);

                LocalWebCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);
                LocalWebCam.Start();

                CameraEvent.Set();
                //decodingThread.Start();
                if (!CameraStarted) CameraStarted = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if (currentBitmapForDecoding == null)
                {
                    currentBitmapForDecoding = (Bitmap)eventArgs.Frame.Clone();
                }

                BitmapImage bi;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    bi = new BitmapImage();
                    bi.BeginInit();
                    MemoryStream ms = new MemoryStream();
                    bitmap.Save(ms, ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                }
                bi.Freeze();
                MainWindow.Current.Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    MainWindow.Current.FrameHolder.Source = bi;
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DecodeBarcode()
        {
            var reader = new BarcodeReader();
            while (true)
            {
                CameraEvent.WaitOne();

                if (currentBitmapForDecoding != null)
                {
                    var result = reader.Decode(currentBitmapForDecoding);
                    if (result != null)
                    {
                        userList.MarkPerson(result.Text, _conferenceID);
                        MainWindow.Current.Dispatcher.BeginInvoke(new Action<Result>(ShowResult), result);
                        Thread.Sleep(650);
                        MainWindow.Current.Dispatcher.BeginInvoke(new Action(ClearResult));
                    }
                    currentBitmapForDecoding.Dispose();
                    currentBitmapForDecoding = null;
                    Thread.Sleep(650);

                }
                Thread.Sleep(200);
            }
        }

        private void ShowResult(Result result)
        {
            currentResult = result;
            MainWindow.Current.BarcodeTextBlock.Text = result.Text;
        }

        private void ClearResult()
        {
            MainWindow.Current.BarcodeTextBlock.Text = "";
        }

        public void CameraClose()
        {
            CameraEvent.Reset();
            LocalWebCam.Stop();

            MainWindow.Current.BarcodeTextBlock.Text = null;
            MainWindow.Current.MainDrawer.IsRightDrawerOpen = false;
        }

        public void MainWindowClosing(object sender, CancelEventArgs e)
        {
            if (CameraStarted)
            {
                LocalWebCam.NewFrame -= new NewFrameEventHandler(Cam_NewFrame);
                Dispatcher.CurrentDispatcher.InvokeShutdown();
                LocalWebCam.SignalToStop();
                LocalWebCam.WaitForStop();
            }
        }

        public void UserEdit(Person person)
        {
            //Flipper flipper = (Flipper)item;
            //Person person = (Person)item;
            if (person != null)
            {
                userList.EditUser(person, _conferenceID);
            }

            //flipper.IsFlipped = false;
        }
    }
}
