using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

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
            Type dataType = data.GetType();
            if (dataType.IsPrimitive)
            {
                return data;
            }
            Dictionary<string, object> output = new Dictionary<string, object>();
            IEnumerable<FieldInfo> fields = dataType.GetRuntimeFields();
            foreach (FieldInfo field in fields)
            {
                if (field.GetValue(data) is Array || field.GetValue(data) is IEnumerable)
                {
                    List<object> dataList = new List<object>();
                    IEnumerator enumerator = ((IEnumerable)field.GetValue(data)).GetEnumerator();
                    enumerator.Reset();
                    while (enumerator.MoveNext())
                    {
                        dataList.Add(SerializeInternal(enumerator.Current));
                    }
                    output.Add(field.Name, dataList.ToArray());
                }
                output.Add(field.Name, SerializeInternal(field.GetValue(data)));
            }
            return output;
        }
    }
}
