using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable SA1600 // Elements should be documented
namespace BeanLib.References.Exceptions
{
    [Serializable]
    public class ReferenceNotFoundException : Exception
    {
        public ReferenceNotFoundException() { }

        public ReferenceNotFoundException(string message) : base(message) { }

        public ReferenceNotFoundException(string message, Exception inner) : base(message, inner) { }

        protected ReferenceNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
#pragma warning restore SA1600 // Elements should be documented
