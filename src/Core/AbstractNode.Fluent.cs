namespace NStructuredDataModel
{
    public partial class AbstractNode
    {
        public AbstractNode Node(string name)
        {
            Node node = new();
            Add(name, node);
            return node;
        }

        public AbstractNode Value<T>(string name, T value)
        {
            ValidatePropertyValue(value);

            Add(name, value);

            return this;
        }
    }
}
