using RemObjects.InternetPack.Ftp.VirtualFtp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FTPServer.DesktopClient
{
    public class VmMain : INotifyPropertyChanged
    {
        private VirtualFtpServer ftpServer;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public VmMain()
        {
            this.hostIpCollection = new ObservableCollection<string>();
            this.directoryCollection = new ObservableCollection<DirectoryViewModel>();
            this.ftpServiceStatus = DesktopClient.FtpServiceStatus.Stopped;
            this.controlServiceCommand = new RelayCommand((arg) => this.ControlService());

            this.ftpServer = new VirtualFtpServer();
            this.ftpServer.Timeout = 60 * 1000; /* 1 minute */
            if (this.ftpServer.BindingV4 != null)
                this.ftpServer.BindingV4.ListenerThreadCount = 10;

            this.FtpServerName = "FTP Server";
        }

        public void Load()
        {
            try
            {
                var logicalDriveCollection = Environment.GetLogicalDrives();
                foreach (var logicalDrive in logicalDriveCollection)
                {

                    var directoryViewModel = new DirectoryViewModel
                    {
                        DirectoryName = logicalDrive,
                        PhysicalPath = logicalDrive,
                        DirectoryIconPath = "/FTPServer.DesktopClient;component/hdd_drive.png",
                        IsHidden = false,
                    };
                    var subDirectoryCount = Directory.GetDirectories(logicalDrive).Count();
                    if (subDirectoryCount > 0)
                        directoryViewModel.SubDirectoryCollection = new ObservableCollection<DirectoryViewModel>();
                    this.DirectoryCollection.Add(directoryViewModel);
                }
            }
            catch
            {
            }

            try
            {
                var hostName = Dns.GetHostName();
                var iphostentry = Dns.GetHostEntry(hostName);

                this.HostIpCollection.Add("127.0.0.1");
                foreach (IPAddress ipaddress in iphostentry.AddressList)
                {
                    if(ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    this.HostIpCollection.Add(ipaddress.ToString());
                }
                this.HostIP = this.HostIpCollection.FirstOrDefault();
                this.HostPort = 21;
            }
            catch
            {
                
            }
        }

        private ObservableCollection<string> hostIpCollection;
        public ObservableCollection<string> HostIpCollection
        {
            get
            {
                return this.hostIpCollection;
            }
            set
            {
                if (this.hostIpCollection == value)
                    return;

                this.hostIpCollection = value;
                this.OnPropertyChanged("HostIpCollection");
            }
        }

        private ObservableCollection<DirectoryViewModel> directoryCollection;
        public ObservableCollection<DirectoryViewModel> DirectoryCollection
        {
            get
            {
                return this.directoryCollection;
            }
            set
            {
                if (this.directoryCollection == value)
                    return;

                this.directoryCollection = value;
                this.OnPropertyChanged("DirectoryCollection");
            }
        }

        private DirectoryViewModel selectedDirectory;
        public DirectoryViewModel SelectedDirectory
        {
            get
            {
                return this.selectedDirectory;
            }
            set
            {
                if (this.selectedDirectory == value)
                    return;

                this.selectedDirectory = value;
                this.OnPropertyChanged("SelectedDirectory");
                this.OnPropertyChanged("SelectedDirectoryPath");
            }
        }

        public string SelectedDirectoryPath
        {
            get
            {
                if (this.SelectedDirectory == null)
                    return "";
                else
                    return this.selectedDirectory.PhysicalPath;
            }
        }

        private string hostIP;
        public string HostIP
        {
            get
            {
                return this.hostIP;
            }
            set
            {
                if (this.hostIP == value)
                    return;

                this.hostIP = value;
                this.OnPropertyChanged("HostIP");
            }
        }

        private int hostPort;
        public int HostPort
        {
            get
            {
                return this.hostPort;
            }
            set
            {
                if (this.hostPort == value)
                    return;

                this.hostPort = value;
                this.OnPropertyChanged("HostPort");
            }
        }

        private string ftpUsername;
        public string FtpUsername
        {
            get
            {
                return this.ftpUsername;
            }
            set
            {
                if (this.ftpUsername == value)
                    return;

                this.ftpUsername = value;
                this.OnPropertyChanged("FtpUsername");
            }
        }

        private string ftpPassword;
        public string FtpPassword
        {
            get
            {
                return this.ftpPassword;
            }
            set
            {
                if (this.ftpPassword == value)
                    return;

                this.ftpPassword = value;
                this.OnPropertyChanged("FtpPassword");
            }
        }

        private string ftpPathRoot;
        public string FtpPathRoot
        {
            get
            {
                return this.ftpPathRoot;
            }
            set
            {
                if (this.ftpPathRoot == value)
                    return;

                this.ftpPathRoot = value;
                this.OnPropertyChanged("FtpPathRoot");
            }
        }

        private string ftpServerName;
        public string FtpServerName
        {
            get
            {
                return this.ftpServerName;
            }
            set
            {
                if (this.ftpServerName == value)
                    return;

                this.ftpServerName = value;
                this.OnPropertyChanged("FtpServerName");
            }
        }

        private FtpServiceStatus ftpServiceStatus;
        public FtpServiceStatus FtpServiceStatus
        {
            get
            {
                return this.ftpServiceStatus;
            }
            set
            {
                if (this.ftpServiceStatus == value)
                    return;

                this.ftpServiceStatus = value;
                this.OnPropertyChanged("FtpServiceStatus");
                this.OnPropertyChanged("ControlButtonText");
                this.OnPropertyChanged("IsAlterationEnabled");
                this.OnPropertyChanged("ServerColor");
            }
        }

        public bool IsAlterationEnabled
        {
            get
            {
                return this.FtpServiceStatus == DesktopClient.FtpServiceStatus.Stopped;
            }
        }

        public string ServerColor
        {
            get
            {
                return this.FtpServiceStatus == DesktopClient.FtpServiceStatus.Started ? "Green" : "Red";
            }
        }

        private RelayCommand controlServiceCommand;
        public RelayCommand ControlServiceCommand
        {
            get
            {
                return this.controlServiceCommand;
            }
        }

        public string ControlButtonText
        {
            get
            {
                return this.FtpServiceStatus == DesktopClient.FtpServiceStatus.Started ? "Stop" : "Start";
            }
        }

        private void ControlService()
        {
            try
            {
                if (this.FtpServiceStatus == DesktopClient.FtpServiceStatus.Started)
                {
                    this.ftpServer.Close();

                    this.FtpServiceStatus = DesktopClient.FtpServiceStatus.Stopped;
                }
                else
                {
                    if (string.IsNullOrEmpty(this.HostIP))
                        MessageBox.Show("Host IP is required", "FTP Server", MessageBoxButton.OK, MessageBoxImage.Error);
                    else if (this.HostPort == default(int))
                        MessageBox.Show("Host Port is required", "FTP Server", MessageBoxButton.OK, MessageBoxImage.Error);
                    else if (string.IsNullOrEmpty(this.FtpUsername))
                        MessageBox.Show("FTP Username is required", "FTP Server", MessageBoxButton.OK, MessageBoxImage.Error);
                    else if (string.IsNullOrEmpty(this.FtpPassword))
                        MessageBox.Show("FTP Password is required", "FTP Server", MessageBoxButton.OK, MessageBoxImage.Error);
                    else if (this.SelectedDirectory == null)
                        MessageBox.Show("FTP Directory Root is required", "FTP Server", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        this.ftpServer.Port = this.HostPort;
                        this.ftpServer.ServerName = this.FtpServerName;

                        var userManager = new UserManager();
                        ((UserManager)userManager).AddUser(this.FtpUsername, this.FtpPassword);

                        this.ftpServer.UserManager = userManager;
                        this.ftpServer.RootFolder = new DiscFolder(null, "/", this.SelectedDirectoryPath);

                        this.ftpServer.Open();

                        this.FtpServiceStatus = DesktopClient.FtpServiceStatus.Started;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "FTP Server", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}