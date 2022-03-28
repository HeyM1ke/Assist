using System;
using System.Xml;
using System.Xml.Linq;
using System.Net.Sockets;
using System.Net.Security;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Assist.MVVM.ViewModel;
using ValNet;

namespace Assist.Modules.XMPP
{
    internal class XmppClient
    {
        private string xmppHostUrl { get; set; }
        private RiotPPClient _ppClient;
        private RiotUser _xmppUser;
        private readonly TcpClient _client;
        private SslStream client;

        private List<string> _xmlStack;
        private List<string> _xmlParsedMessages;
        private int _xmppMessagesHandled = 0;


        public XmppClient(RiotUser user)
        {
            xmppHostUrl = $"la1.chat.si.riotgames.com";
            _client = new TcpClient();
            _xmppUser = user;
            _xmlStack = new List<string>();
            _xmlParsedMessages = new List<string>();
        }


		public async Task ConnectAsync()
		{
            AssistLog.Debug("Attempting to Connect to XMPP...");
            try
            {
                await _client.ConnectAsync(xmppHostUrl, 5223);
                client = new SslStream(_client.GetStream(), false);
                client.AuthenticateAsClient(xmppHostUrl);
			}
            catch (Exception e)
            {
				AssistLog.Error(e.Message);
                throw;
            }

            var _ = Task.Run(async () => await ParseStreamAsync());
		}

		internal async Task WriteAsync(string message, int skipCount = 0)
		{
			await client.WriteAsync(Encoding.UTF8.GetBytes(message), 0, Encoding.UTF8.GetBytes(message).Length);
			await ReceiveAsync(skipCount);
		}

		internal async Task WriteXMLAsync(XElement xml, int skipCount = 0)
		{
			await client.WriteAsync(Encoding.UTF8.GetBytes(xml.ToString(SaveOptions.DisableFormatting)), 0, Encoding.UTF8.GetBytes(xml.ToString(SaveOptions.DisableFormatting)).Length);
			await ReceiveAsync(skipCount);
		}

		internal async Task<string> ReceiveSingleAsync()
		{
			return await Task.Run(() =>
			{
				while (true)
				{
					if (_xmlParsedMessages.Count > 0 && _xmlParsedMessages[0] != null)
					{
						var temp = _xmlParsedMessages[0];
						_xmlParsedMessages.RemoveAt(0);
						return temp;
					}
				}
			});
		}

		internal async Task<XmlElement> ReceiveSingleXMLAsync()
		{
			return await Task.Run(async () =>
			{
				while (true)
				{
					if (_xmlParsedMessages.Count > 0 && _xmlParsedMessages[0] != null)
					{
						XmlDocument _document = new XmlDocument();
						try
						{
							_document.LoadXml(_xmlParsedMessages[0]);
                            _xmlParsedMessages.RemoveAt(0);

							return _document.DocumentElement;
						}
						catch
						{
							_document.LoadXml("<root>" + _xmlParsedMessages[0] + "</root>");
							foreach (XmlNode child in _document.DocumentElement.ChildNodes)
							{
								_xmlParsedMessages.Add(child.OuterXml);
							}
							_xmlParsedMessages.RemoveAt(0);
							return await ReceiveSingleXMLAsync();
						}
					}
				}
			});
		}

		internal async Task<List<string>> ReceiveAsync(int count = 1)
		{
			return await Task.Run(() =>
			{
				List<string> messages = new List<string>();
				while (true)
				{
					if (messages.Count == count)
						break;

					if (_xmlParsedMessages.Count > 0 && _xmlParsedMessages[0] != null)
					{
						messages.Add(_xmlParsedMessages[0]);
                        _xmlParsedMessages.RemoveAt(0);
					}
				}

				return messages;
			});
		}

		// Where all Data Gets Sent.
		internal async Task HandleEventsAsync() 
		{
			while (true)
			{
				try
				{
					XmlElement _message = await ReceiveSingleXMLAsync();
					AssistLog.Normal(_message.ToString());
				}
				catch (Exception ex) { Console.WriteLine(ex); }

			}
		}

		private async Task ParseStreamAsync()
		{
			while (true)
			{
				byte[] xmppMessageBuffer = new byte[2048];
				await client.ReadAsync(xmppMessageBuffer, 0, xmppMessageBuffer.Length);
				string xmppMessage = Encoding.ASCII.GetString(xmppMessageBuffer);
				if (!string.IsNullOrEmpty(xmppMessage))
				{
					foreach (string raw_tag in xmppMessage.Split('>').Where(tag => !string.IsNullOrEmpty(tag.Replace("\0", ""))))
					{
                        
						string tag = raw_tag;
                        if (xmppMessage.Contains(raw_tag + ">"))
							tag = raw_tag + ">";
						if (_xmlStack.Count > 0)
						{
							if (tag.Contains($"</{_xmlStack[_xmlStack.Count - 1].Split('>')[0].Split(' ')[0].Replace("<", "").Replace(">", "")}"))
								await ParseXMLAsync(tag);
							else
								_xmlStack[_xmlStack.Count - 1] += tag;
						}
						else
						{
							if (_xmppMessagesHandled != 0 && _xmppMessagesHandled != 3)
							{
                                _xmlStack.Add(tag);
								if (tag.EndsWith("/>"))
									await ParseXMLAsync(null);
							}
						}
					}
				}
				_xmppMessagesHandled += 1;
			}
		}

		private async Task ParseXMLAsync(string tag)
		{
			await Task.Run(() =>
			{
				string fullTag = _xmlStack[_xmlStack.Count - 1] + tag;
				fullTag = fullTag.Replace("\0", "");
				_xmlStack.RemoveAt(_xmlStack.Count - 1);
				_xmlParsedMessages.Add(fullTag);
            });
		}
	}
}
