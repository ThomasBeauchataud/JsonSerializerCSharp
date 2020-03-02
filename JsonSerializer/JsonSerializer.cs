using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JsonSerializer
{
    public class JsonSerializer
    {
        public static Dictionary<string, object> Serialize(object data)
        {
            if (data is null)
            {
                return null;
            }
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("attributes", (Dictionary<string, object>)SerializeInternal(data));
            result.Add("class", data.GetType().ToString());
            return result;

        }

        private static object SerializeInternal(object data)
        {
            if (data is null)
            {
                return "null";
            }
            Type dataType = data.GetType();
            if (dataType.IsPrimitive || dataType.Equals(typeof(string)))
            {
                return data;
            }
            Dictionary<string, object> output = new Dictionary<string, object>();
            IEnumerable<FieldInfo> fields = GetAllFields(dataType);
            foreach (FieldInfo field in fields)
            {
                Object fieldValue = field.GetValue(data);
                if (!(fieldValue is String) && (fieldValue is Array || fieldValue is IEnumerable))
                {
                    List<object> dataList = new List<object>();
                    IEnumerator enumerator = ((IEnumerable)fieldValue).GetEnumerator();
                    enumerator.Reset();
                    while (enumerator.MoveNext())
                    {
                        dataList.Add(SerializeInternal(enumerator.Current));
                    }
                    output.Add(field.Name, dataList.ToArray());
                }
                else
                {
                    output.Add(field.Name, SerializeInternal(fieldValue));
                }
            }
            return output;
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            for (Type t = type; t != null; t = t.BaseType)
            {
                fields.AddRange(t.GetRuntimeFields());
            }
            return fields.GroupBy(field => field.Name).Select(group => group.First());
        }
    }
}
