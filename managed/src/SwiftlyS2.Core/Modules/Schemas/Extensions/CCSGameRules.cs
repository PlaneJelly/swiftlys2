using SwiftlyS2.Shared.Schemas;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Shared.SchemaDefinitions;

public partial interface CCSGameRules
{
    /// <summary>
    /// Find the player that the controller is targetting
    /// </summary>
    /// <typeparam name="T">Entity Class</typeparam>
    /// <param name="controller">Player Controller</param>
    public T? FindPickerEntity<T>( CBasePlayerController controller ) where T : ISchemaClass<T>;

    /// <summary>
    /// Ends the current round with the specified reason after an optional delay
    /// </summary>
    /// <param name="reason">The reason for ending the round</param>
    /// <param name="delay">The delay before ending the round</param>
    public void TerminateRound( RoundEndReason reason, float delay );

    /// <summary>
    /// Add wins to the Terrorist team
    /// </summary>
    /// <param name="wins">The number of wins to add</param>
    public void AddTerroristWins( short wins );

    /// <summary>
    /// Add wins to the Counter-Terrorist team
    /// </summary>
    /// <param name="wins">The number of wins to add</param>
    public void AddCTWins( short wins );
}