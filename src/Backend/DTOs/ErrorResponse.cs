namespace Rte.Api.DTOs;

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }
}
