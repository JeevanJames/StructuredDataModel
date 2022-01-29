// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace NStructuredDataModel;

/// <summary>
///     Represents the base options for a <see cref="IStructuredDataFormat" />. The base options specifies
///     how to convert .NET property names to a specific format's property name (e.g. JSON properties
///     or YAML properties).
///     <para />
///     Concrete formats can derive their own options from this class to add more capabilities specific
///     to the format.
/// </summary>
public class FormatOptions
{
    /// <summary>
    ///     Gets or sets an optional converter delegate to convert from the .NET property names in
    ///     the <see cref="Node" /> instances to a format's property name.
    /// </summary>
    public Converter<string, string>? PropertyNameConverter { get; set; }

    /// <summary>
    ///     Shortcut method to use the <see cref="PropertyNameConverter" /> to convert from the .NET
    ///     property names in the <see cref="Node" /> instances to a format's property name.
    /// </summary>
    /// <param name="propertyName">The property name to convert.</param>
    /// <returns>
    ///     The converted property name, if a converter is specified, otherwise the original property
    ///     name itself.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="propertyName" /> is <c>null</c>.
    /// </exception>
    public string ConvertPropertyName(string propertyName)
    {
        if (propertyName is null)
            throw new ArgumentNullException(nameof(propertyName));
        return PropertyNameConverter?.Invoke(propertyName) ?? propertyName;
    }
}
