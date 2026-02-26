using Android.App;
using Android.Content.PM;
using Avalonia.Android;
using Nyaavigator.Core;

namespace Nyaavigator.Android;

[Activity(
    Label = Constants.Title,
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity;