using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AssetStudio.Metas
{
    public class MetaObject
    {
        public int fileFormatVersion;
        public string guid;

        public MetaObject()
        {
            fileFormatVersion = 2;
            guid = Guid.NewGuid().ToString("N");
        }
        public override string ToString()
        {
            return CreateSerializerBuilder().Build().Serialize(this);
        }

        protected virtual SerializerBuilder CreateSerializerBuilder()
        {
            return new SerializerBuilder();
        }
    }
}
