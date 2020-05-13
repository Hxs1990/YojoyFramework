#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="DisableInEditorModeAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="DisableInEditorModeAttribute"/>.
    /// </summary>
    /// <seealso cref="DisableInPlayModeAttribute"/>
    /// <seealso cref="DisableInEditorModeAttribute"/>
    /// <seealso cref="HideInEditorModeAttribute"/>
    /// <seealso cref="HideInPlayModeAttribute"/>
    /// <seealso cref="DisableIfAttribute"/>
    /// <seealso cref="EnableIfAttribute"/>

    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public class DisableInEditorModeAttributeDrawer : OdinAttributeDrawer<DisableInEditorModeAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
		protected override void DrawPropertyLayout(GUIContent label)
        {
            GUIHelper.PushGUIEnabled(Application.isPlaying && GUI.enabled);
            this.CallNextDrawer(label);
            GUIHelper.PopGUIEnabled();
        }
    }
}
#endif
#pragma warning enable