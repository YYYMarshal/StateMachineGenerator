
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

public class LineClass
{
    public LineRenderer line = null;
    //public EdgeCollider2D edge = null;
    public Button btnLineDel = null;
    public GameObject pre;
    public GameObject next;
}
public class StateClass
{
    public GameObject goItemState = null;
    public InputField iptName;
}
public class CurtRelatedClass
{
    public LineRenderer line = null;
    public bool isStartPaint = false;
    public int lineIndex = 0;
}

public class GlobalVariable 
{
    private static readonly Lazy<GlobalVariable> lazy =
        new Lazy<GlobalVariable>(() => new GlobalVariable());
    public static GlobalVariable Instance
    {
        get
        {
            return lazy.Value;
        }
    }
    public CurtRelatedClass curt = new CurtRelatedClass();
    public List<StateClass> lstState = new List<StateClass>();
    public List<LineClass> lstLine = new List<LineClass>();

    public string PathXml = $"{Application.dataPath}/Resources/XML/YYYXB_Labaction_AND_Condition.xml";
}
 