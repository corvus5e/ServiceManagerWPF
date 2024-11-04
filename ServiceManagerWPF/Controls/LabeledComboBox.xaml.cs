using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace ServiceManagerWPF.Controls
{
    /// <summary>
    /// Interaction logic for LabeledComboBox.xaml
    /// </summary>
    public partial class LabeledComboBox : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(LabeledComboBox), new UIPropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register("SelectedItem", typeof(object), typeof(LabeledComboBox), new UIPropertyMetadata(null));

        public IEnumerable ItemsSource
        { 
            get => _comboBox.ItemsSource;
            set => _comboBox.ItemsSource = value;
        }

        public object SelectedItem
        {
            get => _comboBox.SelectedItem;
            set => _comboBox.SelectedItem = value;
        }

        public string? Title
        {
            get => _titleLabel.Content.ToString();
            set => _titleLabel.Content = value;
        }

        public event SelectionChangedEventHandler SelectionChanged
        {
            add { _comboBox.SelectionChanged += value; }
            remove { _comboBox.SelectionChanged -= value; }
        }

        public LabeledComboBox()
        {
            InitializeComponent();
        }
    }
}
