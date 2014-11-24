using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SocialCommentaryApi.Service.Helpers
{
    public static class ObjectExtensions
    {
        public static byte[] BinarySerialize(this object obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);

                ms.Position = 0;
                return ms.ToArray(); // here's your data!
            }
        }

        public static object BinaryDeserialize(this byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }

        public static string JsonSerialize<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T JsonDeserialize<T>(this string jsonData)
        {
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        public static string XmlSerialize<T>(this T obj, Encoding encoding)
        {
            using (var memoryStream = new MemoryStream())
            {
                var xmlWriterSettings = new XmlWriterSettings { Encoding = encoding };
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(xmlWriter, obj);
                memoryStream.Seek(0, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(memoryStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public static string XmlSerialize<T>(this T obj)
        {
            return XmlSerialize(obj, Encoding.UTF8);
        }

        public static object XmlDeserialize(this string xml, Type returnType)
        {
            using (var stringReader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(returnType);
                return serializer.Deserialize(XmlReader.Create(stringReader));
            }
        }

        public static bool XmlDeserialize<T>(this string xml, out T obj, out Exception exception)
        {
            exception = null;
            obj = default(T);
            try
            {
                obj = XmlDeserialize<T>(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool XmlDeserialize<T>(this string xml, out T obj)
        {
            Exception exception;
            return XmlDeserialize(xml, out obj, out exception);
        }

        public static T XmlDeserialize<T>(this string xml)
        {
            return (T)XmlDeserialize(xml, typeof(T));
        }

        public static bool SaveXmlToFile<T>(this T obj, string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveXmlToFile(obj, fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public static bool SaveXmlToFile<T>(this T obj, string fileName, out Exception exception)
        {
            return SaveXmlToFile(obj, fileName, Encoding.UTF8, out exception);
        }

        public static void SaveXmlToFile<T>(this T obj, string fileName, Encoding encoding)
        {
            using (var streamWriter = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                var xmlString = XmlSerialize(obj, encoding);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
        }

        public static void SaveXmlToFile<T>(this T obj, string fileName)
        {
            SaveXmlToFile(obj, fileName, Encoding.UTF8);
        }

        public static bool LoadXmlFromFile<T>(string fileName, Encoding encoding, out T obj, out Exception exception)
        {
            exception = null;
            obj = default(T);
            try
            {
                obj = LoadFromFile<T>(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadXmlFromFile<T>(string fileName, out T obj, out Exception exception)
        {
            return LoadXmlFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile<T>(string fileName, out T obj)
        {
            Exception exception;
            return LoadXmlFromFile(fileName, out obj, out exception);
        }

        public static T LoadFromFile<T>(string fileName, Encoding encoding)
        {
            using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(file, encoding))
            {
                var xmlString = sr.ReadToEnd();
                return XmlDeserialize<T>(xmlString);
            }
        }

        public static T LoadFromFile<T>(string fileName)
        {
            return LoadFromFile<T>(fileName, Encoding.UTF8);
        }
    }
}