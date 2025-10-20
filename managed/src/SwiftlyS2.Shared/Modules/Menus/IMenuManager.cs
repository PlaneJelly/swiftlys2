using SwiftlyS2.Shared.Players;

namespace SwiftlyS2.Shared.Menus;

public struct MenuSettings
{
    public string NavigationPrefix;
    public string InputMode;
    public string ButtonsUse;
    public string ButtonsScroll;
    public string ButtonsExit;
    public string SoundUseName;
    public float SoundUseVolume;
    public string SoundScrollName;
    public float SoundScrollVolume;
    public string SoundExitName;
    public float SoundExitVolume;
    public int ItemsPerPage;
}


public interface IMenuManager
{
    public IMenu CreateMenu(string title);
    public IMenu? GetMenu(IPlayer player);
    public void CloseMenu(IMenu menu);
    public void CloseMenuForPlayer(IPlayer player);
    public void CloseMenuByTitle(string title, bool exact = false);
    public void OpenMenu(IPlayer player, IMenu menu);

    event Action<IPlayer, IMenu>? OnMenuClosed;
    event Action<IPlayer, IMenu>? OnMenuOpened;
    event Action<IPlayer, IMenu>? OnMenuRendered;
    /// <summary>
    /// Menu settings.
    /// </summary>
    public MenuSettings Settings { get; }
}