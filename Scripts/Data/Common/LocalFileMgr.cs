using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
/// <summary>
/// 本地文件管理器
/// </summary>
public class LocalFileMgr : SingletonMiddle<LocalFileMgr>
{
    #region 根据所传路径将本地文件制作成byte数组返回 GetBufffer
    /// <summary>
    /// 根据所传路径将本地文件制作成byte数组返回
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
