// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;

using YamlDotNet.RepresentationModel;

namespace NStructuredDataModel.Yaml;

/// <summary>
///     Traverses a yaml document and constructs a dynamic object model from it.
/// </summary>
internal sealed class YamlImporter
{
    private readonly YamlMappingNode _yamlRoot;
    private readonly Node _model;

    /// <summary>
    ///     Initializes a new instance of the <see cref="YamlImporter"/> class.
    /// </summary>
    /// <param name="yaml">The yaml document to traverse.</param>
    /// <param name="model">The dynamic object to update with the object model.</param>
    internal YamlImporter(YamlDocument yaml, Node model)
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

    private static void Traverse(YamlMappingNode mappingNode, Node model)
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
                    Node property = SetObjectValue(model, propertyName);
                    Traverse(childMappingNode, property);
                    break;
            }
        }
    }

    private static void SetScalarValue(Node parentNode, string propertyName, object? value)
    {
        // Check for string values as boolean
        //TODO: This will treat strings with true/false values as boolean. Need to handle this.
        if (value is string str)
        {
            if (string.Equals(str, "true", StringComparison.OrdinalIgnoreCase))
                value = true;
            else if (string.Equals(str, "false", StringComparison.OrdinalIgnoreCase))
                value = false;
        }

        parentNode.AddOrUpdate(propertyName, new NodeValue(value));
    }

    /// <summary>
    ///     This method is called for YAML mapping nodes, which are objects with properties. No
    ///     scalar value is being set; we just create a variable node for this property, if one
    ///     doesn't exist.
    /// </summary>
    private static Node SetObjectValue(Node parentNode, string propertyName)
    {
        // Check if node has a property with the specified property name.
        if (parentNode.TryGetValue(propertyName, out NodeValue value))
        {
            // If the property is a variable node, that's fine.
            if (value.IsNode)
                return value.Node;

            // If the property is any other type, that means it's a scalar value and hence a
            // leaf node. This should not be allowed.
            throw new InvalidOperationException($"Property {propertyName} already exists, but it is a scalar value and not a variable node.");
        }

        // If the property does not exist, add it as a variable node.
        var newProperty = new Node();
        parentNode.Add(propertyName, new NodeValue(newProperty));
        return newProperty;
    }

    private static void SetSequenceValue(Node parentNode, string propertyName, IList<YamlNode> children)
    {
        if (parentNode.TryGetValue(propertyName, out NodeValue value) && !value.IsNode)
            throw new InvalidOperationException($"Property {propertyName} already exists, but it is a scalar value and not a variable node.");

        // Create a node for the sequence property
        Node sequenceNode = SetObjectValue(parentNode, propertyName);

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
                    Node vn = SetObjectValue(sequenceNode, index);
                    Traverse(mappingItem, vn);
                    break;
            }
        }
    }
}
