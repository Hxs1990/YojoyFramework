#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="AtomHandlerAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class AtomHandlerAttribute : Attribute
    {
    }
}
#endif
#pragma warning enable