using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Marson.Grocker.Common
{
    public class XmlColorSchemaSerializer<T> : IColorSchemaSerializer<T>
    {
        public IEnumerable<ColorSchema<T>> Deserialize(Stream stream)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<ColorSchema<T>>));
            return (List<ColorSchema<T>>)ser.Deserialize(stream);
        }

        public void Serialize(IEnumerable<ColorSchema<T>> colorSchemas, Stream stream)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<ColorSchema<T>>));
            ser.Serialize(stream, colorSchemas.ToList());
        }
    }
}
