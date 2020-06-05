#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/5 11:53:46
//Email:         854327817@qq.com

#endregion

using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Core.Run
{
    /// <summary>
    /// ����ʱģ�����ʹ�ñ༭���µ����ͺͷ���
    /// </summary>
    /// <typeparam name="TPrefsValueSet"></typeparam>
    /// <typeparam name="TPrefsValue"></typeparam>
    /// <typeparam name="Tvalue"></typeparam>
    [Serializable]
    public abstract class AbstractPrefsValueStorageGeneric<TPrefsValueSet, TPrefsValue, Tvalue>
        where TPrefsValue : IPrefsValueGeneric<Tvalue>, new()
      where TPrefsValueSet : AbstractPrefsValueSetGeneric<TPrefsValue, Tvalue>, new()
    {
        #region Prefs Query
        [SerializeField]
        [BoxGroup(englishGroupId: "Query", chineseGroupId: "��ѯ")]
        [LabelText(english: "Target prefs id", chinese: "Ŀ������ID")]
        private string targetPrefsId;

        [ShowIf(memberName:"TargetPrefsIsNotNull")]
        [SerializeField]
        [LabelText("Target prefs value","Ŀ������")]
        private TPrefsValue targetPrefsValue;
        private bool TargetPrefsIsNotNull() => targetPrefsValue != null;


        [BoxGroup(englishGroupId: "Query", chineseGroupId: "��ѯ")]
        [Button(english: "Query", chinese: "��ѯ")]
        private void Query()
        {
            if (!targetPrefsId.IsValid())
            {
                YojoyEditorAgent.DispalyTip("The prefs id is null!");
                return;
            }
            var prefsValue = GetPrefsValue(targetPrefsId);
            if (prefsValue == null)
            {
                YojoyEditorAgent.DispalyTip("Cannot find target prefs");
                return;
            }
            targetPrefsValue = prefsValue;
          }


        #endregion

        #region Prefs Value Set
        [HideLabel]
        [SerializeField]
        private TPrefsValueSet prefsValueSet = new TPrefsValueSet();

        public TPrefsValueSet PrefsValueSet => prefsValueSet;
        #endregion

        #region Get And Save
        public TPrefsValue GetPrefsValue(string key) =>
            prefsValueSet.GetPrefsValue(key);
        public void SavePrefsValue(TPrefsValue prefsValue) =>
            prefsValueSet.AddOrUpdatePrefsValue(prefsValue);

        #endregion
    }
}
