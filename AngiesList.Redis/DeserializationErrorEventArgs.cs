using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngiesList.Redis
{
    public class DeserializationErrorEventArgs : EventArgs 
    {
        public Exception Exception { get; set; }
        public bool ExceptionHandled { get; set; }
    }
}
