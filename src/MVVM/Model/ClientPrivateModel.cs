using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using ValNet;
using System.Net;
using ValNet.Objects;
using Assist.MVVM.ViewModel;

namespace Assist.MVVM.Model
{
    internal class ClientPrivateModel
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


        public ClientPrivateModel(RiotUser user)
        {
            foreach (Cookie cook in user.UserClient.CookieContainer.GetAllCookies())
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

        public YamlStream CreateFile()
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
                                                new YamlScalarNode($"{ cookieValues.tdid }")
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
    }
    internal class ClientGameModel
    {
        public CookieValue cookieValues = new CookieValue();

        internal struct CookieValue
        {
            public string tdid ;
            public string ssid;
            public string clid;
            public string sub;
            public string csid;
        }


        public ClientGameModel(RiotUser user)
        {
            foreach (Cookie cook in user.UserClient.CookieContainer.GetAllCookies())
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

        public YamlStream CreateFile()
        {
            var settings = new YamlStream(
                new YamlDocument(
                    new YamlMappingNode(                       
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
                                                new YamlScalarNode($"{ cookieValues.tdid }")
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
    }

}
