using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using Assist.MVVM.ViewModel;
using Serilog;
using ValNet;
using ValNet.Objects;
using ValNet.Objects.Authentication;

namespace Assist.Settings
{
    public class ProfileSetting
    {
        public string ProfileUuid { get; set; }
        public string RiotId { get => $"{Gamename}#{Tagline}";}
        public string Gamename { get; set; }
        public string Tagline { get; set; }
        public RiotRegion Region { get; set; }
        public string PCID { get; set; }
        public string AssCAuth64 { get; set; }
        public int playerLevel { get; set; }
        public DateTime TimeOfLogin { get; set; }

        public int Tier { get; set; } = 0;
        public string profileNote { get; set; }

        internal List<PatchlineObj> entitlements = new List<PatchlineObj>();

        /// <summary>
        /// Converts Cookies within container to AssCAuth64 Format.
        /// </summary>
        /// <param name="container"> Cookie Container Containing Cookies</param>
        public void ConvertCookiesTo64(CookieContainer container)
        {
            string sixFour = "";

            foreach (Cookie cookie in container.GetAllCookies())
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(cookie.ToString());
                sixFour += Convert.ToBase64String(plainTextBytes) + ";a;";
            }

            this.AssCAuth64 = sixFour;
        }
        /// <summary>
        /// Converts AssCAuth64 String Format to Cookies for Authentication
        /// </summary>
        /// <returns></returns>
        public CookieContainer Convert64ToCookies()
        {
            CookieContainer cookiecontainer = new CookieContainer();
            string[] cookies = AssCAuth64.Split(";a;");

            foreach (string cookie in cookies)
            {
                var regCookie = Encoding.UTF8.GetString(Convert.FromBase64String(cookie));
                cookiecontainer.SetCookies(new Uri("https://auth.riotgames.com"), regCookie);
            }

            return cookiecontainer;

        }



        public async Task SetupProfile(RiotUser pUser)
        {
            try
            {
               var inv =  await pUser.Inventory.GetPlayerInventory();
               PCID = inv.PlayerData.PlayerCardID;
            }
            catch (Exception e)
            {
                PCID = "9ee85a55-4b94-e382-b8a8-f985a33c1cc2";
            }

            try
            {
                var r = await pUser.Player.GetPlayerProgression();
                playerLevel = r.Progress.Level;
            }
            catch (Exception e)
            {
                playerLevel = 1;
            }

            try
            {
                var rnkD = await pUser.Player.GetCompetitiveUpdates();

                if (rnkD != null && rnkD.Matches != null)
                {
                    if (rnkD.Matches.Count != 0)
                        Tier = rnkD.Matches[0].TierAfterUpdate;
                }
                
            }
            catch (Exception e)
            {
                Tier = 0;
            }

            try
            {
                this.entitlements.Clear();

                // Every Account has Access to Live.
                entitlements.Add(new()
                {
                    PatchlineName = "Live",
                    PatchlinePath = "live"
                });

                var entitles = await pUser.Authentication.GetPlayerGameEntitlements();

                entitles.ForEach(x => this.entitlements.Add(x));

            }
            catch (ValNetException e)
            {
                Log.Error(e.RequestContent);
                Log.Error(e.RequestStatusCode.ToString());
                Log.Error(e.Message);
            }

            
            Gamename = pUser.UserData.acct.game_name;
            Log.Debug("Name being Read" + Gamename);
            Tagline = pUser.UserData.acct.tag_line;
            Log.Debug("Tag being Read" + Tagline);

        }
    }

    public class ProfileOptions
    {
        public bool bShowTagline = true;
        public string profileNote = "";
    }
}
