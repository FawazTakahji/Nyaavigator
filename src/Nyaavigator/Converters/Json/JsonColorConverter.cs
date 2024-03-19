using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Media;

namespace Nyaavigator.Converters.Json;

public class JsonColorConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (Color.TryParse(reader.GetString(), out Color color))
            return color;

        NLog.LogManager.GetCurrentClassLogger().Error("Color is not valid, using default color.");
        return Colors.Crimson;
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}