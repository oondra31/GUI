using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace minesweeper
{
    public partial class EndWindow : Window
    {
        public bool result { get; private set; } = false;
        public EndWindow()
        {
            InitializeComponent();
        }

        private void ClickYes(object sender, RoutedEventArgs e)
        {
            this.Close();           
            result = true;
        }

        private void ClickNO(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
