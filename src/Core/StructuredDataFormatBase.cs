// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NStructuredDataModel
{
    public abstract class StructuredDataFormatBase<TOptions> : IStructuredDataFormat
        where TOptions : StructuredDataFormatOptions
    {
        protected StructuredDataFormatBase(TOptions options)
        {
            Options = options;
        }

        public TOptions Options { get; }

        public virtual Task ImportAsync(TextReader reader, AbstractNode node)
        {
            Import(reader, node);
            return Task.CompletedTask;
        }

        protected virtual void Import(TextReader reader, AbstractNode node)
        {
            throw new NotImplementedException();
        }

        public virtual Task ExportAsync(TextWriter writer, AbstractNode node)
        {
            Export(writer, node);
            return Task.CompletedTask;
        }

        protected virtual void Export(TextWriter writer, AbstractNode node)
        {
            throw new NotImplementedException();
        }

        protected Task Traverse(AbstractNode node, Func<IList<string>, Task> nodeFn,
            Func<IList<string>, object?, Task> valueFn)
        {
            List<string> path = new();
            return TraverseRecursive(node, path, nodeFn, valueFn);
        }

        private async Task TraverseRecursive(AbstractNode node, List<string> path, Func<IList<string>, Task> nodeFn,
            Func<IList<string>, object?, Task> valueFn)
        {
            foreach ((string key, NodeValue value) in node)
            {
                if (value.ValueType == NodeValueType.Node)
                {
                    await nodeFn(path);
                    path.Add(key);
                    await TraverseRecursive(value.AsNode(), path, nodeFn, valueFn);
                }
                else
                {
                    path.Add(key);
                    await valueFn(path, value.Value);
                }

                path.RemoveAt(path.Count - 1);
            }
        }
    }
}
