using SwiftlyS2.Shared.Menus;

namespace SwiftlyS2.Core.Menus.OptionsBase;

/// <summary>
/// Represents a simple text-only menu option without interactive behavior.
/// </summary>
public sealed class TextMenuOption : MenuOptionBase
{
    /// <summary>
    /// Creates an instance of <see cref="TextMenuOption"/> with dynamic text updating capabilities.
    /// </summary>
    /// <param name="text">The text content to display.</param>
    /// <param name="maxWidth">The maximum display width for menu option text in relative units. Defaults to 26.</param>
    /// <param name="textSize">The size of the text. Defaults to <see cref="MenuOptionTextSize.Medium"/>.</param>
    /// <param name="textStyle">The text overflow style. Defaults to <see cref="MenuOptionTextStyle.TruncateEnd"/>.</param>
    /// <param name="updateIntervalMs">The interval in milliseconds between text updates. Defaults to 120ms.</param>
    /// <param name="pauseIntervalMs">The pause duration in milliseconds before starting the next text update cycle. Defaults to 1000ms.</param>
    public TextMenuOption(
        string text,
        float maxWidth = 26f,
        MenuOptionTextSize textSize = MenuOptionTextSize.Medium,
        MenuOptionTextStyle textStyle = MenuOptionTextStyle.TruncateEnd,
        int updateIntervalMs = 120,
        int pauseIntervalMs = 1000 ) : base(updateIntervalMs, pauseIntervalMs)
    {
        Text = text;
        MaxWidth = maxWidth;
        TextSize = textSize;
        TextStyle = textStyle;
        PlaySound = false;
    }
}