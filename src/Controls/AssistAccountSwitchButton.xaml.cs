using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Assist.Settings;

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistAccountSwitchButton.xaml
    /// </summary>
    public partial class AssistAccountSwitchButton : UserControl
    {
        public AssistAccountSwitchButton()
        {
            InitializeComponent();
        }



        internal AccountSettings Account
        {
            get { return (AccountSettings)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Account.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AccountProperty =
            DependencyProperty.Register("Account", typeof(AccountSettings), typeof(AssistAccountSwitchButton));




        public double checkmarkOpac
        {
            get { return (double)GetValue(checkmarkOpacProperty); }
            set { SetValue(checkmarkOpacProperty, value); }
        }

        // Using a DependencyProperty as the backing store for checkmarkOpac.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty checkmarkOpacProperty =
            DependencyProperty.Register("checkmarkOpac", typeof(double), typeof(AssistAccountSwitchButton));



        public string accountRegion
        {
            get { return (string)GetValue(accountRegionProperty); }
            set { SetValue(accountRegionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for accountRegion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty accountRegionProperty =
            DependencyProperty.Register("accountRegion", typeof(string), typeof(AssistAccountSwitchButton));



        public string accountName
        {
            get { return (string)GetValue(accountNameProperty); }
            set { SetValue(accountNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for accountName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty accountNameProperty =
            DependencyProperty.Register("accountName", typeof(string), typeof(AssistAccountSwitchButton));


        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonClick;

        private void accSwitchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.ButtonClick != null)
                this.ButtonClick(this, e);
        }
    }
}
