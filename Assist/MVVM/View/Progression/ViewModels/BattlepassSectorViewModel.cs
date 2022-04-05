using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Assist.Controls;
using Assist.Controls.Progression;
using Assist.MVVM.Model;
using Assist.MVVM.ViewModel;
using AssistWPFTest.MVVM.ViewModel;
using ValNet;
using ValNet.Objects.Contacts;

namespace Assist.MVVM.View.Progression.ViewModels
{
    internal class BattlepassSectorViewModel : ViewModelBase
    {
        private ContactsFetchObj.Contract BattlepassContractData;
        private List<BattlePassObj> BattlePassData = null;
        public async Task LoadBattlepass(object container)
        {
            UniformGrid ItemContainer = (UniformGrid) container;
            BattlepassContractData = await AssistApplication.AppInstance.CurrentUser.Contracts.GetCurrentBattlepass();
            BattlePassData = await AssistApplication.AppInstance.AssistApiController.GetBattlepassData();

            if(BattlePassData is null || BattlepassContractData is null || ItemContainer is null)
                return;

            int tier = 1;
            // Convert obj param to Uniform as 
            for (int i = 0; i < BattlePassData.Count; i++)
            {
                var listOfItems = BattlePassData[i].itemsInChapter;
                foreach (var Item in listOfItems)
                {
                    

                    Item.tierNumber = tier;
                    ItemContainer.Children.Add(new BattlepassItem(Item)
                    {
                        Margin = new Thickness(8, 6, 8, 6),
                        bIsEarned = BattlepassContractData.ProgressionLevelReached >= tier,
                        bCurrentItem = tier == BattlepassContractData.ProgressionLevelReached+1,

                    });
                    tier++;
                }
                

                
            }
        }

    }
}
