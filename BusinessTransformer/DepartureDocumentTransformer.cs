using BusinessTransformer.Records;
using CommonInterfaces.DocumentsRelated;

namespace BusinessTransformer;

/// <summary>
/// A class that transforms a DeparturesDocument into a TrainStation.
/// </summary>
public class DepartureDocumentTransformer : IDocumentTransformer<DeparturesDocument, TrainStation>
{
    /// <inheritdoc/>
    public TrainStation Transform(DeparturesDocument input)
    {
        throw new NotImplementedException();
    }
}