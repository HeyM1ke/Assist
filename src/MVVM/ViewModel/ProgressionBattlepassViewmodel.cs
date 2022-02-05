using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Controls;
using Assist.MVVM.Model;
using RestSharp;

namespace Assist.MVVM.ViewModel
{
    internal class ProgressionBattlepassViewmodel : INotifyPropertyChanged
    {
        private const string battlepassUrl = "https://assist-api.jaren.workers.dev/battlepass";
        internal List<BattlePassObj> battlepass;
        public AssistBattlepassItem currentItem { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public async Task GetBattlepassData()
        {
            var resp = await new RestClient().ExecuteAsync(new RestRequest(battlepassUrl, Method.Get));

            if (resp.IsSuccessful)
            {
                battlepass =  JsonSerializer.Deserialize<List<BattlePassObj>>(resp.Content);
            }
        }

        private void OnPropertyChanged(string propName)
        {
            if(PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propName)); }
        }
    }
}
