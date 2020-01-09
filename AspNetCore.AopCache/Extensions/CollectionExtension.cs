using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.AopCache.Extensions
{
    public static class CollectionExtension
    {
        public static IEnumerable<object> GetArrayValues(this Array array)
        {
            var list = new List<object>();
            if (array == null || array.Length == 0)
                return list;
            list.AddRange(array.Cast<object>().Select((t, index) => array.GetValue(index)));
            return list;
        }
    }
}
