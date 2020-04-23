using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Text;
using Newtonsoft.Json;

namespace CustomExtensions.UrlExtensions
{
    public static class RequestUrlExtensions
    {
        /// <summary>
        ///     Is numeric Type.
        /// </summary>
        private static bool IsNumericType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type == typeof(int) || type == typeof(double) || type == typeof(double) || type == typeof(decimal)
                   || type == typeof(decimal) || type == typeof(long) || type == typeof(short) || type == typeof(float)
                   || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(uint)
                   || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong) || type == typeof(sbyte)
                   || type == typeof(float);
        }

        /// <summary>
        ///     Is string Type.
        /// </summary>
        private static bool IsStringType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type == typeof(string);
        }

        /// <summary>
        ///     Converts NameValueCollection to Dictionary.
        /// </summary>
        public static Dictionary<string, object> NvcToDictionary(
            this NameValueCollection nvc,
            bool handleMultipleValuesPerKey = false)
        {
            var result = new Dictionary<string, object>();
            foreach (string key in nvc.Keys)
            {
                if (!handleMultipleValuesPerKey)
                {
                    result.Add(key, nvc[key]);
                    continue;
                }

                var values = nvc.GetValues(key);
                if (values == null)
                {
                    continue;
                }

                if (values.Length == 1)
                {
                    result.Add(key, values[0]);
                }
                else
                {
                    result.Add(key, values);
                }
            }

            return result;
        }

        /// <summary>
        ///     Converts NameValueCollection to dynamic.
        /// </summary>
        public static dynamic ToDynamic(this NameValueCollection valueCollection)
        {
            var result = new ExpandoObject() as IDictionary<string, object>;
            foreach (var key in valueCollection.AllKeys)
            {
                result.Add(key, valueCollection[key]);
            }

            return result;
        }

        /// <summary>
        ///     Converts specified type to NameValueColllection.
        /// </summary>
        public static NameValueCollection ToNameValueCollection<T>(this T source)
        {
            var jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var nameValueCollection = new NameValueCollection();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(source))
            {
                var obj = propertyDescriptor.GetValue(source);
                if (obj == null)
                {
                    continue;
                }

                string value;
                if (!obj.GetType().IsPrimitive &&
                    !obj.GetType().IsStringType() &&
                    !obj.GetType().IsValueType &&
                    !obj.GetType().IsNumericType())
                {
                    value = JsonConvert.SerializeObject(obj, jsonSerializerSettings);
                }
                else
                {
                    value = obj.ToString();
                }

                nameValueCollection.Add(propertyDescriptor.Name, value);
            }

            return nameValueCollection;
        }

        /// <summary>
        ///     Converts dictionary to query string.
        /// </summary>
        public static string ToQueryString(this IDictionary<string, object> dict)
        {
            var sb = new StringBuilder();

            foreach (var (key, value) in dict)
            {
                sb.Append(sb.Length == 0 ? "?" : "&");
                sb.AppendFormat($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value.ToString())}");
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Converts NameValueCollection to query string.
        /// </summary>
        public static string ToQueryString(this NameValueCollection nvc)
        {
            var sb = new StringBuilder();

            foreach (string key in nvc.Keys)
            {
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                var values = nvc.GetValues(key);
                if (values == null)
                {
                    continue;
                }

                foreach (var value in values)
                {
                    sb.Append(sb.Length == 0 ? "?" : "&");
                    sb.AppendFormat($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
                }
            }

            return sb.ToString();
        }
    }
}
