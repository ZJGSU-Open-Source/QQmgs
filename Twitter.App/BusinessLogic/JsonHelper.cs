using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Twitter.App.Common;

namespace Twitter.App.BusinessLogic
{
    public static class JsonHelper
    {
        public static string ToJsonString(object value)
        {
            Guard.ArgumentNotNull(value, nameof(value));

            // Note that when JsonConvert simple types, quotes will be added to the value
            return JsonConvert.SerializeObject(value, new StringEnumConverter());
        }

        public static T FromJsonString<T>(string value)
        {
            Guard.ArgumentNotNull(value, nameof(value));

            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (JsonReaderException jre)
            {
                throw new ArgumentException("Invalid json string", jre);
            }
            catch (JsonSerializationException jse)
            {
                throw new ArgumentException($"Invalid json string for type {typeof(T)}.", jse);
            }
        }

        public static List<Photo> LoadJson(string jsonDestination)
        {
            using (var r = new StreamReader(jsonDestination))
            {
                var json = r.ReadToEnd();

                return FromJsonString<List<Photo>>(json);
            }
        }

        public class Photo
        {
            public string Url;

            public int Width;

            public int Height;
        }
    }
}