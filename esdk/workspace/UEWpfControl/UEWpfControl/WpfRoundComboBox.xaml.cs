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

namespace UEWpfControl
{
    /// <summary>
    /// WpfComboBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WpfRoundComboBox : UserControl
    {
        public ComboBox customComboBox = null;
        public WpfRoundComboBox()
        {
            InitializeComponent();
            customComboBox = cb;            
        }
        
        public void SetSize(int width, int height)
        {
            cb.Width = width;
            cb.Height = height;
        }
        
        private void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
