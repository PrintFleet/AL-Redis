using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using System.IO;

namespace AngiesList.Redis
{
	public class SSJsonSerializer : IValueSerializer
	{
        public class CacheContainer
        {
            public Type Type { get; set; }
            public string Value { get; set; }
        }

        //public class CacheContainerTyped<T>
        //{
        //    public T Value { get; set; }
        //}

		public byte[] Serialize(object value)
		{
            CacheContainer item = null;
            if (value != null)
            {
                item = new CacheContainer()
                {
                    Type = value.GetType(),
                    Value = JsonSerializer.SerializeToString(value, value.GetType()),
                };
            }

			var memStream = new MemoryStream(4);
			JsonSerializer.SerializeToStream(item, memStream);
			var bytes = memStream.ToArray();
			memStream.Close();
			return bytes;
		}

		public object Deserialize(byte[] bytes)
		{
            var memStream = new MemoryStream(bytes);
            var item = ServiceStack.Text.JsonSerializer.DeserializeFromStream<CacheContainer>(memStream);
            memStream.Close();

            if (item is CacheContainer && item != null)
            {
                return Deserialize(item.Type, item.Value);
            }
            else
            {
                return null;
            }
            
		}

		public object Deserialize(Type type, string serialized)
		{
            return ServiceStack.Text.JsonSerializer.DeserializeFromString(serialized, type);
		}

	}
}
