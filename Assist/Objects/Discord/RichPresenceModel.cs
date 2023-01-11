using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.Objects.Discord
{
    public class RichPresenceModel
    {
        public bool ShowAgent { get; set; } = true;
        public bool ShowRank { get; set; } = true;
        public bool ShowScore { get; set; } = true;
        public bool ShowMode { get; set; } = true;
        public bool ShowParty { get; set; } = true;

        public string LargeImageData = "Map"; // Map, Agent, Logo, None Defaults: Map
        public string SmallImageData = "Rank"; // Agent, Rank, None Defaults: Rank
        public string DetailsTextData = "Default"; // Default, Rank, None Defaults: Default
    }
}
