
/********************************************************************
	created:	2020/09/05
	created:	5:9:2020   15:07
	filename: 	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts\BtnLineDel.cs
	file path:	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts
	file base:	BtnLineDel
	file ext:	cs
	author:		YYYMarshal
	
	purpose:	LineRenderer物体在Plane物体下的索引、BtnLineDel按钮物体在BtnLineDelGroup物体下的索引
                和 LineClass.line 或 LineClass.btnLineDel 在 GlobalVariable.lstLine 中的索引 相同
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnLineDel : MonoBehaviour

{
    private GameObject planeLineGroup;
    private GameObject btnLineDelGroiup;
    private void Awake()
    {
        planeLineGroup = GameObject.Find("PlaneLineGroup");
        btnLineDelGroiup = GameObject.Find("BtnLineDelGroiup");

        gameObject.GetComponent<Button>().onClick.AddListener(BtnLineDelOnClick);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void BtnLineDelOnClick()
    {
        int index = transform.GetSiblingIndex();
        Destroy(planeLineGroup.transform.GetChild(index).gameObject);
        Destroy(btnLineDelGroiup.transform.GetChild(index).gameObject);
        GlobalVariable.Instance.lstLine.RemoveAt(index);
    }
}
