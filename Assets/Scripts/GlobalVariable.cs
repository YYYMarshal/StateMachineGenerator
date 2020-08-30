
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
    public Vector3 startPos;
    public Vector3 endPos;
    public LineClass(LineRenderer line, Vector3 startPos, Vector3 endPos = default)
    {
        this.line = line;
        this.startPos = startPos;
        this.endPos = endPos;
    }
}
public class ItemStateClass
{
    public GameObject goItemState = null;
    public InputField iptNamme;
    public Vector3 launchPos;
    public Vector3 receivePos;

    public List<LineClass> lstLine = new List<LineClass>();
    public ItemStateClass(GameObject goItemState)
    {
        this.goItemState = goItemState;
        iptNamme = goItemState.transform.GetChild(1).GetComponent<InputField>();
        launchPos = goItemState.transform.GetChild(2).position;
        receivePos = goItemState.transform.GetChild(3).position;
        lstLine.Clear();
    }
}

public class GlobalVariable : MonoBehaviour
{
    //Struct don't require the static modifier
    public struct CurtRelated
    {
        public static LineRenderer currentLine = null;
        public static bool isStartPaint = false;
        public static int curtStateIndex = 0;
        public static int curtLineIndex = 0;
    }
    public static List<ItemStateClass> lstItemState = new List<ItemStateClass>();
}
