using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.Common
{
    public interface IColorSchemaSerializer
    {
        void Serialize(IEnumerable<ColorSchema> colorSchemas, Stream stream);
        IEnumerable<ColorSchema> Deserialize(Stream stream);
    }
}
