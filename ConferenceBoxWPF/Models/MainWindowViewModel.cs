using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceBoxWPF.Models
{
    /// <summary>
    /// Controls Conference List and users load
    /// </summary>
    public class MainWindowViewModel : NotificationBase
    {
        public MainWindowViewModel()
        {
            ConferenceListLoadAsync();
        }

        private string _TitleText = "ConferenceBox";
        public string TitleText
        {
            get { return _TitleText; }
            set { SetProperty(ref _TitleText, value); }
        }


        public async Task ConferenceListLoadAsync()
        {

        }
    }
}
