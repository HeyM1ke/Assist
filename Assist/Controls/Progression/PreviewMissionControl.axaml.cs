using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Progression;

public class PreviewMissionControl : TemplatedControl
{
        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<PreviewMissionControl, string>("Title", "Get 25 headshots");
        public static readonly StyledProperty<double> MaxProgressProperty = AvaloniaProperty.Register<PreviewMissionControl, double>("MaxProgress", 400);
        public static readonly StyledProperty<double> CurrentProgressProperty = AvaloniaProperty.Register<PreviewMissionControl, double>("CurrentProgress", 300);
        public static readonly StyledProperty<string> PreviewTextProperty = AvaloniaProperty.Register<PreviewMissionControl, string>("PreviewText", "2500/2500");
        public static readonly StyledProperty<int> TitleFontSizeProperty = AvaloniaProperty.Register<PreviewMissionControl, int>("TitleFontSize", 12);
        public static readonly StyledProperty<string> XpGrantAmountProperty = AvaloniaProperty.Register<PreviewMissionControl, string>("XpGrantAmount", "32,000XP");
        public static readonly StyledProperty<object?> ContentProperty = AvaloniaProperty.Register<PreviewMissionControl, object?>("Content");

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

        public object? Content
        {
            get { return (object?)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        private void DetermineStringFontSize(string stringInQuestion)
        {
            if (stringInQuestion.Length <= 22)
                return;

            if (stringInQuestion.Length >= 22 && stringInQuestion.Length <= 29)
                TitleFontSize = 12;

            if(stringInQuestion.Length >= 29)
                TitleFontSize = 10;
        }
}