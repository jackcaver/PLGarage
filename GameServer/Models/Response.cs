using System.Xml;
using System.Xml.Serialization;
using GameServer.Utils;

namespace GameServer.Models
{
    public class ResponseStatus
    {
        public int id { get; set; }
        public string message { get; set; }
    }

    [XmlRoot("result")]
    public class Response<T> where T : class
    {
        public ResponseStatus status { get; set; }

        public T response { get; set; }

        public string Serialize() {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            Utf8StringWriter sw = new Utf8StringWriter();
            serializer.Serialize(sw, this, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
            return sw.ToString();
        }
    }
}