using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Assist.MVVM.Model;

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistBattlepassItem.xaml
    /// </summary>
    public partial class AssistBattlepassItem : UserControl
    {
        public readonly BattlePassObj.RewardItem itemData;
        public AssistBattlepassItem(BattlePassObj.RewardItem data)
        {
            itemData = data;
            InitializeComponent();
        }



        public bool bIsEarned
        {
            get { return (bool)GetValue(bIsEarnedProperty); }
            set { SetValue(bIsEarnedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for isEarned.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty bIsEarnedProperty =
            DependencyProperty.Register("bIsEarned", typeof(bool), typeof(AssistBattlepassItem));



        public bool bCurrentItem
        {
            get { return (bool)GetValue(bCurrentItemProperty); }
            set { SetValue(bCurrentItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for bCurrentItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty bCurrentItemProperty =
            DependencyProperty.Register("bCurrentItem", typeof(bool), typeof(AssistBattlepassItem));



        public bool bIsSelected
        {
            get { return (bool)GetValue(bIsSelectedProperty); }
            set { SetValue(bIsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for bIsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty bIsSelectedProperty =
            DependencyProperty.Register("bIsSelected", typeof(bool), typeof(AssistBattlepassItem));




        private void Item_Initialized(object sender, EventArgs e)
        {
            earnableImage.Stretch = Stretch.Uniform;

            loadImage(itemData.imageUrl);

            tierName.Content = $"TIER {itemData.tierNumber}";

        }

        private async void loadImage(string url)
        {
            // Allows the image to be loaded with the resolution it is intended to be used for.
            // Because the program is a solo resolution that doesnt change res, this is fine.

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(url, UriKind.Absolute);
            image.EndInit();

            earnableImage.Source = image;

        }

        private void Item_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("Item Down");
        }

        private void Item_MouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("Item Up");
        }
    }
}
