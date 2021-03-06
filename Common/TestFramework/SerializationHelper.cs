﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

namespace TestFramework
{
    public class SerializationHelper
    {
        public static void Dump(string fullFilePath, object obj)
        {
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }
            using (var s = new FileStream(fullFilePath, FileMode.CreateNew, FileAccess.Write))
            {
                var bf = new BinaryFormatter();

                bf.Serialize(s, obj);
            }
        }

        public static T Load<T>(string fullFilePath)
        {
            T t;
            using (var s = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read))
            {
                var bf = new BinaryFormatter();
                t = (T) bf.Deserialize(s);
            }
            return t;
        }

        public static void DumpToXml<T>(string fullFilePath, T t)
        {
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }
            using (var s = new FileStream(fullFilePath, FileMode.CreateNew, FileAccess.Write))
            {
                var ser = new XmlSerializer(typeof (T));

                ser.Serialize(s, t);
            }
        }

        public static void DumpToXml<T>(TextWriter writer, T @object)
        {
            var serializer = new XmlSerializer(typeof (T));
            serializer.Serialize(writer, @object);
        }

        public static void DumpToJson<T>(Stream stream, T obj)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(stream, obj);
        }

        
        public static T LoadFromJson<T>(string fullFilePath)
        {
            T sceneDescription;
            
            using (var s = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read))
            {
                var settingsSerializer = new DataContractJsonSerializer(typeof(T));
                sceneDescription = (T)settingsSerializer.ReadObject(s);
            }
            return sceneDescription;
        }

        public static T LoadFromXml<T>(string fullFilePath)
        {
            T t;
            using (var s = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read))
            {
                var ser = new XmlSerializer(typeof (T));
                t = (T) ser.Deserialize(s);
            }
            return t;
        }

        public static T LoadFromXml<T>(TextReader reader)
        {
            var serializer = new XmlSerializer(typeof (T));
            return (T) serializer.Deserialize(reader);
        }
    }
}