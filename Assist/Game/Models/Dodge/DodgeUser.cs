using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.Game.Models.Dodge
{
    public class DodgeUser
    {
        public string UserId { get; set; }
        public string GameName { get; set; }
        public string Note { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
