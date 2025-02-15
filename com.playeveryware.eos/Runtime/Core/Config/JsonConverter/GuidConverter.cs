/*
* Copyright (c) 2024 PlayEveryWare
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

namespace PlayEveryWare.EpicOnlineServices
{
    using Newtonsoft.Json;
    using System;

    public class GuidConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Guid guid)
            {
                // Serialize Guid as a lowercase string without dashes
                writer.WriteValue(guid.ToString("N").ToLowerInvariant());
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var stringValue = reader.Value?.ToString();

            if (string.IsNullOrEmpty(stringValue))
            {
                return objectType == typeof(Guid?) ? (Guid?)null : Guid.Empty;
            }

            if (!Guid.TryParseExact(stringValue, "N", out Guid value))
            {
                if (!Guid.TryParse(stringValue, out value))
                {
                    value = Guid.Empty;
                }
            }

            // Deserialize the Guid from the string
            return value;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Guid) || objectType == typeof(Guid?);
        }
    }
}