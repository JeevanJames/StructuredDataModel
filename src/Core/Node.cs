using System;
using System.Runtime.Serialization;

namespace NStructuredDataModel
{
    [Serializable]
    public sealed class Node : AbstractNode
    {
        public Node()
        {
        }

        private Node(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
