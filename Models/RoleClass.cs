using System.Text.Json.Serialization;

namespace csharp.Models
{   
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RoleClass
    {
        User = 1,
        Admin = 2
    }
}