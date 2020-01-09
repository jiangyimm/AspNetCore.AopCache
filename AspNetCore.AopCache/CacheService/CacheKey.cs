using AspNetCore.AopCache.Extensions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AspNetCore.AopCache.CacheService
{
    public class CacheKey
    {
        private MethodInfo Method { get; }
        private ParameterInfo[] InputArguments { get; }
        private object[] ParameterValues { get; }
        public CacheKey(MethodInfo method, ParameterInfo[] arguments, object[] values)
        {
            Method = method;
            InputArguments = arguments;
            ParameterValues = values;
        }

        public override bool Equals(object obj)
        {
            CacheKey another = obj as CacheKey;
            if (null == another)
            {
                return false;
            }
            if (!this.Method.Equals(another.Method))
            {
                return false;
            }
            for (int index = 0; index < this.InputArguments.Length; index++)
            {
                var argument1 = this.InputArguments[index];
                var argument2 = another.InputArguments[index];
                if (argument1 == null && argument2 == null)
                {
                    continue;
                }

                if (argument1 == null || argument2 == null)
                {
                    return false;
                }

                if (!argument1.Equals(argument2))
                {
                    return false;
                }
            }
            return true;
        }

        public string GetRedisCacheKey() => $"{Method.DeclaringType?.Namespace}:{Method.DeclaringType?.Name}:{Method.Name}:{GetRedisKey()}";

        public string GetMemoryCacheKey() => $"{Method.DeclaringType?.Namespace}_{Method.DeclaringType?.Name}_{Method.Name}_{GetHashCode()}";

        public override int GetHashCode()
        {
            var hashCode = Method.GetHashCode();
            try
            {
                foreach (var value in ParameterValues)
                {
                    if (value == null)
                        continue;
                    //todo 此处可将value对象转成json字符串，然后字符串转HashCode
                    if (value.GetType().IsValueType || value is string)
                    {
                        hashCode ^= value.GetHashCode();
                    }
                    else if (value.GetType().IsArray)
                    {
                        var arrValues = ((Array)value).GetArrayValues()?.ToArray();
                        if (arrValues == null || !arrValues.Any())
                            continue;
                        hashCode = arrValues.Aggregate(hashCode, (current, obj) => current ^ obj.GetHashCode());
                    }
                    else
                    {
                        hashCode = value.GetType().GetProperties()
                            .Select(propertyInfo => propertyInfo.GetValue(value) ?? string.Empty)
                            .Aggregate(hashCode, (current, propertyValue) => current ^ propertyValue.GetHashCode());
                    }
                }
                return hashCode;
            }
            catch (Exception ex)
            {
                //log
                Console.WriteLine(ex.Message, ex);
                //避免获取hashCode出错时返回了相同的hashCode
                hashCode ^= new Random().Next(int.MinValue, int.MaxValue);
                hashCode ^= DateTime.Now.GetHashCode();
                return hashCode;
            }
        }

        private string GetRedisKey()
        {
            try
            {
                var json = JsonConvert.SerializeObject(ParameterValues);
                var bytes = Encoding.Default.GetBytes(json);
                var base64 = Convert.ToBase64String(bytes);
                return base64;
            }
            catch (Exception ex)
            {
                //log
                var hashCode = 0;
                Console.WriteLine(ex.Message, ex);
                //避免获取hashCode出错时返回了相同的hashCode
                hashCode ^= new Random().Next(int.MinValue, int.MaxValue);
                hashCode ^= DateTime.Now.GetHashCode();
                return hashCode.ToString();
            }
        }
    }
}
