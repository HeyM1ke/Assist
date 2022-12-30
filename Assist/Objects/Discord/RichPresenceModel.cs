using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.Objects.Discord
{
    internal class RichPresenceModel
    {
        public bool ShowAgent { get; set; } = true;
        public bool ShowRank { get; set; } = false;
        public bool ShowScore { get; set; } = true;
        public bool ShowMode { get; set; } = true;
        public bool ShowParty { get; set; } = true;

        public string LargeImageData = "Map"; // Map, Agent, Logo, None Defaults: Map
        public string SmallImageData = "Agent"; // Agent, Rank, None Defaults: Agent
        public string HoverTextData = "Assist"; // Assist, Rank RR, None Defaults: Assist
        public string DetailsTextData = "Assist"; // Assist, Rank RR, None Defaults: Assist
    }
}
