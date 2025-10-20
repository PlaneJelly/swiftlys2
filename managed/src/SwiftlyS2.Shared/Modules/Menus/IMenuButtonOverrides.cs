using SwiftlyS2.Shared.Events;

namespace SwiftlyS2.Shared.Menus;

public interface IMenuButtonOverrides
{
    public KeyKind? Select { get; set; }
    public KeyKind? Move { get; set; }
    public KeyKind? Exit { get; set; }
}