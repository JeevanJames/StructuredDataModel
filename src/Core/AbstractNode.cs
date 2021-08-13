using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace NStructuredDataModel
{
    [Serializable]
    public abstract partial class AbstractNode : Dictionary<string, object?>
    {
        protected AbstractNode()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        protected AbstractNode(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public bool TryGetAsArray(out object?[] array)
        {
            if (Count == 0)
            {
                array = Array.Empty<object?>();
                return false;
            }

            if (!TryGetValue("0", out object? firstValue))
            {
                array = Array.Empty<object?>();
                return false;
            }

            array = new object?[Count];
            array[0] = firstValue;

            int index = 1;
            while (index < Count)
            {
                if (!TryGetValue(index.ToString(), out object? value))
                    return false;
                array[index] = value;
                index++;
            }

            return true;
        }

        public T? GetValue<T>(string propertyName, T? defaultValue = default)
        {
            return TryGetValue(propertyName, out object? valueObject) ? (T?)valueObject : defaultValue;
        }

        public T? Read<T>(ICollection<string> propertyPath, T? defaultValue)
        {
            ValidatePropertyPath(propertyPath);

            AbstractNode propertyParent = GetPropertyParentNode(propertyPath);
            return propertyParent.GetValue(propertyPath.Last(), defaultValue);
        }

        public T? Read<T>(string propertyName, T? defaultValue)
        {
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));

            string[] propertyNames = propertyName.Split('.');
            return Read(propertyNames, defaultValue);
        }

        public AbstractNode Write<T>(ICollection<string> propertyPath, T? value)
        {
            ValidatePropertyPath(propertyPath);
            ValidatePropertyValue(value);

            AbstractNode parentNode = GetPropertyParentNode(propertyPath);
            string propertyName = propertyPath.Last();
            parentNode.AddOrUpdate(propertyName, value);

            return this;
        }

        public AbstractNode Write<T>(string propertyName, T? value)
        {
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));

            string[] propertyPath = propertyName.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (propertyPath.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"Property name '{propertyName}' is invalid.", nameof(propertyName));

            return Write(propertyPath, value);
        }

        private static void ValidatePropertyPath(ICollection<string> propertyPath)
        {
            if (propertyPath is null)
                throw new ArgumentNullException(nameof(propertyPath));
            if (propertyPath.Count == 0)
                throw new ArgumentException("The property path cannot be empty.", nameof(propertyPath));
            if (propertyPath.Any(p => string.IsNullOrWhiteSpace(p) || !PropertyNamePattern.IsMatch(p)))
                throw new ArgumentException("The specified property path is invalid.", nameof(propertyPath));
        }

        private static readonly Regex PropertyNamePattern = new(@"^[A-Za-z]\w*$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(1));

        /// <summary>
        ///     Returns the parent of a property path in the Variable. If parts of the property
        ///     tree do not exist, then they will be created.
        /// </summary>
        private AbstractNode GetPropertyParentNode(ICollection<string> propertyPath)
        {
            // This StringBuilder is used just to construct the error message.
            StringBuilder propertyNameBuilder = new();

            // Start traversal at the root object in the variable. If this is for a root property
            // (propertyPath is empty), then the Variable.Data property is returned.
            AbstractNode current = this;

            foreach (string property in propertyPath.SkipLast(1))
            {
                // Build up the property name up to the current property in the traversal.
                if (propertyNameBuilder.Length > 0)
                    propertyNameBuilder.Append('.');
                propertyNameBuilder.Append(property);

                // If the property does not have a value, create a new tree node.
                if (!current.TryGetValue(property, out object? propertyValue))
                {
                    propertyValue = new Node();
                    current.Add(property, propertyValue);
                }

                // If the property value is not a Node, then it is a leaf node value. But
                // that should not be the case here as we expect a parent node, which should be a
                // VariableNode.
                else if (propertyValue is not AbstractNode)
                {
                    string errorMessage =
                        $"The property {propertyNameBuilder} has already been assigned a scalar or array value. "
                        + "It cannot be a complex object.";
                    throw new InvalidOperationException(errorMessage);
                }

                current = (Node)propertyValue;
            }

            return current;
        }

        private static void ValidatePropertyValue(object? value)
        {
            // It's ok if the value is null
            if (value is null)
                return;

            Type valueType = value.GetType();

            // It's ok if the value type belongs to the valid list.
            if (ValidPropertyTypes.Contains(valueType))
                return;

            // Check if the value implements IEnumerable<>
            Type? enumerableInterface = Array.Find(valueType.GetInterfaces(), intf => intf.IsGenericType
                && intf.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (enumerableInterface is null)
                throw new ArgumentException("Property value is invalid.", nameof(value));

            // If the value implements IEnumerable<>, the generic type must belong to the valid list.
            Type enumerableType = enumerableInterface.GenericTypeArguments[0];
            if (!ValidPropertyTypes.Contains(enumerableType))
            {
                string errorMessage = $"Property value is an enumerable that contains an invalid type {enumerableType}.";
                throw new ArgumentException(errorMessage, nameof(value));
            }
        }

        private static readonly List<Type> ValidPropertyTypes = new()
        {
            typeof(string),
            typeof(char),
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
        };
    }
}
