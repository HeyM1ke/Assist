using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Assist.Objects.RiotClient
{
    public class RiotGamesPrivateModel
    {
        [YamlMember(Alias = "riot-login", ApplyNamingConventions = false)]
        public RiotLoginSection RiotLogin = null;

        [YamlMember(Alias = "rso-authenticator", ApplyNamingConventions = false)]
        public object? rsoAuth = null;
    }

    public class RiotLoginSection
    {
        [YamlMember(Alias = "persist", ApplyNamingConventions = false)]
        public PersistSection Persist;
    }

    public class PersistSection
    {
        [YamlMember(Alias = "region", ApplyNamingConventions = false)]
        public string Region;
    
        [YamlMember(Alias = "session", ApplyNamingConventions = false)]
        public SessionSection Session;
    }

    public class SessionSection
    {
        [YamlMember(Alias = "cookies", ApplyNamingConventions = false)]
        public List<RiotCookie> Cookies;
    }

    public class RiotCookie
    {
        public string domain;
        public bool hostOnly;
        public bool httpOnly;
        public string name;
        public string path;
        public bool persistent;
        public bool secureOnly;
        public string value;
    }
}