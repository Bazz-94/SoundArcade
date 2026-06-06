namespace SoundArcade.Domain;

/// <summary>
/// Defines the minimal contract implemented by every mini-game module.
/// </summary>
public interface IGame
{
  /// <summary>
  /// Gets the unique game identity.
  /// </summary>
  GameIdentity Identity { get; }
}
