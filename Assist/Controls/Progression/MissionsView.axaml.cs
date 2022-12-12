using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Assist.Controls.Progression
{
    public class MissionsView : TemplatedControl
    {
        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<MissionsView, string>("Title", "Missions");
        public static readonly StyledProperty<object?> ContentProperty = AvaloniaProperty.Register<MissionsView, object?>("Content");


        public object? Content
        {
            get { return (object?)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

       


        public MissionsView()
        {
            
        }

    }
}
