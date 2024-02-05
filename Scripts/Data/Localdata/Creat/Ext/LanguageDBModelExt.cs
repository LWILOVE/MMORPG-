using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LanguageDBModel
{
    
    /// <summary>
    /// 当前语言
    /// </summary>
    public Language CurrLanguage
    { get; set; }

    public string GetText(string module, string key)
    {
        if (m_List != null && m_List.Count > 0)
        {
            for (int i = 0; i < m_List.Count; i++)
            {
                //若模块和key都对应上
                if (m_List[i].Module.Equals(module, System.StringComparison.CurrentCultureIgnoreCase) &&
                    m_List[i].Key.Equals(key, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    switch (CurrLanguage)
                    {
                        case Language.CN:
                            return m_List[i].CN;
                        case Language.EN:
                            return m_List[i].EN;
                    }
                }
            }
        }
        return null;
    }

    public string GetText(int id)
    {
        switch (CurrLanguage)
        {
            case Language.CN:
                return m_Dict[id].CN;
            case Language.EN:
                return m_Dict[id].EN;
        }
        return null;
    }
}
