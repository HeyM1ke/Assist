using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistNavButton.xaml
    /// </summary>
    public partial class AssistNavButton : UserControl
    {
        public AssistNavButton()
        {
            InitializeComponent();
        }

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(AssistNavButton));




        public PathGeometry pathGeometry
        {
            get { return (PathGeometry)GetValue(pathGeometryProperty); }
            set { SetValue(pathGeometryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for pathGeometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty pathGeometryProperty =
            DependencyProperty.Register("pathGeometry", typeof(PathGeometry), typeof(AssistNavButton));

        public bool? isChecked
        {
            get { return (bool?)GetValue(isCheckedProperty); }
            set { SetValue(isCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCheck.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty isCheckedProperty =
            DependencyProperty.Register("isChecked", typeof(bool?), typeof(AssistNavButton));



        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonClick;

        private void navBTN_Click(object sender, RoutedEventArgs e)
        {
            if (this.ButtonClick != null)
                this.ButtonClick(this, e);
        }
    }
}
