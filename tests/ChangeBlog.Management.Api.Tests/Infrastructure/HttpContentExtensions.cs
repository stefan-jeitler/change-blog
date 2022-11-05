using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChangeBlog.Management.Api.Tests.Infrastructure;

public static class HttpContentExtensions
{
    public static Task<T> Deserialize<T>(this HttpContent content) =>
        content.ReadFromJsonAsync<T>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = {new JsonStringEnumConverter()}
        });
}