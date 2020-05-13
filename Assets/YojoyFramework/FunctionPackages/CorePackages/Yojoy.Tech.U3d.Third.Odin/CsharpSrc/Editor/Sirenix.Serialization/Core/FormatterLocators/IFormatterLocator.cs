#pragma warning disable
//-----------------------------------------------------------------------\n// <copyright file="IFormatterLocator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Serialization
{
    using System;

    public interface IFormatterLocator
    {
        bool TryGetFormatter(Type type, FormatterLocationStep step, ISerializationPolicy policy, out IFormatter formatter);
    }
}
#pragma warning enable