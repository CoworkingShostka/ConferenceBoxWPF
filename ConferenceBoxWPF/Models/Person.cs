using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceBoxWPF.Models
{
    /// <summary>
    /// Base class to describe conference user
    /// </summary>
    public class Person : NotificationBase
    {
        private int _id;
        private string _firstname;
        private string _lastname;
        private string _email;
        private string _barcode;
        private string _notes;
        private int _isVisited;
        private string _colorMode = "PrimaryLight";

        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        public string Firstname
        {
            get { return _firstname; }
            set { SetProperty(ref _firstname, value); }
        }
        public string Lastname
        {
            get { return _lastname; }
            set { SetProperty(ref _lastname, value); }
        }
        public string Barcode
        {
            get { return _barcode; }
            set { SetProperty(ref _barcode, value); }
        }
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }
        public string Notes
        {
            get { return _notes; }
            set { SetProperty(ref _notes, value); }
        }
        public int IsVisited
        {
            get { return _isVisited; }
            set { SetProperty(ref _isVisited, value); }
        }
        public string ColorMode
        {
            get { return _colorMode; }
            set { SetProperty(ref _colorMode, value); }
        }
    }
}
