using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnlineStore.Application.Abstractions.Messaging;

public static class QueueMessageJsonOptions
{
    public static JsonSerializerOptions Create()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
        return options;
    }
}
