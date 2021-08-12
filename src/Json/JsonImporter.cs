using System;
using System.Text.Json;

namespace NStructuredDataModel.Json
{
    internal sealed class JsonImporter
    {
        internal void Import(AbstractNode node, ReadOnlySpan<byte> jsonBytes)
        {
            Utf8JsonReader reader = new(jsonBytes);
            ImportNode(node, reader);
        }

        private void ImportNode(AbstractNode node, Utf8JsonReader reader)
        {
            while (reader.Read())
            {
                node.Add(Guid.NewGuid().ToString("N"), reader.TokenType.ToString());
            }
        }
    }
}
