using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
/// <summary>
/// �����ļ�������
/// </summary>
public class LocalFileMgr : SingletonMiddle<LocalFileMgr>
{
    #region ��������·���������ļ�������byte���鷵�� GetBufffer
    /// <summary>
    /// ��������·���������ļ�������byte���鷵��
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public byte[] GetBufffer(string path)
    {
        byte[] buffer = null;
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
        }
        return buffer;
    }
    #endregion
}
