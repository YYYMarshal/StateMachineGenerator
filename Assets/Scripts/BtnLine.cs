
/********************************************************************
	created:	2020/09/08
	created:	8:9:2020   13:22
	filename: 	E:\DEMO\yuxuebing\StateMachineGenerator\Assets\Scripts\BtnLine.cs
	file path:	E:\DEMO\yuxuebing\StateMachineGenerator\Assets\Scripts
	file base:	BtnLine
	file ext:	cs
	author:		YYYMarshal
	
	purpose:	LineRenderer物体在Plane物体下的索引、BtnLineDel按钮物体在BtnLineDelGroup物体下的索引
                和 LineClass.line 或 LineClass.btnLineDel 在 GlobalVariable.lstLine 中的索引 相同
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BtnLine : MonoBehaviour, IPointerClickHandler
{
    private GameObject goPlaneLineGroup;
    private GameObject goBtnLineGroup;

    private GameObject goSettingPanel;

    private Button btnLineDel;
    private void Awake()
    {
        goPlaneLineGroup = GameObject.Find("PlaneLineGroup");
        goBtnLineGroup = GameObject.Find("BtnLineGroup");

        goSettingPanel = transform.parent.parent.Find("SettingPanel").gameObject;

        transform.GetComponent<Button>().onClick.AddListener(BtnLineOnClick);
        btnLineDel = transform.Find("BtnLineDel").GetComponent<Button>();
        btnLineDel.onClick.AddListener(BtnLineDelOnClick);
        btnLineDel.gameObject.SetActive(false);
    }
    void Update()
    {
    }
    private void BtnLineOnClick()
    {
        if (!goSettingPanel.activeSelf)
            goSettingPanel.SetActive(true);
        int index = transform.GetSiblingIndex();
        goSettingPanel.GetComponent<SettingPanel>().SetSettingPanel(GlobalVariable.Instance.lstLine[index]);
    }
    private void BtnLineDelOnClick()
    {
        int index = transform.GetSiblingIndex();
        Destroy(goPlaneLineGroup.transform.GetChild(index).gameObject);
        Destroy(goBtnLineGroup.transform.GetChild(index).gameObject);
        GlobalVariable.Instance.lstLine.RemoveAt(index);
        btnLineDel.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            btnLineDel.gameObject.SetActive(!btnLineDel.gameObject.activeSelf);
    }
}
