namespace BusinessTransformer;

/// <summary>
/// Interface to mark exceptions as part of the business transformer domain (Marker interface for domain exceptions)
/// Even if it has no members, it is useful to identify exceptions that are part of the domain when catching with exceptions types.
/// Because we want our exception to inherit from the system exceptions, we use a marker interface to identify them (as we can't inherit from multiple classes in C#).
/// See : https://en.wikipedia.org/wiki/Marker_interface_pattern
/// </summary>
public interface IBusinessTransformerException { }

/// <summary>
/// Exception thrown when the format of any parameter is invalid. (Like FormatException)
/// </summary>
public class BusinessTransformerFormatException(string message, Exception? innerException = null)
    : FormatException(message, innerException), IBusinessTransformerException;

/// <summary>
/// Thrown when the input format is invalid based on mapping / schema provided.
/// </summary>
public class BusinessTransformerInvalidInputFormatException(string message, Exception? innerException = null) 
    : BusinessTransformerFormatException(message, innerException);

/// <summary>
/// Thrown when the *mapping* provided is invalid.
/// </summary>
public class BusinessTransformerMappingException(string message, Exception? innerException = null) 
    : BusinessTransformerFormatException(message, innerException);