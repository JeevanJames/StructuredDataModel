using System;
using System.Collections.Generic;
using System.Globalization;

using YamlDotNet.RepresentationModel;

namespace NStructuredDataModel.Yaml
{
#pragma warning disable CA1812
    /// <summary>
    ///     Traverses a yaml document and constructs a dynamic object model from it.
    /// </summary>
    internal sealed class YamlDocumentTraverser
    {
        private readonly YamlMappingNode _yamlRoot;
        private readonly AbstractNode _model;

        /// <summary>
        ///     Initializes a new instance of the <see cref="YamlDocumentTraverser"/> class.
        /// </summary>
        /// <param name="yaml">The yaml document to traverse.</param>
        /// <param name="model">The dynamic object to update with the object model.</param>
        internal YamlDocumentTraverser(YamlDocument yaml, AbstractNode model)
        {
            if (yaml is null)
                throw new ArgumentNullException(nameof(yaml));
            _yamlRoot = (YamlMappingNode)yaml.RootNode;
            _model = model;
        }

        internal void Traverse()
        {
            Traverse(_yamlRoot, _model);
        }

        private static void Traverse(YamlMappingNode mappingNode, AbstractNode model)
        {
            foreach ((YamlNode key, YamlNode value) in mappingNode.Children)
            {
                string? propertyName = ((YamlScalarNode)key).Value;
                if (propertyName is null)
                    throw new InvalidOperationException();

                switch (value)
                {
                    case YamlScalarNode scalarNode:
                        SetScalarValue(model, propertyName, scalarNode.Value);
                        break;

                    case YamlSequenceNode sequenceNode:
                        SetSequenceValue(model, propertyName, sequenceNode.Children);
                        break;

                    case YamlMappingNode childMappingNode:
                        AbstractNode property = SetObjectValue(model, propertyName);
                        Traverse(childMappingNode, property);
                        break;
                }
            }
        }

        private static void SetScalarValue(AbstractNode parentNode, string propertyName, object? value)
        {
            //TODO: Should we validate the value here?
            parentNode.AddOrUpdate(propertyName, value);
        }

        /// <summary>
        ///     This method is called for YAML mapping nodes, which are objects with properties. No
        ///     scalar value is being set; we just create a variable node for this property, if one
        ///     doesn't exist.
        /// </summary>
        private static AbstractNode SetObjectValue(AbstractNode parentNode, string propertyName)
        {
            // Check if node has a property with the specified property name.
            if (parentNode.TryGetValue(propertyName, out object? value))
            {
                // If the property is a variable node, that's fine.
                if (value is AbstractNode node)
                    return node;

                // If the property is any other type, that means it's a scalar value and hence a
                // leaf node. This should not be allowed.
                throw new InvalidOperationException($"Property {propertyName} already exists, but it is a scalar value and not a variable node.");
            }

            // If the property does not exist, add it as a variable node.
            var newProperty = new Node();
            parentNode.Add(propertyName, newProperty);
            return newProperty;
        }

        private static void SetSequenceValue(AbstractNode parentNode, string propertyName, IList<YamlNode> children)
        {
            if (parentNode.TryGetValue(propertyName, out object? value) && value is not AbstractNode)
                throw new InvalidOperationException($"Property {propertyName} already exists, but it is a scalar value and not a variable node.");

            // Create a node for the sequence property
            AbstractNode sequenceNode = SetObjectValue(parentNode, propertyName);

            for (int i = 0; i < children.Count; i++)
            {
                string index = i.ToString(CultureInfo.InvariantCulture);
                switch (children[i])
                {
                    case YamlScalarNode scalarItem:
                        SetScalarValue(sequenceNode, index, scalarItem.Value);
                        break;
                    case YamlSequenceNode sequenceItem:
                        SetSequenceValue(sequenceNode, index, sequenceItem.Children);
                        break;
                    case YamlMappingNode mappingItem:
                        AbstractNode vn = SetObjectValue(sequenceNode, index);
                        Traverse(mappingItem, vn);
                        break;
                }
            }
        }
    }
}
