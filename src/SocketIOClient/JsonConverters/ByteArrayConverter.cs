﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocketIOClient.JsonConverters
{
    public class ByteArrayConverter : JsonConverter
    {
        public SocketIO Client { get; set; }
        public IList<byte[]> BinaryBytes { get; set; }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(byte[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            byte[] bytes = null;
            if (reader.TokenType == JsonToken.StartObject)
            {
                reader.Read();
                if (reader.TokenType == JsonToken.PropertyName && reader.Value?.ToString() == "_placeholder")
                {
                    reader.Read();
                    if (reader.TokenType == JsonToken.Boolean && (bool)reader.Value)
                    {
                        reader.Read();
                        if (reader.TokenType == JsonToken.PropertyName && reader.Value?.ToString() == "num")
                        {
                            reader.Read();
                            if (reader.Value != null)
                            {
                                if (int.TryParse(reader.Value.ToString(), out int num))
                                {
                                    bytes = BinaryBytes[num];
                                    reader.Read();
                                }
                            }
                        }
                    }
                }
            }
            return bytes;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var source = (value as byte[]).ToList();
            if (Client.Options.EIO != 4)
                source.Insert(0, 4);
            BinaryBytes.Add(source.ToArray());
            writer.WriteStartObject();
            writer.WritePropertyName("_placeholder");
            writer.WriteValue(true);
            writer.WritePropertyName("num");
            writer.WriteValue(BinaryBytes.Count - 1);
            writer.WriteEndObject();
        }
    }
}
