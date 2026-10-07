using System.Text.Json.Serialization;

namespace TablexAPI.Models;

public class ApiError
{
    public int StatusCode { get; set; }
    public string Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; set; }

    public ApiError(int statusCode, string message, IDictionary<string, string[]>? errors = null)
    {
        StatusCode = statusCode;
        Message = message;
        Errors = errors;
    }
}
