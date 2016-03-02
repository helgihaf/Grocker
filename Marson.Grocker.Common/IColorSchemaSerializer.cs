using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.Common
{
    public interface IColorSchemaSerializer<T>
    {
        void Serialize(IEnumerable<ColorSchema<T>> colorSchemas, Stream stream);
        IEnumerable<ColorSchema<T>> Deserialize(Stream stream);
    }
}
