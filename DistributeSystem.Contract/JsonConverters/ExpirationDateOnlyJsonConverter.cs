using MongoDB.Bson.IO;
using Newtonsoft.Json;
using System.Globalization;

namespace DistributeSystem.Contract.JsonConverters;

public class ExpirationDateOnlyJsonConverter : JsonConverter
{
    private const string Format = "MM/yy";

    public override bool CanConvert(Type objectType)
        => objectType == typeof(DateOnly);

    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object? value, JsonSerializer serializer)
       => writer.WriteValue((value is DateOnly only ? only : default).ToString(Format, CultureInfo.InvariantCulture));


    public override object? ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
       => DateOnly.ParseExact(reader.Value as string ?? string.Empty, Format, CultureInfo.InvariantCulture);
}
