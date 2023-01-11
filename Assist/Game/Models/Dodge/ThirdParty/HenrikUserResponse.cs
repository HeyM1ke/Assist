using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.Game.Models.Dodge.ThirdParty
{
    internal class HenrikUserResponse
    {
        public int status { get; set; } 
        public Data data { get; set; }
        

        public class Data
        {
            public string puuid { get; set; }
            public string region { get; set; }
            public int account_level { get; set; }
            public string name { get; set; }
            public string tag { get; set; }
            public Card card { get; set; }
            public string last_update { get; set; }
            public int last_update_raw { get; set; }
        }

        public class Card
        {
            public string small { get; set; }
            public string large { get; set; }
            public string wide { get; set; }
            public string id { get; set; }
        }

    }
}
