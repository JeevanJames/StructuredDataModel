﻿// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace NStructuredDataModel
{
    [Serializable]
    public sealed class StructuredDataModel : AbstractNode
    {
        public StructuredDataModel()
        {
        }

        private StructuredDataModel(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
