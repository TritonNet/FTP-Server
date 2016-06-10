using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPServer.DesktopClient
{
    public class DirectoryViewModel : INotifyPropertyChanged
    {
        public string DirectoryName { get; set; }

        public string PhysicalPath { get; set; }

        public string DirectoryIconPath { get; set; }

        public bool IsHidden { get; set; }

        public double Opacity
        {
            get
            {
                return this.IsHidden ? 0.5 : 1;
            }
        }

        private ObservableCollection<DirectoryViewModel> subDirectoryCollection;
        public ObservableCollection<DirectoryViewModel> SubDirectoryCollection
        {
            get
            {
                return this.subDirectoryCollection;
            }
            set
            {
                if (this.subDirectoryCollection == value)
                    return;

                this.subDirectoryCollection = value;
                this.OnPropertyChanged("SubDirectoryCollection");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
