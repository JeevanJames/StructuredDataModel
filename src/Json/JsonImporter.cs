using System;
using System.Text.Json;

namespace NStructuredDataModel.Json
{
    internal sealed class JsonImporter
    {
        internal void Import(AbstractNode node, ReadOnlySpan<byte> jsonBytes)
        {
            Utf8JsonReader reader = new(jsonBytes, new JsonReaderOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip,
            });
            ImportNode(node, reader);
        }

        private static void ImportNode(AbstractNode node, Utf8JsonReader reader)
        {
            while (reader.Read())
            {
                node.Add(Guid.NewGuid().ToString("N"), new NodeValue(reader.TokenType.ToString()));
            }
        }
    }
}
