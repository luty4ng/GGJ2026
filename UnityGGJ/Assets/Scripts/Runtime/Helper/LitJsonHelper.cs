using System;
using GameFramework;
using LitJson;

namespace GameMain
{
    public class LitJsonHelper : Utility.Json.IJsonHelper
    {
        public string ToJson(object obj)
        {
            return JsonMapper.ToJson(obj);
        }

        public T ToObject<T>(string json)
        {
            return JsonMapper.ToObject<T>(json);
        }

        public object ToObject(Type objectType, string json)
        {
            return JsonMapper.ToObject(json, objectType);
        }
    }
}