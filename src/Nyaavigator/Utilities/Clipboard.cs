using System.Threading.Tasks;

namespace Nyaavigator.Utilities;

public static class Clipboard
{
    public static async Task Copy(string str)
    {
        await App.TopLevel.Clipboard.SetTextAsync(str);
    }
}