//===================================================
//��    �ߣ�����  http://www.u3dol.com  QQȺ��87481002
//����ʱ�䣺2016-06-11 12:53:31
//��    ע��
//===================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using XLua;
using UnityEngine.UI;

[LuaCallCSharp]
public class GameUtil
{
    #region ��ȡ�������
    //��
    static string[] surnameArray = {"˾��", "ŷ��", "��ľ", "�Ϲ�", "����", "�ĺ�", "ξ��", "����", "�ʸ�", "����", "Ľ��", "����", "����", "˾ͽ", "��ԯ", "����", "����", "���",
            "���", "�Ϲ�", "����", "����", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "֣", "��", "��", "��", "��", "��", "Ԭ", "��", "��",
            "��", "��", "��", "��", "��", "κ", "Ҷ", "��", "��", "��", "��", "��", "ʯ", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "³", "��", "��",
            "��", "÷", "��", "��", "��", "��", "ͯ", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "׿", "��", "��", "��", "��", "��",
            "ξ", "ʩ", "��", "��", "��", "��", "��", "��", "½", "��", "��", "��"};
    //��1�� 
    static string[] male1Array = {"��", "��", "��", "С", "ǧ", "��", "��", "һ", "��", "Ц", "˫", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "С", "��",
            "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "Ӣ", "��", "��", "ѩ", "��", "��", "��", "��", "��", "Ʈ", "��",
            "��", "��", "ɭ", "��", "˼", "��", "��", "Ԫ", "Ϧ", "��", "��", "��", "��", "��", "а", "��"};
    //��2��        
    static string[] male2Array = {"��", "��", "��", "��", "��", "��", "�", "��", "��", "��", "��", "��", "��", "��", "��", "Ȩ", "��", "��", "��", "��", "��", "ƽ", "��", "��", "ǿ",
            "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "Ȼ", "˳", "��", "��", "��", "��", "��", "־", "��", "��", "��", "��", "Ԫ", "ī", "��", "��", "֮",
            "��", "��", "��", "��", "��", "��", "��", "˪", "ɽ", "��", "�", "ʢ", "�", "��", "��", "��", "ҫ", "��", "��", "��", "��", "��", "�", "��"};
    //Ů1��            
    static string[] female1Array = {"˼", "��", "ҹ", "��", "С", "��", "��", "��", "ӳ", "��", "��", "��", "��", "֮", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��",
            "�", "��", "��", "��", "��", "��", "��", "��", "Ц", "��", "Ԫ", "��", "��", "��", "��", "��", "Ľ", "��", "��", "��", "��", "��", "��", "��", "��", "��", "Ѱ",
            "ˮ", "��", "��", "ϧ", "ʫ", "��", "��", "��", "��", "��", "ѩ", "��", "��", "ӭ", "��", "��", "��", "��", "��", "��", "��", "��"};
    //Ů2��
    static string[] female2Array = {"��", "��", "��", "��", "��", "��", "��", "Ƽ", "��", "��", "��", "˪", "��", "˿", "��", "��", "¶", "��", "ܽ", "��", "��", "��", "��", "��", "��",
            "ѩ", "��", "��", "��", "��", "��", "��", "��", "�", "��", "��", "��", "ͮ", "��", "��", "��", "��", "��", "ɺ", "��", "��", "��", "��", "��", "��", "˫", "��",
            "��", "��", "��", "��", "��", "÷", "��", "ɽ", "��", "֮", "��", "��", "��", "��", "��", "��", "��", "ޱ", "��", "��", "��", "��", "��", "ҹ"};

    /// <summary>
    /// ������ɫʱ�������
    /// </summary>
    public static string RandomName()
    {
        string CurName = "";  //��ǰ������

        string[] CopyArray1;
        string[] CopyArray2;

        bool isMale = UnityEngine.Random.Range(0, 2) == 0;

        //�жϽ�ɫ������Ů
        //if(��ɫ����) ���������鸴�Ƶ�CopyArray��
        if (isMale)
        {
            CopyArray1 = new string[male1Array.Length];
            CopyArray2 = new string[male2Array.Length];
            male1Array.CopyTo(CopyArray1, 0);
            male2Array.CopyTo(CopyArray2, 0);
        }
        else
        {
            CopyArray1 = new string[female1Array.Length];
            CopyArray2 = new string[female2Array.Length];
            female1Array.CopyTo(CopyArray1, 0);
            female2Array.CopyTo(CopyArray2, 0);
        }

        int LastNameNum = 0;  //��������
        int TempRan = UnityEngine.Random.Range(1, 11);
        if (TempRan % 3 == 0)
        {
            LastNameNum = 1;
        }
        else
        {
            LastNameNum = 2;
        }

        //�������+�������(����һ���ֻ���������)
        if (LastNameNum == 1)
        {
            int FirstNameIndex = UnityEngine.Random.Range(0, surnameArray.Length);
            int LastName1 = UnityEngine.Random.Range(0, CopyArray1.Length);
            CurName = surnameArray[FirstNameIndex] + CopyArray1[LastName1];
        }
        else if (LastNameNum == 2)
        {
            int FirstNameIndex = UnityEngine.Random.Range(0, surnameArray.Length);
            int LastName1 = UnityEngine.Random.Range(0, CopyArray1.Length);
            int LastName2 = UnityEngine.Random.Range(0, CopyArray2.Length);
            CurName = surnameArray[FirstNameIndex] + CopyArray1[LastName1] + CopyArray2[LastName2];
        }

        return CurName;
    }

    /// <summary>
    /// ��ȡͼƬ��Դ
    /// </summary>
    /// <param name="type"></param>
    /// <param name="picName"></param>
    /// <returns></returns>
    public static Sprite LoadSprite(SpriteSourceType type, string picName)
    {
        string path = string.Empty;
        Sprite spr = null;
        switch (type)
        {
            case SpriteSourceType.GameLevelIco://��Ϸ�ؿ�ͼƬ
                path = "Download/Source/UISource/GameLevel/GameLevelIco";
                break;
            case SpriteSourceType.GameLevelDetail://��Ϸ�ؿ�ϸ��ͼ
                path = "Download/Source/UISource/GameLevel/GameLevelDetail";
                break;
            case SpriteSourceType.WorldMapIco://�����ͼͼƬ
                path = "Download/Source/UISource/WorldMap";
                break;
            case SpriteSourceType.WorldMapSmall://����С��ͼ
                path = "Download/Source/UISource/SmallMap";
                break;
        }

        AssetBundleMgr.Instance.LoadOrDownload<Texture2D>(string.Format("{0}/{1}.assetbundle",path,picName), picName,
    (Texture2D obj) =>
    {
        var iconRect = new Rect(0, 0, obj.width, obj.height);
        spr = Sprite.Create(obj, iconRect, new Vector2(0.5f, 0.5f));
    }, type: 1);
        return spr;
    }

    /// <summary>
    /// ��ȡ����ͼƬ
    /// </summary>
    /// <param name="goodsId"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Sprite LoadGoodsImg(int goodsId, GoodsType type)
    {
        string pathName = string.Empty;
        Sprite spr = null;
        switch (type)
        {
            case GoodsType.Equip:
                pathName = "Download/Source/UISource/GameLevel/EquipIco";
                break;
            case GoodsType.Item:
                pathName = "Download/Source/UISource/GameLevel/ItemIco";
                break;
            case GoodsType.Material:
                pathName = "Download/Source/UISource/GameLevel/MaterialIco";
                break;
        }
        AssetBundleMgr.Instance.LoadOrDownload<Texture2D>(string.Format("{0}/{1}.assetbundle", pathName, goodsId), goodsId.ToString(),
(Texture2D obj) =>
{
    var iconRect = new Rect(0, 0, obj.width, obj.height);
    spr = Sprite.Create(obj, iconRect, new Vector2(0.5f, 0.5f));
}, type: 1);
        return spr;
    }
    #endregion

    #region ��ȡ��ɫ����״̬
    private static Dictionary<string, RoleAnimatorState> dic;
    
    /// <summary>
    /// ��ȡ��ɫ����״̬�±�
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static RoleAnimatorState GetRoleAnimatorState(RoleAttackType type, int index)
    {
        if (dic == null)
        {
            dic = new Dictionary<string, RoleAnimatorState>();
            dic["PhyAttack1"] = RoleAnimatorState.PhyAttack1;
            dic["PhyAttack2"] = RoleAnimatorState.PhyAttack2;
            dic["PhyAttack3"] = RoleAnimatorState.PhyAttack3;
            dic["Skill1"] = RoleAnimatorState.Skill1;
            dic["Skill2"] = RoleAnimatorState.Skill2;
            dic["Skill3"] = RoleAnimatorState.Skill3;
            dic["Skill4"] = RoleAnimatorState.Skill4;
            dic["Skill5"] = RoleAnimatorState.Skill5;
            dic["Skill6"] = RoleAnimatorState.Skill6;
        }

        string key = string.Format("{0}{1}", type == RoleAttackType.PhyAttack ? "PhyAttack" : "Skill", index);

        if (dic.ContainsKey(key))
        {
            return dic[key];
        }
        return RoleAnimatorState.Skill1;
    }
    #endregion

    #region GetRandomPos ��ȡĿ�����Χ�������
    /// <summary>
    /// ��ȡĿ�����Χ�������
    /// </summary>
    /// <param name="targerPos"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPos(Vector3 targerPos, float distance)
    {
        //1.����һ������
        Vector3 v = new Vector3(0, 0, 1); //z�ᳬǰ��

        //2.��������ת
        v = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360f), 0) * v;

        //3.���� * ����(�뾶) = �����
        Vector3 pos = v * distance * UnityEngine.Random.Range(0.8f, 1f);

        //4.��������� Χ�����ǵ� ��������
        return targerPos + pos;
    }

    public static Vector3 GetRandomPos(Vector3 currPos, Vector3 targerPos, float distance)
    {
        //1.����һ������
        Vector3 v = (currPos - targerPos).normalized;

        //2.��������ת
        v = Quaternion.Euler(0, UnityEngine.Random.Range(-90f, 90f), 0) * v;

        //3.���� * ����(�뾶) = �����
        Vector3 pos = v * distance * UnityEngine.Random.Range(0.8f, 1f);

        //4.��������� Χ�����ǵ� ��������
        return targerPos + pos;
    }
    #endregion

    #region GetPathLen ����·���ĳ���
    /// <summary>
    /// ����·���ĳ���
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static float GetPathLen(List<Vector3> path)
    {
        float pathLen = 0f; //·�����ܳ��� �����·��

        for (int i = 0; i < path.Count; i++)
        {
            if (i == path.Count - 1) continue;

            float dis = Vector3.Distance(path[i], path[i + 1]);
            pathLen += dis;
        }

        return pathLen;
    }
    #endregion

    #region GetFileName ��ȡ�ļ���
    /// <summary>
    /// ��ȡ�ļ���
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileName(string path)
    {
        string fileName = path;
        int lastIndex = path.LastIndexOf('/');
        if (lastIndex > -1)
        {
            fileName = fileName.Substring(lastIndex + 1);
        }

        lastIndex = fileName.LastIndexOf('.');
        if (lastIndex > -1)
        {
            fileName = fileName.Substring(0, lastIndex);
        }

        return fileName;
    }
    #endregion

    #region AutoLoadTexture �Զ�����ͼƬ
    /// <summary>
    /// �Զ�����ͼƬ
    /// </summary>
    /// <param name="go"></param>
    /// <param name="imgPath"></param>
    /// <param name="imgName"></param>
    public static void AutoLoadTexture(GameObject go, string imgPath, string imgName, bool isSetNativeSize)
    {
        if (go != null)
        {
            AutoLoadTexture component = go.GetOrCreatComponent<AutoLoadTexture>();
            if (component != null)
            {
                component.ImgPath = imgPath;
                component.ImgName = imgName;
                component.IsSetNativeSize = isSetNativeSize;
                component.SetImg();
            }
        }
    }
    #endregion

    #region AutoNumberAnimation �Զ����ֶ���
    /// <summary>
    /// �Զ����ֶ���
    /// </summary>
    /// <param name="go"></param>
    /// <param name="number"></param>
    public static void AutoNumberAnimation(GameObject go, int number)
    {
        if (go != null)
        {
            AutoNumberAnimation component = go.GetOrCreatComponent<AutoNumberAnimation>();
            component.DoNumber(number);
        }
    }
    #endregion

    /// <summary>
    /// ���������
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public static GameObject AddChild(Transform parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.SetParent(parent, false);
            go.layer = parent.gameObject.layer;
        }
        return go;
    }
}