using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Progression
{
    public class MissionControl : TemplatedControl
    {
        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<MissionControl, string>("Title", "Get 25 headshots");
        public static readonly StyledProperty<double> MaxProgressProperty = AvaloniaProperty.Register<MissionControl, double>("MaxProgress", 400);
        public static readonly StyledProperty<double> CurrentProgressProperty = AvaloniaProperty.Register<MissionControl, double>("CurrentProgress", 300);
        public static readonly StyledProperty<string> PreviewTextProperty = AvaloniaProperty.Register<MissionControl, string>("PreviewText", "2500/2500");
        public static readonly StyledProperty<int> TitleFontSizeProperty = AvaloniaProperty.Register<MissionControl, int>("TitleFontSize", 12);
        public static readonly StyledProperty<string> XpGrantAmountProperty = AvaloniaProperty.Register<MissionControl, string>("XpGrantAmount", "32,000XP");

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set
            {
                DetermineStringFontSize(value);
                SetValue(TitleProperty, value);

            }
        }

        public double MaxProgress
        {
            get { return (double)GetValue(MaxProgressProperty); }
            set { SetValue(MaxProgressProperty, value); }
        }

        public double CurrentProgress
        {
            get { return (double)GetValue(CurrentProgressProperty); }
            set { SetValue(CurrentProgressProperty, value); }
        }

        public string PreviewText
        {
            get { return (string)GetValue(PreviewTextProperty); }
            set { SetValue(PreviewTextProperty, value); }
        }

        public int TitleFontSize
        {
            get { return (int)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        public string XpGrantAmount
        {
            get { return (string)GetValue(XpGrantAmountProperty); }
            set { SetValue(XpGrantAmountProperty, value); }
        }

        private void DetermineStringFontSize(string stringInQuestion)
        {
            if (stringInQuestion.Length <= 22)
                return;

            if (stringInQuestion.Length >= 22 && stringInQuestion.Length <= 29)
                TitleFontSize = 10;

            if(stringInQuestion.Length >= 29)
                TitleFontSize = 8;
        }
    }
}
