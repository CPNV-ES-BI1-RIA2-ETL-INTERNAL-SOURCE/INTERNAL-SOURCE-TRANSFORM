﻿namespace BusinessTransformer;

/// <summary>
/// Interface to mark exceptions as part of the business transformer domain.
/// </summary>
public interface IBusinessTransformerException { }

/// <summary>
/// Exception thrown when the format of any parameter is invalid. (Like FormatException)
/// </summary>
public class BusinessTransformerFormatException(string message)
    : FormatException(message), IBusinessTransformerException;

/// <summary>
/// Thrown when the input format is invalid based on mapping / schema provided.
/// </summary>
public class InvalidInputFormatException(string message) : BusinessTransformerFormatException(message);