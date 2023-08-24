using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace My.SaveSystem
{
    /*
        ��Ĭ������£�JSON ���л�ֻ�����л����ö�������ã��������������л���������Ľṹ����ˣ����㽫һ�����ʵ�������ֵ�ļ�ʱ������Ĭ�Ͻ������л�Ϊ�������ã����������л��ö�����ڲ���ϸ��Ϣ��

        Ȼ�������㽫һ�����ʵ�������ֵ��ֵʱ��JSON ���л�����Ҫ����������Ľṹ���л��������������ֶκ����Ե�ֵ����ȷ������׼ȷ�����¹����ö���

        ��ˣ���Items��Ϊֵ���л�ʱ�����ڲ��ṹҲ��һһ���л�����Items�е�Sprite�������Vector3��Vector2���͵ı����������������ѭ�����ú��Զ���Vector3��Vector2��ʽ������ֱ���
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
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // ����
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
