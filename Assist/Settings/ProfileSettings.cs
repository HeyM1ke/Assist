using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Assist.Objects.Enums;
using Assist.ViewModels;
using Serilog;
using ValNet;
using ValNet.Enums;
using YamlDotNet.Core;

namespace Assist.Settings
{
    public class ProfileSettings
    {
        public string ProfileUuid { get; set; }
        public string RiotId => $"{Gamename}#{Tagline}";
        public string Gamename { get; set; }
        public string Tagline { get; set; }
        public RiotRegion Region { get; set; }
        public int PlayerLevel { get; set; }
        public int ValRankTier { get; set; } = 0;
        public string PlayerCardId { get; set; }
        public string ProfileNote { get; set; }
        public bool isExpired { get; set; } = false;
        public string Username { get; set; }
        public EAuthenticationType AuthType { get; set; }
        public string CAuth { get; set; }
        public DateTime LastUsed { get; set; }

        public async Task SetupProfile(RiotUser u)
        {
            ProfileUuid = u.UserData.sub;
            Gamename = u.UserData.acct.game_name;
            Tagline = u.UserData.acct.tag_line;
            Username = u.GetCredentials().username;
            Region = u.GetRegion();
            LastUsed = DateTime.UtcNow;

            await SetupProfileData(u);

            Log.Information("Name being Read" + Gamename);
            Log.Information("Tag being Read" + Tagline);
        }

        public Dictionary<string, string> Convert64ToCookies()
        {
            Dictionary<string, string> cookiecontainer = new Dictionary<string, string>();

            foreach (string cookie in CAuth.Split(";assist;"))
            {
                var regCookie = Encoding.UTF8.GetString(Convert.FromBase64String(cookie));
                var c = regCookie.Split("::");
                if(c.Length <= 1)
                    continue;
                cookiecontainer.Add(c[0], c[1]);
            }

            return cookiecontainer;

        }

        public void ConvertCookiesTo64(Dictionary<string, string> container)
        {
            string sixFour = "";

            foreach (var cookie in container)
            {
                string s = $"{cookie.Key}::{cookie.Value}";
                var plainTextBytes = Encoding.UTF8.GetBytes(s);
                sixFour += Convert.ToBase64String(plainTextBytes) + ";assist;";
            }

            this.CAuth = sixFour;
        }

        public void ConvertCookiesTo64(IEnumerable<Cookie> container)
        {
            string sixFour = "";

            foreach (var cookie in container)
            {
                string s = $"{cookie.Name}::{cookie.Value}";
                var plainTextBytes = Encoding.UTF8.GetBytes(s);
                sixFour += Convert.ToBase64String(plainTextBytes) + ";assist;";
            }

            this.CAuth = sixFour;
        }

        public async Task SetupProfileData(RiotUser pUser)
        {
            // Access Player Inventory and Grab Data from Resp
            var inv = await pUser.Inventory.GetPlayerInventory();

            this.PlayerCardId = inv.PlayerData.PlayerCardID;
        }
    }
}
