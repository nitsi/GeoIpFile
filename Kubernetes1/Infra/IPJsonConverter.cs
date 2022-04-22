using System;
using System.Net;
using Newtonsoft.Json;

namespace Kubernetes1.Infra
{
    /// <summary>
    /// <see cref="JsonConvert"/> for <see cref="IPAddress"/> data.
    /// </summary>
    public class IPJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return IPAddress.Parse((string)reader.Value);
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPAddress));
        }

        public override bool CanWrite => false;
    }
}