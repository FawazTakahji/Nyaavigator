using Avalonia.Interactivity;

namespace Nyaavigator.Views;

public partial class NewReleasesView : DialogViewBase
{
    public NewReleasesView()
    {
        InitializeComponent();

        CloseButton.Click += (_, _) => Hide();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        CloseButton.Focus();
    }
}