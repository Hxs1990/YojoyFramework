#pragma warning disable
//-----------------------------------------------------------------------
// <copyright file="DisableInNonPrefabsAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// Disables a property if it is drawn from a non-prefab asset or instance.
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All)]
    public class DisableInNonPrefabsAttribute : Attribute
    {
    }
}
#pragma warning enable