using System;
using System.IO;

using YamlDotNet.RepresentationModel;

namespace NStructuredDataModel.Yaml
{
    public sealed class YamlFormat : StructuredDataFormatBase
    {
        protected override void Import(TextReader reader, AbstractNode node)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            YamlStream yaml = new();
            yaml.Load(reader);

            foreach (YamlDocument yamlDocument in yaml.Documents)
            {
                YamlDocumentTraverser traverser = new(yamlDocument, node);
                traverser.Traverse();
            }
        }
    }
}
