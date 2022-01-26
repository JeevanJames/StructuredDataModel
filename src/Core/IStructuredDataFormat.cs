// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NStructuredDataModel;

/// <summary>
///     Provides the ability to import/export a <see cref="Node"/> from/to a specific format like
///     JSON, XML, YAML, etc.
/// </summary>
public interface IStructuredDataFormat
{
    /// <summary>
    ///     Imports the contents of the specified <see cref="TextReader"/> <paramref name="reader"/>
    ///     into the specified <paramref name="node"/>.
    /// </summary>
    /// <param name="reader">The <see cref="TextReader"/> whose contents are to be imported.</param>
    /// <param name="node">The <see cref="Node"/> instance to import into.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ImportAsync(TextReader reader, Node node);

    /// <summary>
    ///     Exports the contents of the specified <see cref="Node"/> into the specified <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to export the node to.</param>
    /// <param name="node">The <see cref="Node"/> to export.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ExportAsync(TextWriter writer, Node node);
}

/// <summary>
///     Provides commonly-used overloads to the <see cref="IStructuredDataFormat"/> methods, such
///     as importing/exporting directly from/to a string.
/// </summary>
public static class StructuredDataFormatExtensions
{
    /// <summary>
    ///     Imports the contents of the specified string <paramref name="data"/> into the specified
    ///     <see cref="Node"/> instance.
    /// </summary>
    /// <param name="format">The <see cref="IStructuredDataFormat"/> instance.</param>
    /// <param name="data">The string to import.</param>
    /// <param name="node">The <see cref="Node"/> instance to import into.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static Task ImportAsync(this IStructuredDataFormat format, string data, Node node)
    {
        using StringReader reader = new(data);
        return format.ImportAsync(reader, node);
    }

    public static async Task<StructuredDataModel> ImportAsync(this IStructuredDataFormat format, TextReader reader)
    {
        StructuredDataModel model = new();
        await format.ImportAsync(reader, model).ConfigureAwait(false);
        return model;
    }

    public static Task<StructuredDataModel> ImportAsync(this IStructuredDataFormat format, string data)
    {
        using StringReader reader = new(data);
        return format.ImportAsync(reader);
    }

    public static async Task<string> ExportAsync(this IStructuredDataFormat format, Node node)
    {
        StringBuilder sb = new();
        await using StringWriter writer = new(sb);
        await format.ExportAsync(writer, node).ConfigureAwait(false);
        return writer.ToString();
    }
}
