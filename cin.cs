using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
       private static Server ParseNetchUri(string text)
        {
            text = URLSafeBase64Decode(text.Substring(8));

            var NetchLink = JsonSerializer.Deserialize<JsonElement>(text);

            if (string.IsNullOrEmpty(NetchLink.GetProperty("Hostname").GetString()))
                throw new FormatException();

            if (!ushort.TryParse(NetchLink.GetProperty("Port").GetString(), out _))
                throw new FormatException();

            return JsonSerializer.Deserialize<Server>(text,
                new JsonSerializerOptions
                {
                    Converters = { new ServerConverterWithTypeDiscriminator() }
                })!;
        }
}
