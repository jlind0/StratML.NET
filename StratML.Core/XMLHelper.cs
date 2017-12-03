using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Data.SqlTypes;

namespace StratML.Core
{
    public static class XmlHelper
    {
        public static SqlXml ConvertToSqlXml<T>(this T obj)
        {
            return ConvertToSqlXml(obj, typeof(T));
        }
        public static SqlXml ConvertToSqlXml(this object obj, Type type)
        {
            return ConvertToSqlXml(Serialize(obj, type));
        }
        public static string Serialize<T>(this T obj)
        {
            return Serialize(obj, typeof(T));
        }
        public static string Serialize(this object obj, Type type)
        {
            XmlSerializer xSerializer = new XmlSerializer(type);
            MemoryStream ms = new MemoryStream();
            xSerializer.Serialize(ms, obj);
            return UTF8ByteArrayToString(ms.ToArray());
        }
        public static T Deserialize<T>(this string xml)
            where T : new()
        {
            return (T)Deserialize(xml, typeof(T));
        }
        public static object Deserialize(this string xml, Type type)
        {
            XmlSerializer xSerializer = new XmlSerializer(type);
            MemoryStream ms = new MemoryStream(StringToUTF8ByteArray(xml));
            return xSerializer.Deserialize(ms);
        }
        /// <summary>
        /// Converts a string to a SQLXml object
        /// </summary>
        /// <param name="xml">target string</param>
        /// <returns>populated SqlXml object</returns>
        public static SqlXml ConvertToSqlXml(this string xml)
        {
            SqlXml s = new SqlXml();
            MemoryStream stream = new MemoryStream(StringToUTF8ByteArray(xml));
            return new SqlXml(stream);
        }
        /// <summary>
        /// Converts a string to a byte[] array for deserializtion
        /// </summary>
        /// <param name="pXmlString">target string</param>
        /// <returns>corresponding byte arrary</returns>
        private static byte[] StringToUTF8ByteArray(string pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }
        /// <summary>
        /// Converts a byte array to a string for serialization
        /// </summary>
        /// <param name="characters">byte arrary</param>
        /// <returns>corresponding string</returns>
        private static string UTF8ByteArrayToString(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters, 0, characters.Length);
            return (constructedString);

        }
    }
}