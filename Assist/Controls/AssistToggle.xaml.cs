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

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistToggle.xaml
    /// </summary>
    public partial class AssistToggle : UserControl
    {
        public AssistToggle()
        {
            InitializeComponent();
        }



        public bool? IsCheck
        {
            get { return (bool?)GetValue(IsCheckProperty); }
            set { SetValue(IsCheckProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCheck.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckProperty =
            DependencyProperty.Register("IsCheck", typeof(bool?), typeof(AssistToggle));




        public bool Checked
        {
            get { return (bool)GetValue(CheckedProperty); }
            set { SetValue(CheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Checked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckedProperty =
            DependencyProperty.Register("Checked", typeof(bool), typeof(AssistToggle));


        private void toggle_Checked(object sender, RoutedEventArgs e)
        {
            this.Checked = true;
        }

        private void toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Checked = false;
        }
    }
}
