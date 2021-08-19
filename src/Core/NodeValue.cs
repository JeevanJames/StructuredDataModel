// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace NStructuredDataModel
{
    public readonly struct NodeValue
    {
        public NodeValue(object? value)
        {
            NodeValueType? valueType = GetValueType(value);
            if (valueType is null)
                throw new ArgumentException("Invalid value specified.", nameof(value));
            Value = value;
            ValueType = valueType.Value;
        }

        public object? Value { get; }

        public NodeValueType ValueType { get; }

        public AbstractNode AsNode()
        {
            if (Value is null || ValueType != NodeValueType.Node)
                throw new InvalidCastException();
            return (AbstractNode)Value;
        }

        public string AsString() => TryCastAs<string>();

        public string? AsNullableString() => TryCastNullableAs<string>();

        public char AsChar() => TryCastAs<char>();

        public char? AsNullableChar() => TryCastNullableAs<char>();

        public bool AsBool() => TryCastAs<bool>();

        public bool? AsNullableBool() => TryCastNullableAs<bool>();

        public byte AsByte() => TryCastAs<byte>();

        public byte? AsNullableByte() => TryCastNullableAs<byte>();

        public sbyte AsSByte() => TryCastAs<sbyte>();

        public sbyte? AsNullableSByte() => TryCastNullableAs<sbyte>();

        public short AsShort() => TryCastAs<short>();

        public short? AsNullableShort() => TryCastNullableAs<short>();

        public ushort AsUShort() => TryCastAs<ushort>();

        public ushort? AsNullableUShort() => TryCastNullableAs<ushort>();

        public int AsInt() => TryCastAs<int>();

        public int? AsNullableInt() => TryCastNullableAs<int>();

        public uint AsUInt() => TryCastAs<uint>();

        public uint? AsNullableUInt() => TryCastNullableAs<uint>();

        public long AsLong() => TryCastAs<long>();

        public long? AsNullableLong() => TryCastNullableAs<long>();

        public ulong AsULong() => TryCastAs<ulong>();

        public ulong? AsNullableULong() => TryCastNullableAs<ulong>();

        public float AsFloat() => TryCastAs<float>();

        public float? AsNullableFloat() => TryCastNullableAs<float>();

        public double AsDouble() => TryCastAs<double>();

        public double? AsNullableDouble() => TryCastNullableAs<double>();

        public decimal AsDecimal() => TryCastAs<decimal>();

        public decimal? AsNullableDecimal() => TryCastNullableAs<decimal>();

        private T TryCastAs<T>()
        {
            if (Value is null)
                throw new InvalidCastException();

            if (typeof(T) != Value.GetType())
                throw new InvalidCastException();

            return (T)Value;
        }

        private T? TryCastNullableAs<T>()
        {
            if (Value is null)
                return default;

            return (T)Value;
        }

        private static NodeValueType? GetValueType(object? value)
        {
            // It's ok if the value is null
            if (value is null)
                return NodeValueType.Null;

            if (value is AbstractNode)
                return NodeValueType.Node;

            Type valueType = value.GetType();

            // It's ok if the value type belongs to the valid list.
            if (ValidPropertyTypes.TryGetValue(valueType, out NodeValueType nodeValueType))
                return nodeValueType;

            return null;

            //// Check if the value implements IEnumerable<>
            //Type? enumerableInterface = Array.Find(valueType.GetInterfaces(), intf => intf.IsGenericType
            //    && intf.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            //if (enumerableInterface is null)
            //    return false;

            //// If the value implements IEnumerable<>, the generic type must belong to the valid list.
            //Type enumerableType = enumerableInterface.GenericTypeArguments[0];
            //if (!ValidPropertyTypes.Contains(enumerableType))
            //    return false;

            //return true;
        }

        private static readonly Dictionary<Type, NodeValueType> ValidPropertyTypes = new()
        {
            [typeof(string)] = NodeValueType.String,
            [typeof(char)] = NodeValueType.Char,
            [typeof(bool)] = NodeValueType.Bool,
            [typeof(byte)] = NodeValueType.Byte,
            [typeof(sbyte)] = NodeValueType.SByte,
            [typeof(short)] = NodeValueType.Short,
            [typeof(ushort)] = NodeValueType.UShort,
            [typeof(int)] = NodeValueType.Int,
            [typeof(uint)] = NodeValueType.UInt,
            [typeof(long)] = NodeValueType.Long,
            [typeof(ulong)] = NodeValueType.ULong,
            [typeof(float)] = NodeValueType.Float,
            [typeof(double)] = NodeValueType.Double,
            [typeof(decimal)] = NodeValueType.Decimal,
        };
    }

    public enum NodeValueType
    {
        Null,
        Node,
        String,
        Char,
        Bool,
        Byte,
        SByte,
        Short,
        UShort,
        Int,
        UInt,
        Long,
        ULong,
        Float,
        Double,
        Decimal,
    }
}
