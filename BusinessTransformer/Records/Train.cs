namespace BusinessTransformer.Records;

/// <summary>
/// Represents a train with associated identifiers.
/// </summary>
/// <param name="G">Group / category of the train.</param>
/// <param name="L">Line the train operates on.</param>
public record Train(string G, string L);