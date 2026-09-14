using System.Text.Json;
using System.Text.Json.Serialization;

namespace FootballGm.Api.Utility;

public sealed class CamelCaseEnumConverter<T>()
    : JsonStringEnumConverter<T>(JsonNamingPolicy.CamelCase)
    where T : struct, Enum;
