namespace IFY.Archimedes.Models.Schema;

/// <summary>
/// A link in a JSON schema entry.
/// </summary>
/// <param name="Type">A choice from <see cref="LinkType"/>.</param>
/// <param name="Text">Optional text for the link.</param>
/// <param name="Reverse">True if the link is reversed (target to source).</param>
public record JsonLink(string? Type, string? Text, bool Reverse);