using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC;
using ValNet;
using YamlDotNet.RepresentationModel;

namespace Assist.Services.Riot
{
    internal class ClientPrivate
    {
        public CookieValue cookieValues = new CookieValue();
    
        internal struct CookieValue
        {
            public string tdid;
            public string ssid;
            public string clid;
            public string sub;
            public string csid;
        }

        public ClientPrivate(RiotUser user)
        {
            foreach (Cookie cook in user.GetAuthClient().ClientCookies)
            {
                switch (cook.Name)
                {
                    case "tdid":
                        cookieValues.tdid = cook.Value;
                        break;
                    case "ssid":
                        cookieValues.ssid = cook.Value;
                        break;
                    case "clid":
                        cookieValues.clid = cook.Value;
                        break;
                    case "sub":
                        cookieValues.sub = cook.Value;
                        break;
                    case "csid":
                        cookieValues.csid = cook.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        public YamlStream CreateClientPrivateModel()
        {
            var settings = new YamlStream(
                new YamlDocument(
                    new YamlMappingNode(
                        new YamlScalarNode("private"), new YamlMappingNode(
                            new YamlScalarNode("riot-login"), new YamlMappingNode(
                                new YamlScalarNode("persist"), new YamlMappingNode(
                                    new YamlScalarNode("session"), new YamlMappingNode(
                                        new YamlScalarNode("cookies"), new YamlSequenceNode(
                                            // TDID Cookie Section
                                            new YamlMappingNode(
                                                new YamlScalarNode("domain"),
                                                new YamlScalarNode("auth.riotgames.com")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                                new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                                                new YamlScalarNode("name"),
                                                new YamlScalarNode("tdid")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("path"),
                                                new YamlScalarNode("/")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                                new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                                new YamlScalarNode("value"),
                                                new YamlScalarNode($"{cookieValues.tdid}")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                            ),
                                            // SSID Cookie Section
                                            new YamlMappingNode(
                                                new YamlScalarNode("domain"),
                                                new YamlScalarNode("auth.riotgames.com")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                                new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                                                new YamlScalarNode("name"),
                                                new YamlScalarNode("ssid")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("path"),
                                                new YamlScalarNode("/")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                                new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                                new YamlScalarNode("value"),
                                                new YamlScalarNode($"{cookieValues.ssid}")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                            ),
                                            // clid Cookie Section
                                            new YamlMappingNode(
                                                new YamlScalarNode("domain"),
                                                new YamlScalarNode("auth.riotgames.com")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                                new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                                                new YamlScalarNode("name"),
                                                new YamlScalarNode("clid")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("path"),
                                                new YamlScalarNode("/")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                                new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                                new YamlScalarNode("value"),
                                                new YamlScalarNode("ue1")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                            ),
                                            // sub Cookie Section
                                            new YamlMappingNode(
                                                new YamlScalarNode("domain"),
                                                new YamlScalarNode("auth.riotgames.com")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                                new YamlScalarNode("httpOnly"), new YamlScalarNode("false"),
                                                new YamlScalarNode("name"),
                                                new YamlScalarNode("sub")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("path"),
                                                new YamlScalarNode("/")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                                new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                                new YamlScalarNode("value"),
                                                new YamlScalarNode($"{cookieValues.sub}")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                            ),
                                            // csid Cookie Section
                                            new YamlMappingNode(
                                                new YamlScalarNode("domain"),
                                                new YamlScalarNode("auth.riotgames.com")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                                new YamlScalarNode("httpOnly"), new YamlScalarNode("false"),
                                                new YamlScalarNode("name"),
                                                new YamlScalarNode("csid")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("path"),
                                                new YamlScalarNode("/")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                                new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                                new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                                new YamlScalarNode("value"),
                                                new YamlScalarNode($"{cookieValues.csid}")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                            )
                                        ))))))));

            return settings;
        }

        public YamlStream CreateGameModel()
        {
            var settings = new YamlStream(
               new YamlDocument(
                   new YamlMappingNode(
                           new YamlScalarNode("riot-login"), new YamlMappingNode(
                               new YamlScalarNode("persist"), new YamlMappingNode(
                                   //new YamlScalarNode("region"), new YamlScalarNode("NA") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                   new YamlScalarNode("session"), new YamlMappingNode(
                                       new YamlScalarNode("cookies"), new YamlSequenceNode(
                                           // TDID Cookie Section
                                           new YamlMappingNode(
                                               new YamlScalarNode("domain"),
                                               new YamlScalarNode("auth.riotgames.com")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                               new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                                               new YamlScalarNode("name"),
                                               new YamlScalarNode("tdid")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("path"),
                                               new YamlScalarNode("/")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                               new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                               new YamlScalarNode("value"),
                                               new YamlScalarNode($"{cookieValues.tdid}")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                           ),
                                           // SSID Cookie Section
                                           new YamlMappingNode(
                                               new YamlScalarNode("domain"),
                                               new YamlScalarNode("auth.riotgames.com")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                               new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                                               new YamlScalarNode("name"),
                                               new YamlScalarNode("ssid")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("path"),
                                               new YamlScalarNode("/")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                               new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                               new YamlScalarNode("value"),
                                               new YamlScalarNode($"{cookieValues.ssid}")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                           ),
                                           // clid Cookie Section
                                           new YamlMappingNode(
                                               new YamlScalarNode("domain"),
                                               new YamlScalarNode("auth.riotgames.com")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                               new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                                               new YamlScalarNode("name"),
                                               new YamlScalarNode("clid")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("path"),
                                               new YamlScalarNode("/")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                               new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                               new YamlScalarNode("value"),
                                               new YamlScalarNode("ue1")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                           ),
                                           // sub Cookie Section
                                           new YamlMappingNode(
                                               new YamlScalarNode("domain"),
                                               new YamlScalarNode("auth.riotgames.com")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                               new YamlScalarNode("httpOnly"), new YamlScalarNode("false"),
                                               new YamlScalarNode("name"),
                                               new YamlScalarNode("sub")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("path"),
                                               new YamlScalarNode("/")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                               new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                               new YamlScalarNode("value"),
                                               new YamlScalarNode($"{cookieValues.sub}")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                           ),
                                           // csid Cookie Section
                                           new YamlMappingNode(
                                               new YamlScalarNode("domain"),
                                               new YamlScalarNode("auth.riotgames.com")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                               new YamlScalarNode("httpOnly"), new YamlScalarNode("false"),
                                               new YamlScalarNode("name"),
                                               new YamlScalarNode("csid")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("path"),
                                               new YamlScalarNode("/")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                               new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                               new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                               new YamlScalarNode("value"),
                                               new YamlScalarNode($"{cookieValues.csid}")
                                               { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                           )
                                       )))))));

            return settings;
        }

        public YamlStream CreateGameModelWRegion()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(cookieValues.tdid);


            var settings = new YamlStream(
                new YamlDocument(
                    new YamlMappingNode(
                        new YamlScalarNode("riot-login"), new YamlMappingNode(
                            new YamlScalarNode("persist"), new YamlMappingNode(
                                new YamlScalarNode("region"),
                                new YamlScalarNode($"NA") // change to actual region for enum in riotuser
                                    { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                new YamlScalarNode("session"), new YamlMappingNode(
                                    new YamlScalarNode("cookies"), new YamlSequenceNode(
                                        // TDID Cookie Section
                                        new YamlMappingNode(
                                            new YamlScalarNode("domain"),
                                            new YamlScalarNode("riotgames.com")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("hostOnly"), new YamlScalarNode("false"),
                                            new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("name"),
                                            new YamlScalarNode("tdid")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("path"),
                                            new YamlScalarNode("/")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                            new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("value"),
                                            new YamlScalarNode($"{cookieValues.tdid}")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                        ),
                                        // SSID Cookie Section
                                        new YamlMappingNode(
                                            new YamlScalarNode("domain"),
                                            new YamlScalarNode("auth.riotgames.com")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("name"),
                                            new YamlScalarNode("ssid")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("path"),
                                            new YamlScalarNode("/")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                            new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("value"),
                                            new YamlScalarNode($"{cookieValues.ssid}")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                        ),
                                        // clid Cookie Section
                                        new YamlMappingNode(
                                            new YamlScalarNode("domain"),
                                            new YamlScalarNode("auth.riotgames.com")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("name"),
                                            new YamlScalarNode("clid")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("path"),
                                            new YamlScalarNode("/")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                            new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("value"),
                                            new YamlScalarNode("ue1")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                        ),
                                        // sub Cookie Section
                                        new YamlMappingNode(
                                            new YamlScalarNode("domain"),
                                            new YamlScalarNode("auth.riotgames.com")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("httpOnly"), new YamlScalarNode("false"),
                                            new YamlScalarNode("name"),
                                            new YamlScalarNode("sub")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("path"),
                                            new YamlScalarNode("/")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                            new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("value"),
                                            new YamlScalarNode($"{cookieValues.sub}")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                        ),
                                        // csid Cookie Section
                                        new YamlMappingNode(
                                            new YamlScalarNode("domain"),
                                            new YamlScalarNode("auth.riotgames.com")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("httpOnly"), new YamlScalarNode("false"),
                                            new YamlScalarNode("name"),
                                            new YamlScalarNode("csid")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("path"),
                                            new YamlScalarNode("/")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                            new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("value"),
                                            new YamlScalarNode($"{cookieValues.csid}")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                        )
                                    )
                                )
                            )
                        ),
                        new YamlScalarNode("rso-authenticator"), new YamlMappingNode(
                        new YamlScalarNode("tdid"), new YamlMappingNode(
                            new YamlScalarNode("domain"), new YamlScalarNode("riotgames.com") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                            new YamlScalarNode("expiryTime"), new YamlScalarNode($"{((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()}") { Style = YamlDotNet.Core.ScalarStyle.Plain },
                            new YamlScalarNode("hostOnly"), new YamlScalarNode("false"),
                            new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                            new YamlScalarNode("name"), new YamlScalarNode("tdid") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                            new YamlScalarNode("path"), new YamlScalarNode("/") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                            new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                            new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                            new YamlScalarNode("value"), new YamlScalarNode($"{cookieValues.tdid}") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                    )
                )
                    )
                 )
            );

            return settings;
        }
        
         public YamlStream CreateGameModelWRegionMicro()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(cookieValues.tdid);


            var settings = new YamlStream(
                new YamlDocument(
                    new YamlMappingNode(
                        new YamlScalarNode("riot-login"), new YamlMappingNode(
                            new YamlScalarNode("persist"), new YamlMappingNode(
                                new YamlScalarNode("region"),
                                new YamlScalarNode($"NA") // change to actual region for enum in riotuser
                                    { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                new YamlScalarNode("session"), new YamlMappingNode(
                                    new YamlScalarNode("cookies"), new YamlSequenceNode(
                                        // SSID Cookie Section
                                        new YamlMappingNode(
                                            new YamlScalarNode("domain"),
                                            new YamlScalarNode("auth.riotgames.com")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("hostOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("name"),
                                            new YamlScalarNode("ssid")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("path"),
                                            new YamlScalarNode("/")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                            new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                                            new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                                            new YamlScalarNode("value"),
                                            new YamlScalarNode($"{cookieValues.ssid}")
                                                { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                                        )
                                    )
                                )
                            )
                        ),
                        new YamlScalarNode("rso-authenticator"), new YamlMappingNode(
                        new YamlScalarNode("tdid"), new YamlMappingNode(
                            new YamlScalarNode("domain"), new YamlScalarNode("riotgames.com") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                            new YamlScalarNode("expiryTime"), new YamlScalarNode($"{((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()}") { Style = YamlDotNet.Core.ScalarStyle.Plain },
                            new YamlScalarNode("hostOnly"), new YamlScalarNode("false"),
                            new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                            new YamlScalarNode("name"), new YamlScalarNode("tdid") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                            new YamlScalarNode("path"), new YamlScalarNode("/") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                            new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                            new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                            new YamlScalarNode("value"), new YamlScalarNode($"{cookieValues.tdid}") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                    )
                )
                    )
                 )
            );

            return settings;
        }

        public YamlStream CreateSettingsModel()
        {
            var settings = new YamlStream(
                new YamlDocument(
                    new YamlMappingNode(
                        new YamlScalarNode("install"), new YamlMappingNode(
                            new YamlScalarNode("cohorts"),
                            new YamlMappingNode(new YamlScalarNode("RC_15.new_lifecycle"), new YamlScalarNode($"globalEnable")),
                            new YamlScalarNode("globals"), new YamlMappingNode(
                                new YamlScalarNode("region"), new YamlScalarNode($"NA") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                                new YamlScalarNode("locale"), new YamlScalarNode($"en_US") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }),
                            new YamlScalarNode("lifecycle"), new YamlMappingNode(
                                /*new YamlScalarNode("enable_launch_on_computer_start_set_by_player"), new YamlScalarNode("false"),
                                new YamlScalarNode("enable_run_in_background_set_by_player"), new YamlScalarNode("false"),*/
                            new YamlScalarNode("multigame-client"), new YamlMappingNode(
                                new YamlScalarNode("shortcut_created"), new YamlScalarNode("true"))),
                            new YamlScalarNode("multigame-client"), new YamlMappingNode(
                                new YamlScalarNode("shortcut_created"), new YamlScalarNode("true"))))));

            return settings;
        }

        public YamlStream CreateRsoAuth()
        {
            var s = new YamlStream(new YamlDocument(
                new YamlMappingNode(
                    new YamlScalarNode("rso-authenticator"), new YamlMappingNode(
                        new YamlScalarNode("tdid"), new YamlMappingNode(
                            new YamlScalarNode("domain"), new YamlScalarNode("riotgames.com") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                            new YamlScalarNode("expiryTime"), new YamlScalarNode($"{((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()}") { Style = YamlDotNet.Core.ScalarStyle.Plain },
                            new YamlScalarNode("hostOnly"), new YamlScalarNode("false"),
                            new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                            new YamlScalarNode("name"), new YamlScalarNode("tdid") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                            new YamlScalarNode("path"), new YamlScalarNode("/") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                            new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                            new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                            new YamlScalarNode("value"), new YamlScalarNode($"{cookieValues.tdid}") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }
                    )
                )
            )));
            
            
            /*var s = new YamlStream(
                new YamlDocument(new YamlMappingNode(
                    new YamlScalarNode("rso-authenticator"),
                    new YamlMappingNode("tdid"), new YamlSequenceNode(
                        new YamlScalarNode("domain"), new YamlScalarNode("riotgames.com") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                        new YamlScalarNode("expiryTime"), new YamlScalarNode("1731550927") { Style = YamlDotNet.Core.ScalarStyle.Plain },
                        new YamlScalarNode("hostOnly"), new YamlScalarNode("false"),
                        new YamlScalarNode("httpOnly"), new YamlScalarNode("true"),
                        new YamlScalarNode("name"), new YamlScalarNode("tdid") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                        new YamlScalarNode("path"), new YamlScalarNode("/") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted },
                        new YamlScalarNode("persistent"), new YamlScalarNode("true"),
                        new YamlScalarNode("secureOnly"), new YamlScalarNode("true"),
                        new YamlScalarNode("value"), new YamlScalarNode($"{cookieValues.tdid}") { Style = YamlDotNet.Core.ScalarStyle.DoubleQuoted }))));*/

            return s;
        }
    }
}
