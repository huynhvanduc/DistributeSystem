using Newtonsoft.Json;
using System.Globalization;

namespace DistributeSystem.Contract.JsonConverters;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private readonly string Format = "dd/MM/yyyy";

    public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return DateOnly.ParseExact(reader.Value as string ?? string.Empty, Format, CultureInfo.InvariantCulture);
    }

    public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}
