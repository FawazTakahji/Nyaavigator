using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NLog;

namespace Nyaavigator.Converters.Json;

public class JsonLogEventInfoConverter : JsonConverter<LogEventInfo>
{
    public override void Write(Utf8JsonWriter writer, LogEventInfo value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("Time", value.TimeStamp.ToString("h:mm tt"));

        writer.WriteString("Level", value.Level.Name);

        if (value.FormattedMessage.Length > 0)
            writer.WriteString("Message", value.FormattedMessage);

        if (value.Exception != null)
            writer.WriteString("Exception", value.Exception.ToString());

        if (value.LoggerName != null)
            writer.WriteString("Logger", value.LoggerName);

        writer.WriteEndObject();
    }

    public override LogEventInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}