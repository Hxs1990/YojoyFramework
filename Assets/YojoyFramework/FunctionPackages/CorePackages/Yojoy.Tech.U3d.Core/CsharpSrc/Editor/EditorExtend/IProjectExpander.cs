#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/5/31 23:19:35
//Email:         854327817@qq.com

#endregion

namespace Yojoy.Tech.U3d.Core.Editor
{
    public interface IProjectExpander:IEditorExpander
    {
        /// <summary>
        /// ���浱ǰ����Դid��·����Ϣ
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="path"></param>
        void SaveContext(string guid, string path);
        /// <summary>
        /// ��⵱ǰ��Դ����·���Ƿ�����չ���������չĿ��
        /// </summary>
        /// <returns></returns>
        bool CheckContext();
    }
}
