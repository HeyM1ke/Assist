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
    /// Interaction logic for AssistFeaturedButton.xaml
    /// </summary>
    public partial class AssistFeaturedButton : UserControl
    {
        public AssistFeaturedButton()
        {
            InitializeComponent();
        }


        public Visibility isSelected
        {
            get { return (Visibility)GetValue(isSelectedProperty); }
            set { SetValue(isSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for isSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty isSelectedProperty =
            DependencyProperty.Register("isSelected", typeof(Visibility), typeof(AssistFeaturedControl));
    }
}
