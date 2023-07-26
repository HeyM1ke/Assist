using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Assist.Game.Controls.Live
{
    public class LiveMenuPartyUser : TemplatedControl
    {
        public static readonly StyledProperty<string?> PlayerNameProperty = AvaloniaProperty.Register<LiveMenuPartyUser, string?>("PlayerName", "AssistUser");
        public static readonly StyledProperty<string?> PlayercardProperty = AvaloniaProperty.Register<LiveMenuPartyUser, string?>("Playercard", "https://media.valorant-api.com/playercards/33c1f011-4eca-068c-9751-f68c788b2eee/largeart.png");
        public static readonly StyledProperty<string?> AgentImageProperty = AvaloniaProperty.Register<LiveMenuPartyUser, string?>("AgentImage", "https://media.valorant-api.com/agents/dade69b4-4f5a-8528-247b-219e5a1facd6/fullportrait.png");
        public static readonly StyledProperty<bool?> PlayerReadyProperty = AvaloniaProperty.Register<LiveMenuPartyUser, bool?>("PlayerReady", false);

        public string? PlayerName
        {
            get { return (string?)GetValue(PlayerNameProperty); }
            set { SetValue(PlayerNameProperty, value); }
        }

        public string? Playercard
        {
            get { return (string?)GetValue(PlayercardProperty); }
            set { SetValue(PlayercardProperty, value); }
        }

        public string? AgentImage
        {
            get { return (string?)GetValue(AgentImageProperty); }
            set { SetValue(AgentImageProperty, value); }
        }

        public bool? PlayerReady
        {
            get { return (bool?)GetValue(PlayerReadyProperty); }
            set { SetValue(PlayerReadyProperty, value); }
        }

        public Bitmap? PlayerReputationLevel
        {
            get { return (Bitmap?)GetValue(PlayerReputationLevelProperty); }
            set { SetValue(PlayerReputationLevelProperty, value); }
        }

        public string? PlayerId;
        public static readonly StyledProperty<Bitmap?> PlayerReputationLevelProperty = AvaloniaProperty.Register<LiveMenuPartyUser, Bitmap?>("PlayerReputationLevel");
    }
}
