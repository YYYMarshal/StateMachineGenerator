﻿/********************************************************************
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

public class ItemTransitionBtnLine : MonoBehaviour, IPointerClickHandler
{
    private Button btnDelete;
    private void Awake()
    {
        transform.GetComponent<Button>().onClick.AddListener(BtnLineOnClick);

        btnDelete = transform.Find("BtnLineDel").GetComponent<Button>();
        btnDelete.onClick.AddListener(BtnLineDelOnClick);
        btnDelete.gameObject.SetActive(false);
    }

    #region 点击事件
    void BtnLineOnClick()
    {
        int index = transform.GetSiblingIndex();
        HierarchyObject.Instance.ContentPanel.GetComponent<ContentPanelController>().ShowContentPanel(Entities.Instance.listTransition[index]);
    }
    void BtnLineDelOnClick()
    {
        int index = transform.GetSiblingIndex();
        Destroy(HierarchyObject.Instance.PlaneLineGroup.transform.GetChild(index).gameObject);
        Destroy(HierarchyObject.Instance.BtnLineGroup.transform.GetChild(index).gameObject);
        Entities.Instance.listTransition.RemoveAt(index);
        HierarchyObject.Instance.ContentPanel.SetActive(false);
    }
    #endregion
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            btnDelete.gameObject.SetActive(!btnDelete.gameObject.activeSelf);
    }
}
