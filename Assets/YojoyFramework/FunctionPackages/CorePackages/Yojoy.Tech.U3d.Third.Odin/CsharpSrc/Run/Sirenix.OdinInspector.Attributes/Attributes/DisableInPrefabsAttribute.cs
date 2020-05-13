#pragma warning disable
//-----------------------------------------------------------------------
// <copyright file="DisableInPrefabsAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// Disables a property if it is drawn from a prefab asset or a prefab instance.
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All)]
    public class DisableInPrefabsAttribute : Attribute
    {
    }
}
#pragma warning enable