using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConferenceBoxWPF.Models
{
    public class ConferenceItem : NotificationBase
    {
        private int _id;
        private string _name;
        //private object _content;
        
        //public ConferenceItem(int id, string name/*, object content*/)
        //{
        //    _id = id;
        //    _name = name;
        //    //Content = content;
        //}

        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        //public object Content
        //{
        //    get { return _content; }
        //    set { SetProperty(ref _content, value); }
        //}
    }
}
