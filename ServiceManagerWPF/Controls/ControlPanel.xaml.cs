using System;
using System.Collections.Generic;
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

namespace ServiceManagerWPF.Controls
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : UserControl
    {
        public event EventHandler StartClicked = delegate { };
        public event EventHandler StopClicked = delegate { };
        public event EventHandler PauseClicked = delegate { };
        public event EventHandler RefreshClicked = delegate { };
        public event EventHandler ConfigClicked = delegate { };

        public ControlPanel()
        {
            InitializeComponent();

            _startButton.Click += (s, a) => StartClicked.Invoke(s, a);
            _stopButton.Click += (s, a) => StopClicked.Invoke(s, a);
            _pauseButton.Click += (s, a) => PauseClicked.Invoke(s, a);
            _refreshButton.Click += (s, a) => RefreshClicked.Invoke(s, a);
            _configButton.Click += (s, a) => ConfigClicked.Invoke(s, a);
        }
    }
}
