namespace IFY.Archimedes.Models.Schema;

/// <summary>
/// A link in a JSON schema entry.
/// </summary>
/// <param name="Type">A choice from <see cref="LinkType"/>.</param>
/// <param name="Text">Optional text for the link.</param>
public record JsonLink(string? Type, string? Text);