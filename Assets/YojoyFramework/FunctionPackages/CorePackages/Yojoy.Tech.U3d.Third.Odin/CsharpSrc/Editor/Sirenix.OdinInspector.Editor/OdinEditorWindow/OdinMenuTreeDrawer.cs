#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="OdinMenuTreeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using UnityEngine;

    internal class OdinMenuTreeDrawer : OdinValueDrawer<OdinMenuTree>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var tree = entry.SmartValue;
            if (tree != null)
            {
                tree.DrawMenuTree();
                tree.HandleKeybaordMenuNavigation();
            }
        }
    }
}
#endif
#pragma warning enable