using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

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

        public string Serialize()
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            serializer.Serialize(writer, this, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
            return Encoding.UTF8.GetString(stream.ToArray().Skip(3).ToArray());
        }

        public void Deserialize(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            var deserialized = serializer.Deserialize(stream) as Response<T>;
            if (deserialized != null)
            {
                status = deserialized.status;
                response = deserialized.response;
            }
        }
    }
}