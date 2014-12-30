using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WarbandServerMonitor.View
{
    public partial class ServerList
    {
        private GridViewColumnHeader _listViewSortCol;
        private SortAdorner _listViewSortAdorner;

        public ServerList()
        {
            InitializeComponent();
        }

        private void ColumnHeaderOnClick(object sender, RoutedEventArgs e)
        {
            var column = (GridViewColumnHeader)sender;
            var sortBy = column.Tag.ToString();
            if (_listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(_listViewSortCol).Remove(_listViewSortAdorner);
                ServersListView.Items.SortDescriptions.Clear();
            }

            var newDir = ListSortDirection.Ascending;
            if (_listViewSortCol == column && _listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            _listViewSortCol = column;
            _listViewSortAdorner = new SortAdorner(_listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(_listViewSortCol).Add(_listViewSortAdorner);
            ServersListView.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }
    }
}
