namespace SoundArcade.Domain;

/// <summary>
/// Identifies a mini-game for registration and UI presentation.
/// </summary>
/// <param name="Id">Stable internal game identifier.</param>
/// <param name="DisplayName">Human-readable game name.</param>
public readonly record struct GameIdentity(string Id, string DisplayName);
