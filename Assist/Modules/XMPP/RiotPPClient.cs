using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Assist.MVVM.ViewModel;
using Microsoft.VisualBasic.ApplicationServices;
using ValNet;
using ValNet.Objects.Authentication;

namespace Assist.Modules.XMPP
{
    internal class RiotPPClient
    {
        private string _jid;
        private RiotUser _xmppUser;
        private XmppClient _xmppClient;
        private string _xmppRegion;

        public RiotPPClient(RiotUser pUser)
        {
            _xmppRegion = "la1";
            _xmppUser = pUser;
            // Get XMPP Region from Riot User 
            _xmppClient = new XmppClient(_xmppUser);

        }


        private async Task SendStreamDeclaration()
        {
            await _xmppClient.WriteAsync(new XDeclaration("1.0", "UTF-8", "no").ToString());
            await _xmppClient.WriteAsync(
                $"<stream:stream to=\"{_xmppRegion}.pvp.net\" xml:lang=\"en\" version=\"1.0\" xmlns=\"jabber:client\" xmlns:stream=\"http://etherx.jabber.org/streams\">",
                1);
        }
        internal async Task<XmppResult> AuthAsync()
        {
            try
            {
                AssistLog.Debug("Connecting to XMPP...");
                await _xmppClient.ConnectAsync();
                AssistLog.Debug("Connected to XMPP.");
                await SendStreamDeclaration();

                AssistLog.Debug("Attempting to Auth with XMPP...");

                var test = new XElement((XNamespace) "urn:ietf:params:xml:ns:xmpp-sasl" + "auth",
                    new XAttribute("mechanism", "X-Riot-RSO-PAS"),
                    new XElement("rso_token", _xmppUser.tokenData.access),
                    new XElement("pas_token", _xmppUser.tokenData.pasToken));

                AssistLog.Debug(test.ToString());

                await _xmppClient.WriteXMLAsync(test);

                if ((await _xmppClient.ReceiveSingleAsync()).Contains("success"))
                {
                    await SendStreamDeclaration();
                    await _xmppClient.WriteXMLAsync(new XElement("iq", new XAttribute("id", "_xmpp_bind1"),
                        new XAttribute("type", "set"),
                        new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-bind" + "bind",
                            new XElement("puuid-mode", new XAttribute("enabled", true)),
                            new XElement("resource", "RC-ASSIST-CLIENT"))));
                    _jid = (await _xmppClient.ReceiveAsync())[0].Split("<jid>")[1].Split("</jid>")[0];
                    await _xmppClient.WriteXMLAsync(new XElement("iq", new XAttribute("id", "xmpp_entitlements_0"),
                        new XAttribute("type", "set"),
                        new XElement((XNamespace)"urn:riotgames:entitlements" + "entitlements",
                            new XElement("token", _xmppUser.tokenData.entitle))), 1);
                    await _xmppClient.WriteXMLAsync(new XElement("iq", new XAttribute("id", "set_rxep_1"),
                        new XAttribute("type", "set"),
                        new XElement((XNamespace)"urn:riotgames:rxep" + "rxcep",
                            "&lt;last-online-state enabled='true' /&gt;")), 1);
                    await _xmppClient.WriteXMLAsync(new XElement("iq", new XAttribute("id", "_xmpp_session1"),
                        new XAttribute("type", "set"),
                        new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-session" + "session")));

                    var _userId = (await _xmppClient.ReceiveSingleXMLAsync())["session"]["id"];
                    await GetFriendsAsync();
                    var _ = Task.Run(async () => await _xmppClient.HandleEventsAsync());
                    return new XmppResult() { bIsSuccessful = true };
                }

            }
            catch (Exception ex)
            {
                AssistLog.Error(ex.Message);
            }

           

            return new XmppResult()
            {
                bIsSuccessful = false
            };

        }
        async Task GetFriendsAsync()
        {
            AssistLog.Debug("Getting friends list from XMPP.");
            await _xmppClient.WriteXMLAsync(new XElement("iq", new XAttribute("type", "get"),
                new XElement((XNamespace)"jabber:iq:riotgames:roster" + "query")));
            XmlDocument RosterXML = new XmlDocument();
            RosterXML.LoadXml(await _xmppClient.ReceiveSingleAsync());
            foreach (XmlNode Item in RosterXML.FirstChild.FirstChild.ChildNodes)
            {
                AssistLog.Debug(Item.ToString());
            }

            AssistLog.Debug("Got friends list from XMPP.");
        }
    }
}

internal class XmppResult
{
    public bool bIsSuccessful;
}
