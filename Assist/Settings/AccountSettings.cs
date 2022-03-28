using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ValNet;
using ValNet.Objects;

namespace Assist.Settings
{
    internal class AccountSettings
    {
        public string puuid { get; set; }
        public string Gamename {  get; set; }   
        public string Tagline {  get; set; }
        public RiotRegion Region { get; set; } 

        public string cAuth64 { get; set; }



        public void ConvertCookiesTo64(CookieContainer container)
        {
            string sixFour = "";

            foreach(Cookie cookie in container.GetAllCookies())
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(cookie.ToString());
                sixFour += Convert.ToBase64String(plainTextBytes) + ";a;";
            }

            this.cAuth64 = sixFour;
        }

        public CookieContainer Convert64ToCookies()
        {
            CookieContainer cookiecontainer = new CookieContainer();
            string[] cookies = cAuth64.Split(";a;");

            foreach (string cookie in cookies)
            {
                var regCookie = Encoding.UTF8.GetString(Convert.FromBase64String(cookie));
                cookiecontainer.SetCookies(new Uri("https://auth.riotgames.com"), regCookie);
            }

            return cookiecontainer;

        }
    }
}
