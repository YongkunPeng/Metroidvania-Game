using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace My.SaveSystem
{
    /*
        在默认情况下，JSON 序列化只会序列化引用对象的引用，而不会深入序列化整个对象的结构。因此，当你将一个类的实例用作字典的键时，它会默认将其序列化为对象引用，而不会序列化该对象的内部详细信息。

        然而，当你将一个类的实例用作字典的值时，JSON 序列化器需要将整个对象的结构序列化，包括其所有字段和属性的值，以确保可以准确地重新构建该对象。

        因此，当Items作为值序列化时，其内部结构也会一一序列化，而Items中的Sprite对象包括Vector3和Vector2类型的变量，因此若不忽略循环引用和自定义Vector3和Vector2方式，会出现报错
     */
    public static class AdditionalJSONConvert
    {
        public static void AddAllConvert()
        {
            AddVector3Convert();
        }

        private static void AddVector3Convert()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // 忽略
                Converters = { new Vector3Convert(), new Vector2Convert() }
            };
        }
    }

    public class Vector3Convert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Vector3);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var x = (float)obj["x"];
            var y = (float)obj["y"];
            var z = (float)obj["z"];
            return new Vector3(x, y, z);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector = (Vector3)value;
            var obj = new JObject()
            {
                {"x", vector.x},
                {"y", vector.y},
                {"z", vector.z},
            };
            obj.WriteTo(writer);
        }
    }

    public class Vector2Convert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Vector3);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var x = (float)obj["x"];
            var y = (float)obj["y"];
            return new Vector2(x, y);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector = (Vector3)value;
            var obj = new JObject()
            {
                {"x", vector.x},
                {"y", vector.y},
            };
            obj.WriteTo(writer);
        }
    }
}
