using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using System.IO;

namespace AngiesList.Redis
{
    /// <summary>
    /// ServiceStack.Text Type serializer
    /// </summary>
	public class SSTypeSerializer : IValueSerializer
	{
        public class CacheContainer<T>
        {
            public CacheContainer() : base() 
            {
                this.Type = typeof(T).AssemblyQualifiedName;
            }
            
            public CacheContainer(T value) : this() 
            {
                this.Value = value;
            }

            public string Type { get; set; }
            public T Value { get; set; }
        }

		public byte[] Serialize(object value)
		{
            //CacheContainer item = null;

            if (value != null)
            {
                var t = typeof(CacheContainer<>).MakeGenericType(value.GetType());
                var item = Activator.CreateInstance(t,value);
                
                var memStream = new MemoryStream(4);
                TypeSerializer.SerializeToStream(item, memStream);
                var bytes = memStream.ToArray();
                memStream.Close();
                return bytes;
            }

            return new byte[] { }; // empty array
		}

		public object Deserialize(byte[] bytes)
		{
            var stringValue = System.Text.Encoding.Default.GetString(bytes);
            var x = TypeSerializer.DeserializeFromString<CacheContainer<object>>(stringValue);
            
            if (x == null)
                return null;

            var valueType = Type.GetType(x.Type);
            
            // special handling for empty string
            if (valueType == typeof(string) && x.Value == null)
                return string.Empty;

            var item = TypeSerializer.DeserializeFromString(x.Value.ToString(), valueType);
            return item;
		}



	}
}
