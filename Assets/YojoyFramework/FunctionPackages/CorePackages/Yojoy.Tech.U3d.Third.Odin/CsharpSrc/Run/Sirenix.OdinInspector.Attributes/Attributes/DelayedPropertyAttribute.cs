#pragma warning disable
//-----------------------------------------------------------------------
// <copyright file="DelayedPropertyAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
	using System;
    
    /// <summary>
    /// Delays applying changes to properties while they still being edited in the inspector.
    /// Similar to Unity's built-in Delayed attribute, but this attribute can also be applied to properties.
    /// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public class DelayedPropertyAttribute : Attribute
	{ }
}
#pragma warning enable