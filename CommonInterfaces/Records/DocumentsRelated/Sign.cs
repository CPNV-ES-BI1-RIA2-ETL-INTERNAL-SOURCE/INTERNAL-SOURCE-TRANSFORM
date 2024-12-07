namespace CommonInterfaces.DocumentsRelated;

/// <summary>
/// A structure that represents a sign (special character / logo) with its description.
/// </summary>
/// <param name="SignId">The unique identifier of the sign. (Ex. S, 1)</param>
/// <param name="SignDescription">The description of the sign. (Ex. RER, VELOS: Réservation obligatoire)</param>
public record Sign(string SignId, string SignDescription);