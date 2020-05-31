#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/5/31 23:33:26
//Email:         854327817@qq.com

#endregion

using System;
using UnityEditor;

namespace Yojoy.Tech.U3d.Core.Editor
{
    /// <summary>
    /// ΪĿ����չ����Ӷ�̬�˵�
    /// </summary>
    public interface IProjectExpanderMenuBuilder
    {
        /// <summary>
        /// ����ʵ������ע�ı༭����չ������
        /// </summary>
        Type ConcernedExpanderType { get; }

        /// <summary>
        /// ��Ӳ˵���
        /// </summary>
        /// <param name="genericMenu"></param>
        /// <param name="path"></param>
        void AddMenuItem(GenericMenu genericMenu, string path);
    }
}
