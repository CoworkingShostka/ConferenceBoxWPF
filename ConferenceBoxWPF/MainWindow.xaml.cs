using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ConferenceBoxWPF.Models;
using System.Windows.Controls.Primitives;
using MaterialDesignThemes.Wpf;

namespace ConferenceBoxWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Current;   //object to get access to UI elements
                
        public MainWindow()
        {
            InitializeComponent();
            Current = this;

            viewModel = new MainWindowViewModel(MySnackbar.MessageQueue);

            Closing += viewModel.MainWindowClosing; //extend standart Closing method

            DataContext = viewModel;
        }

        MainWindowViewModel viewModel { get; set; }

    }
}
