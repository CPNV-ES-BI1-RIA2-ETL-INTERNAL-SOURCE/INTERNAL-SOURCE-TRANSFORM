namespace RestAPI.DTOs;

/// <summary>
/// Request DTO to encapsulate document and mapping.
/// </summary>
public class TransformRequest
{
    public List<string> Document { get; set; } = [];
    public string MappingInJson { get; set; }
}