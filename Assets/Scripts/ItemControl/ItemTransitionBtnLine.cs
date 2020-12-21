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

    #region CLICK EVENT
    private void BtnLineOnClick()
    {
        int index = transform.GetSiblingIndex();
        if (Entities.Instance.ListTransition[index].pre.transform.Find("IptName").GetComponent<InputField>().text.Trim() == "" ||
           Entities.Instance.ListTransition[index].next.transform.Find("IptName").GetComponent<InputField>().text.Trim() == "")
        {
            Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.StateNameEmpty);
            if (HierarchyObject.Instance.ContentPanel.activeSelf)
                HierarchyObject.Instance.ContentPanel.SetActive(false);
            return;
        }
        HierarchyObject.Instance.ContentPanel.GetComponent<ContentPanelController>().ShowContentPanel(Entities.Instance.ListTransition[index]);
    }
    private void BtnLineDelOnClick()
    {
        int index = transform.GetSiblingIndex();
        Destroy(HierarchyObject.Instance.PlaneLineGroup.transform.GetChild(index).gameObject);
        Destroy(HierarchyObject.Instance.BtnLineGroup.transform.GetChild(index).gameObject);
        Entities.Instance.ListTransition.RemoveAt(index);
        HierarchyObject.Instance.ContentPanel.SetActive(false);
    }
    #endregion
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            btnDelete.gameObject.SetActive(!btnDelete.gameObject.activeSelf);
    }
}
