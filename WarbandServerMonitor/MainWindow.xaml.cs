using System;
using System.Net;
using System.Windows;
using WarbandServerMonitor.Model;

namespace WarbandServerMonitor
{
    public partial class MainWindow
    {
        public ServerList ServerList { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServerList = new ServerList("http://warbandmain.taleworlds.com/handlerservers.ashx?type=list");
            ServerList.StartMonitor();
            DataContext = ServerList;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ServerList.Dispose();
        }
    }
}
