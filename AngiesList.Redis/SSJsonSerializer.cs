using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using System.IO;

namespace AngiesList.Redis
{
    /// <summary>
    /// ServiceStack.Text JSON serializer.
    /// </summary>
	public class SSJsonSerializer : IValueSerializer
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
                JsonSerializer.SerializeToStream(item, memStream);
                var bytes = memStream.ToArray();
                memStream.Close();
                return bytes;
            }

            return new byte[] { }; // empty array
		}

		public object Deserialize(byte[] bytes)
		{
            var x = JsonObject.Parse(System.Text.Encoding.Default.GetString(bytes));

            if (x == null)
                return null;

            string typeName, jsonValue;
            if (x.TryGetValue("Type", out typeName) && x.TryGetValue("Value", out jsonValue))
            {
                var valueType = Type.GetType(typeName);
                var item = JsonSerializer.DeserializeFromString(jsonValue, valueType);
                return item;
            }

            return null;
		}

	}
}
