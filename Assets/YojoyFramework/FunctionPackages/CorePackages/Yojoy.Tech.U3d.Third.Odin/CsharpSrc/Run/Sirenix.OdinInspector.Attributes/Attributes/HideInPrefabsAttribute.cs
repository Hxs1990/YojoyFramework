#pragma warning disable
//-----------------------------------------------------------------------
// <copyright file="HideInPrefabsAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// Hides a property if it is drawn from a prefab instance or a prefab asset.
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All)]
    public class HideInPrefabsAttribute : Attribute
    {
    }
}
#pragma warning enable