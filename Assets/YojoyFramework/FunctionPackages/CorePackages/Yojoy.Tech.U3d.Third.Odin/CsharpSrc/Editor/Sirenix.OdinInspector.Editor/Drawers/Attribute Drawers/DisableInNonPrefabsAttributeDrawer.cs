#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="DisableInNonPrefabsAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="DisableInNonPrefabsAttribute"/>.
    /// </summary>

    [DrawerPriority(1000, 0, 0)]
    public sealed class DisableInNonPrefabsAttributeDrawer : OdinAttributeDrawer<DisableInNonPrefabsAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var property = this.Property;
            var unityObjectTarget = property.Tree.WeakTargets[0] as UnityEngine.Object;

            if (unityObjectTarget == null)
            {
                this.CallNextDrawer(label);
                return;
            }

            PropertyContext<bool> disable;

            if (property.Context.Get(this, "disable", out disable))
            {
                var type = PrefabUtility.GetPrefabType(unityObjectTarget);
                disable.Value =
                    type == PrefabType.None ||
                    type == PrefabType.MissingPrefabInstance ||
                    type == PrefabType.DisconnectedModelPrefabInstance ||
                    type == PrefabType.DisconnectedPrefabInstance;
            }

            if (disable.Value)
            {
                GUIHelper.PushGUIEnabled(false);
                this.CallNextDrawer(label);
                GUIHelper.PopGUIEnabled();
            }
            else
            {
                this.CallNextDrawer(label);
            }
        }
    }
}
#endif
#pragma warning enable