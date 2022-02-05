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
    /// Interaction logic for AssistSocialButton.xaml
    /// </summary>
    public partial class AssistSocialButton : UserControl
    {
        public AssistSocialButton()
        {
            InitializeComponent();
        }



        public string Pathdata
        {
            get { return (string)GetValue(PathdataProperty); }
            set { SetValue(PathdataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Pathdata.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathdataProperty =
            DependencyProperty.Register("Pathdata", typeof(string), typeof(AssistSocialButton));


    }
}
