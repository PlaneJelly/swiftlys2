using System.Collections.Concurrent;
using SwiftlyS2.Core.Menu.Options;
using SwiftlyS2.Core.Natives;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace SwiftlyS2.Core.Menus;

internal class Menu : IMenu
{
    public string Title { get; set; } = "";

    public List<IOption> Options { get; set; } = new();

    public IMenu? Parent { get; set; }
    public ConcurrentDictionary<IPlayer, CancellationTokenSource?> AutoCloseCancelTokens { get; set; } = new();
    public IMenuButtonOverrides? ButtonOverrides { get; set; } = new MenuButtonOverrides();
    public int MaxVisibleOptions { get; set; }
    public bool? ShouldFreeze { get; set; } = false;
    public bool? CloseOnSelect { get; set; } = false;
    public Color RenderColor { get; set; } = new(255, 255, 255, 255);

    public IMenuManager MenuManager { get; private set; }

    public float AutoCloseAfter { get; set; } = 0.0f;

    public event Action<IPlayer>? OnOpen;
    public event Action<IPlayer>? OnClose;
    public event Action<IPlayer>? OnMove;
    public event Action<IPlayer, IOption>? OnItemSelected;
    public event Action<IPlayer, IOption>? OnItemHovered;
    public event Action<IPlayer>? BeforeRender;
    public event Action<IPlayer>? AfterRender;

    private ConcurrentDictionary<IPlayer, string> RenderedText { get; set; } = new();
    private ConcurrentDictionary<IPlayer, int> SelectedIndex { get; set; } = new();

    public void Close(IPlayer player)
    {
        NativePlayer.ClearCenterMenuRender(player.PlayerID);
        OnClose?.Invoke(player);
    }

    public void MoveSelection(IPlayer player, int offset)
    {
        if (!SelectedIndex.ContainsKey(player))
        {
            SelectedIndex[player] = 0;
        }

        SelectedIndex[player] += offset;

        if (SelectedIndex[player] < 0) SelectedIndex[player] = -SelectedIndex[player] % Options.Count;
        if (SelectedIndex[player] >= Options.Count) SelectedIndex[player] %= Options.Count;

        OnMove?.Invoke(player);
        OnItemHovered?.Invoke(player, Options[SelectedIndex[player]]);

        Rerender(player);
    }

    public void Rerender(IPlayer player)
    {
        BeforeRender?.Invoke(player);

        NativePlayer.SetCenterMenuRender(player.PlayerID, RenderedText[player]);

        AfterRender?.Invoke(player);
    }

    public void Show(IPlayer player)
    {
        Rerender(player);
        OnOpen?.Invoke(player);
    }

    public void UseSelection(IPlayer player)
    {
        var selectedOption = Options[SelectedIndex[player]];
        OnItemSelected?.Invoke(player, selectedOption);

        switch (selectedOption)
        {
            case ButtonMenuOption buttonOption:
                {
                    if (buttonOption.ValidationCheck != null && !buttonOption.ValidationCheck(player))
                    {
                        buttonOption.OnValidationFailed?.Invoke(player);
                    }
                    buttonOption.OnClick?.Invoke(player);
                    if (buttonOption.CloseOnSelect)
                    {
                        Close(player);
                    }
                    break;
                }
            case AsyncButtonMenuOption asyncButton:
                {
                    if (asyncButton.ValidationCheck != null && !asyncButton.ValidationCheck(player))
                    {
                        asyncButton.OnValidationFailed?.Invoke(player);
                    }
                    asyncButton.IsLoading = true;
                    asyncButton.SetLoadingText("Processing...");
                    Rerender(player);
                    var closeAfter = asyncButton.CloseOnSelect;
                    Task.Run(async () =>
                    {
                        try
                        {
                            await asyncButton.ExecuteAsync(player, "Processing...");
                        }
                        finally
                        {
                            asyncButton.IsLoading = false;
                            Rerender(player);

                            if (closeAfter && player.IsValid)
                            {
                                Close(player);
                            }
                        }
                    });
                    break;
                }
            case ToggleMenuOption toggle:
                {
                    toggle.Toggle(player);
                    if (toggle.CloseOnSelect)
                    {
                        Close(player);
                    }
                    else
                    {
                        Rerender(player);
                    }
                    break;
                }

            case SubmenuMenuOption submenu:
                var subMenu = submenu.GetSubmenu();
                if (subMenu != null)
                {
                    subMenu.Parent = this;
                    subMenu.Rerender(player);
                    subMenu.Show(player);
                }
                break;
        }
    }

    public void UseSlideOption(IPlayer player, bool isRight)
    {
        var selectedOption = Options[SelectedIndex[player]];

        switch (selectedOption)
        {
            case SliderMenuButton slider:
                if (isRight) slider.Increase(player);
                else slider.Decrease(player);
                break;

            case ChoiceMenuOption choice:
                if (isRight) choice.Next(player);
                else choice.Previous(player);
                break;
        }

        Rerender(player);
    }

    public bool IsCurrentOptionSelectable(IPlayer player)
    {
        var option = Options[SelectedIndex[player]];
        return option is ButtonMenuOption ||
               option is ToggleMenuOption ||
               option is SliderMenuButton ||
               option is SubmenuMenuOption ||
               option is AsyncButtonMenuOption ||
               (option is DynamicMenuOption dynamic && dynamic.CanInteract(null!));
    }

    public void SetFreezeState(IPlayer player, bool freeze)
    {
        if (!player.IsValid || player.IsFakeClient) return;

        var pawn = player.PlayerPawn;
        if (pawn == null || !pawn.IsValid) return;

        var moveType = freeze ? MoveType_t.MOVETYPE_NONE : MoveType_t.MOVETYPE_WALK;
        pawn.MoveType = moveType;
        pawn.ActualMoveType = moveType;
        pawn.MoveTypeUpdated();
    }
}