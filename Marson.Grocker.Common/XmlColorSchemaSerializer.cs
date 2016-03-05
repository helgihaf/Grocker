using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Marson.Grocker.Common
{
    public class XmlColorSchemaSerializer : IColorSchemaSerializer
    {
        public IEnumerable<ColorSchema> Deserialize(Stream stream)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<ColorSchema>));
            return (List<ColorSchema>)ser.Deserialize(stream);
        }

        public void Serialize(IEnumerable<ColorSchema> colorSchemas, Stream stream)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<ColorSchema>));
            ser.Serialize(stream, colorSchemas.ToList());
        }
    }
}
