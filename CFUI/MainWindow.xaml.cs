using CFUI.ViewModels;
using CFUI.Views;
using MahApps.Metro.Controls;
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

namespace CFUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            this.SetBinding(ShowNewPartnumWindowProperty, "ShowNewPartnumWindow");
        }

        public static AddPartnumWindow AddPartnumWindow = null;
        public bool ShowNewPartnumWindow
        {
            get { return (bool)GetValue(ShowNewPartnumWindowProperty); }
            set { SetValue(ShowNewPartnumWindowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowNewPartnumWindow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowNewPartnumWindowProperty =
            DependencyProperty.Register("ShowNewPartnumWindow", typeof(bool), typeof(MainWindow), new PropertyMetadata(new PropertyChangedCallback((d, e) =>
            {
                if (AddPartnumWindow != null)
                {
                    if (AddPartnumWindow.HasShow)
                        return;
                }
                var mMainWindow = d as MainWindow;
                AddPartnumWindow = new AddPartnumWindow();// { Owner = this }.Show();
                AddPartnumWindow.Owner = Application.Current.MainWindow;
                AddPartnumWindow.DataContext = mMainWindow.DataContext;
                AddPartnumWindow.SetBinding(AddPartnumWindow.QuitAddPartnumWindowProperty, "QuitAddPartnumWindow");
                AddPartnumWindow.HasShow = true;
                AddPartnumWindow.Show();
            })));


    }
}
