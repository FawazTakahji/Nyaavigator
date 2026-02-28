using Nyaavigator.Core.Settings;

namespace Nyaavigator.Core.Services;

public interface IAppManager
{
    public void Initialize();
    public void SetTheme(Theme theme);
}