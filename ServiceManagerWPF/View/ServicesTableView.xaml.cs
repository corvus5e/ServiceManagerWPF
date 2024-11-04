using ServiceManagerWPF.Model;
using System.Collections;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace ServiceManagerWPF.View
{
    /// <summary>
    /// Interaction logic for ServicesTableView.xaml
    /// </summary>
    public partial class ServicesTableView : UserControl
    {
        public ServicesTableView()
        {
            InitializeComponent();
            //TODO: This class should be created manually in code in MainWindows with viewModel as a parameter
        }

        public IList SelectedItems
        {
            private set { }
            get
            {
                return _servicesDataGrid.SelectedItems;
            }
        }

        public void FilterView(ICollectionView view)
        {
            _servicesDataGrid.ItemsSource = view;
            view.Refresh();
        }
    }
}
