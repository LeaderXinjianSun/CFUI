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
using System.Windows.Shapes;

namespace CFUI.Views
{
    /// <summary>
    /// AddPartnumWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddPartnumWindow : Window
    {
        public AddPartnumWindow()
        {
            InitializeComponent();
        }


        public bool QuitAddPartnumWindow
        {
            get { return (bool)GetValue(QuitAddPartnumWindowProperty); }
            set { SetValue(QuitAddPartnumWindowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QuitAddPartnumWindow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuitAddPartnumWindowProperty =
            DependencyProperty.Register("QuitAddPartnumWindow", typeof(bool), typeof(AddPartnumWindow), new PropertyMetadata(new PropertyChangedCallback((d, e) =>
            {
                var mAddPartnumWindow = d as AddPartnumWindow;
                if (mAddPartnumWindow.HasShow)
                {
                    mAddPartnumWindow.HasShow = false;
                    mAddPartnumWindow.Close();
                    mAddPartnumWindow = null;
                }
            })));


        public bool HasShow { get; set; }
        protected override void OnClosed(EventArgs e)
        {
            HasShow = false;
            base.OnClosed(e);
        }
    }
}
