
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineClass
{
    public LineRenderer line = null;
    public EdgeCollider2D edge = null;
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

public class GlobalVariable : MonoBehaviour
{
    public static CurtRelatedClass curt = new CurtRelatedClass();
    public static List<StateClass> lstState = new List<StateClass>();
    public static List<LineClass> lstLine = new List<LineClass>();
}
