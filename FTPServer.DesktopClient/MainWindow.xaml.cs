using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using Telerik.Windows.Controls;

namespace FTPServer.DesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VmMain vmMain;

        public MainWindow()
        {
            InitializeComponent();
            this.vmMain = this.DataContext as VmMain;
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (this.vmMain.FtpServiceStatus == FtpServiceStatus.Started)
                MessageBox.Show("Please stop the server to close the application", "FTP Server", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.vmMain.Load();
        }

        private void directoryBrowserTreeView_LoadOnDemand(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var radTreeViewItem = e.OriginalSource as RadTreeViewItem;
            var directoryViewModel = radTreeViewItem.DataContext as DirectoryViewModel;
            try
            {
                var subDirectoryCollection = Directory.GetDirectories(directoryViewModel.PhysicalPath);
                if (!subDirectoryCollection.Any())
                    radTreeViewItem.IsLoadOnDemandEnabled = false;
                else
                {
                    foreach (var subDirectory in subDirectoryCollection)
                    {
                        var directoryInfo = new DirectoryInfo(subDirectory);
                        var subDirectoryViewModel = new DirectoryViewModel
                        {
                            DirectoryName = System.IO.Path.GetFileName(subDirectory),
                            PhysicalPath = subDirectory,
                            DirectoryIconPath = "/FTPServer.DesktopClient;component/folder.png",
                            SubDirectoryCollection = null,
                            IsHidden = (directoryInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden
                        };

                        try
                        {
                            var subDirectoryCount = Directory.GetDirectories(subDirectory).Count();
                            if (subDirectoryCount > 0)
                                subDirectoryViewModel.SubDirectoryCollection = new ObservableCollection<DirectoryViewModel>();
                        }
                        catch
                        {
                        }

                        directoryViewModel.SubDirectoryCollection.Add(subDirectoryViewModel);
                    }
                }
            }
            catch
            {
                radTreeViewItem.IsLoadOnDemandEnabled = false;
            }
        }
    }
}
