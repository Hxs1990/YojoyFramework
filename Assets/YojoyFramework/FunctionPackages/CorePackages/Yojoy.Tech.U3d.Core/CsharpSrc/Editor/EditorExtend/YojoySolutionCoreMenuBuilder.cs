#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/4 22:44:38
//Email:         854327817@qq.com

#endregion

using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Core.Editor
{
    public class YojoySolutionCoreMenuBuilder : IProjectExpanderMenuBuilder
    {
        private string directory;

        public Type ConcernedExpanderType => typeof(YojoySolutionExpander);

        public void AddMenuItem(GenericMenu genericMenu, string path)
        {
            directory = path;
            var opneSandBoxTip = MultiLanguageString.Create(
                "IO/Open sanbox directory",
                "IO/��ɳ��Ŀ¼");
            genericMenu.AddItem(new GUIContent(opneSandBoxTip.Text),
                false, OpenSandBox, null);
            var cleanMetaTip = MultiLanguageString.Create
                ("IO/Clean meta file", "IO/����meta�ļ�");
            genericMenu.AddItem(new GUIContent(cleanMetaTip.Text),
                false, CleanMeta, path);
        }
        private void OpenSandBox(object data) =>
            Process.Start(Application.persistentDataPath);
        private void CleanMeta(object usderData)
        {
            var metaPaths = DirectoryUtility.GetPathsContainSonDirectory(
                directory, p => p.EndsWith(".meta"));
            FileUtility.TryDeleteAll(metaPaths);
            UnityEditorUtility.DisplayTip("meta�ļ�������ϣ�");
        }
    }
}
