using System;
using System.Collections.Generic;
using System.Json;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NSoft.NFramework.Web.Mvc
{
    public static class MvcTool
    {
        public const string JsonContentType = "appliccation/json";

        /// <summary>
        /// Json 직렬화의 기본 설정
        /// </summary>
        public static JsonSerializerSettings DefaultJsonSerializerSettings =
            new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                // 이걸 해줘야 struct 등이 제대로 deserialize 됩니다.
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                TypeNameHandling = TypeNameHandling.None,
                Converters = new List<JsonConverter>
                             {
                                 new IsoDateTimeConverter(),
                                 new StringEnumConverter()
                             }
            };

        public static JsonSerializer CreateJsonSerializer()
        {
            var serializer = new JsonSerializer()
                             {
                                 ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                                 MissingMemberHandling = MissingMemberHandling.Ignore,
                                 NullValueHandling = NullValueHandling.Include,
                                 ObjectCreationHandling = ObjectCreationHandling.Replace,
                                 // 이걸 해줘야 struct 등이 제대로 deserialize 됩니다.
                                 PreserveReferencesHandling = PreserveReferencesHandling.All,
                                 TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                                 TypeNameHandling = TypeNameHandling.None,
                             };

            foreach(var converter in DefaultJsonSerializerSettings.Converters)
                serializer.Converters.Add(converter);

            return serializer;
        }

        public static bool IsNotWriteType(Type type)
        {
            // don't serialize JsonValue structure use default for that
            return type == typeof(JsonValue) ||
                   type == typeof(JsonObject) ||
                   type == typeof(JsonArray);
        }
    }
}