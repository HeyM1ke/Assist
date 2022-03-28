using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ValNet;
using ValNet.Objects;

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
        public string profileNote { get; set; }

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
            await pUser.Inventory.GetPlayerInventory();

            PCID = pUser.Inventory.CurrentInventory.PlayerData.PlayerCardID;
            playerLevel = pUser.Inventory.CurrentInventory.PlayerData.AccountLevel;
        }
    }

    public class ProfileOptions
    {
        public bool bShowTagline = true;
        public string profileNote = "";
    }
}
