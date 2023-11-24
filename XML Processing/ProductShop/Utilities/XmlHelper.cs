﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.Utilities
{
    public class XmlHelper
    {
        public T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute rootAttribute = new XmlRootAttribute(rootName);

            XmlSerializer serializer = new XmlSerializer(typeof(T), rootAttribute);

            using StringReader stringReader = new StringReader(inputXml);

            return (T)serializer.Deserialize(stringReader)!;
        }

        public T Deserialize<T>(string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using StringReader stringReader = new StringReader(inputXml);

            return (T)serializer.Deserialize(stringReader)!;
        }

        public string Serialize<T>(T obj, string rootName)
        {
            StringBuilder sb = new();

            XmlRootAttribute xmlRoot =
                new(rootName);

            XmlSerializer serializer =
                new(typeof(T), xmlRoot);

            XmlSerializerNamespaces serializerNamespaces =
                new();

            serializerNamespaces
                .Add(string.Empty, string.Empty);

            using StringWriter writer =
                new(sb);

            serializer
                .Serialize(writer, obj, serializerNamespaces);

            return sb
                .ToString()
                .TrimEnd();
        }

        public string Serialize<T>(T[] obj, string rootName)
        {
            StringBuilder sb = new();

            XmlRootAttribute xmlRoot =
                new(rootName);

            XmlSerializer serializer =
                new(typeof(T[]), xmlRoot);

            XmlSerializerNamespaces serializerNamespaces =
                new();

            serializerNamespaces
                .Add(string.Empty, string.Empty);

            using StringWriter writer =
                new(sb);

            serializer
                .Serialize(writer, obj, serializerNamespaces);

            return sb
                .ToString()
                .TrimEnd();
        }
    }
}
