using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Nyaavigator.Messages;

namespace Nyaavigator.Views;

public partial class FollowView : DialogViewBase, IRecipient<NotificationMessage>
{
    public FollowView()
    {
        InitializeComponent();

        CloseButton.Click += (_, _) => Hide();
    }

    private void Expander_Collapsing(object? sender, CancelRoutedEventArgs e)
    {
        e.Cancel = true;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        CloseButton.Focus();
    }

    public void Receive(NotificationMessage message)
    {
        Flyout flyout = new()
        {
            Content = new TextBlock
            {
                Text = message.Value.Message,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            },
            Placement = PlacementMode.Bottom,
            VerticalOffset = -100,
            PlacementConstraintAdjustment = PopupPositionerConstraintAdjustment.None
        };
        flyout.ShowAt(this);
        DispatcherTimer.RunOnce(() => flyout.Hide(), TimeSpan.FromSeconds(3));
    }
}