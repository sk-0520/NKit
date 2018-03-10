using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ContentTypeTextNet.NKit.Utility.Model
{
    public abstract class SerializerBase
    {
        #region property

        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public int BufferSize { get; set; } = 4 * 1024;

        #endregion

        #region function

        protected TextReader GetReader(Stream stream) => new StreamReader(stream, Encoding, true, BufferSize, true);
        protected TextWriter GetWriter(Stream stream) => new StreamWriter(stream, Encoding, BufferSize, true);

        /// <summary>
        /// オブジェクトを複製する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">複製したいオブジェクト。</param>
        /// <returns></returns>
        public T Clone<T>(T source)
        {
            if(source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            using(var stream = new MemoryStream()) {
                Save(source, stream);
                stream.Position = 0;
                return Load<T>(stream);
            }
        }

        public abstract TResult Load<TResult>(Stream stream);
        public abstract void Save(object value, Stream stream);

        #endregion
    }

    public class JsonSerializer : SerializerBase
    {
        #region SerializerBase

        public override TResult Load<TResult>(Stream stream)
        {
            using(var reader = GetReader(stream))
            using(var jsonReader = new Newtonsoft.Json.JsonTextReader(reader)) {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                return serializer.Deserialize<TResult>(jsonReader);
            }
        }

        public override void Save(object value, Stream stream)
        {
            using(var writer = GetWriter(stream))
            using(var jsonWriter = new Newtonsoft.Json.JsonTextWriter(writer)) {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.Serialize(jsonWriter, value);
            }
        }

        #endregion
    }

    public abstract class XmlSerializerBase : SerializerBase
    {
        #region function

        protected XmlReaderSettings CreateXmlReaderSettings()
        {
            return new XmlReaderSettings() {
                CloseInput = false,
            };
        }

        protected XmlWriterSettings CreateXmlWriterSettings()
        {
            return new XmlWriterSettings() {
                CloseOutput = false,
                NewLineHandling = NewLineHandling.Entitize,
            };
        }

        #endregion
    }

    public class XmlSerializer : XmlSerializerBase
    {
        #region XmlSerializerBase

        public override TResult Load<TResult>(Stream stream)
        {
            using(var reader = XmlReader.Create(stream, CreateXmlReaderSettings())) {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(TResult));
                return (TResult)serializer.Deserialize(reader);
            }
        }

        public override void Save(object value, Stream stream)
        {
            using(var writer = XmlWriter.Create(stream, CreateXmlWriterSettings())) {
                var serializer = new System.Xml.Serialization.XmlSerializer(value.GetType());
                serializer.Serialize(writer, value);
            }
        }

        #endregion
    }

    public abstract class DataContractSerializerBase : XmlSerializerBase
    { }

    public class XmlDataContractSerializer : DataContractSerializerBase
    {
        #region DataContractSerializerBase

        public override TResult Load<TResult>(Stream stream)
        {
            using(var reader = XmlReader.Create(stream, CreateXmlReaderSettings())) {
                var serializer = new DataContractSerializer(typeof(TResult));
                return (TResult)serializer.ReadObject(reader);
            }
        }

        public override void Save(object value, Stream stream)
        {
            using(var writer = XmlWriter.Create(stream, CreateXmlWriterSettings())) {
                var serializer = new DataContractSerializer(value.GetType());
                serializer.WriteObject(writer, value);
            }
        }

        #endregion
    }

    public class BinaryDataContractSerializer : DataContractSerializerBase
    {
        #region DataContractSerializerBase

        public override TResult Load<TResult>(Stream stream)
        {
            // 閉じない方法がわっからん
            var quotas = new XmlDictionaryReaderQuotas();
            using(var reader = XmlDictionaryReader.CreateBinaryReader(stream, quotas)) {
                var serializer = new DataContractSerializer(typeof(TResult));
                return (TResult)serializer.ReadObject(reader);
            }
        }

        public override void Save(object value, Stream stream)
        {
            using(var writer = XmlDictionaryWriter.CreateBinaryWriter(stream, null, null, false)) {
                var serializer = new DataContractSerializer(value.GetType());
                serializer.WriteObject(writer, value);
            }
        }

        #endregion
    }

#if ENABLED_MessagePack
    public class MessagePackSerializer : SerializerBase
    {
    #region SerializerBase

        public override TResult Load<TResult>(Stream stream)
        {
            return MessagePack.MessagePackSerializer.Deserialize<TResult>(stream);
        }

        public override void Save(object value, Stream stream)
        {
            MessagePack.MessagePackSerializer.Serialize(stream, value);
        }

    #endregion
    }
#endif
}
