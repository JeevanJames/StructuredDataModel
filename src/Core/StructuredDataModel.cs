using System;
using System.Runtime.Serialization;

namespace NStructuredDataModel
{
    [Serializable]
    public sealed class StructuredDataModel : AbstractNode
    {
        public StructuredDataModel()
        {
        }

        private StructuredDataModel(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
