
/********************************************************************
	created:	2020/08/30
	created:	30:8:2020   15:53
	filename: 	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts\GlobalVariable.cs
	file path:	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts
	file base:	GlobalVariable
	file ext:	cs
	author:		YYYMarshal
	
	purpose:	
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalVariable
{
    #region 单例
    private static readonly Lazy<GlobalVariable> lazy = new Lazy<GlobalVariable>(() => new GlobalVariable());
    public static GlobalVariable Instance
    {
        get
        {
            return lazy.Value;
        }
    }
    #endregion

    public string PathXml = $"{Application.dataPath}/../XML/YYYXB_Labaction_AND_Condition.xml";
}
