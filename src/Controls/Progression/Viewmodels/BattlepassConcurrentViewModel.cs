using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using AssistWPFTest.MVVM.ViewModel;

namespace Assist.Controls.Progression.Viewmodels
{
    internal class BattlepassConcurrentViewModel : ViewModelBase
    {
        private BitmapImage _contractRewardImage = App.LoadImageUrl("https://cdn.rumblemike.com/PlayerCards/6a82ba95-436f-9d3c-8710-d3a9e09e8445_DisplayIcon.png").Result;

        public BitmapImage ContractRewardImage
        {
            get => _contractRewardImage;
            set => SetProperty(ref _contractRewardImage, value);
        }
    }
}
