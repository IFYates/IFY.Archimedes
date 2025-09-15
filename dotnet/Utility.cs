using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;

namespace IFY.Archimedes;

internal static class Utility
{
    public static JsonSerializerOptions GlobalJsonOptions { get; } = new(JsonSerializerOptions.Default)
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        PropertyNameCaseInsensitive = true,
        Converters = { new BooleanStringJsonConverter() }
    };
    private sealed class BooleanStringJsonConverter : System.Text.Json.Serialization.JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return bool.TryParse(reader.GetString(), out var result)
                    ? result
                    : throw new JsonException($"Invalid boolean string: {reader.GetString()}");
            }
            return reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                _ => throw new JsonException($"Unexpected token parsing boolean. Token: {reader.TokenType}"),
            };
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }

    public static T? DeserializeJson<T>(this string json)
        => JsonSerializer.Deserialize<T>(json, GlobalJsonOptions);
    public static T? Deserialize<T>(this JsonElement element)
        => JsonSerializer.Deserialize<T>(element, GlobalJsonOptions);

    public static string GetEnumMemberValue<T>(this T enumValue)
        where T : Enum
    {
        var value = enumValue.ToString();
        return typeof(T)
            .GetMember(value).FirstOrDefault()?
            .GetCustomAttribute<EnumMemberAttribute>()?.Value
            ?? value;
    }

    /// <summary>
    /// HTML encodes the string.
    /// </summary>
    [return: NotNullIfNotNull(nameof(value))]
    public static string? HtmlEncode(this string? value)
    {
        return WebUtility.HtmlEncode(value)?
            .Replace("&#", "#");
    }
}
