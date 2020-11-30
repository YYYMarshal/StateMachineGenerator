/********************************************************************
	created:	2020/08/29
	created:	29:8:2020   22:37
	filename: 	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts\GameManager.cs
	file path:	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts
	file base:	GameManager
	file ext:	cs
	author:		YYYMarshal
	
	purpose:	BGGameManager UI物体挂载这个脚本
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class GameManager : MonoBehaviour, IPointerClickHandler
{
    //创建状态的按钮
    private Button btnCreateState;

    private GameObject goMenuPanel;

    private void Awake()
    {
        SetHierarchyObject();

        GameObject tools = transform.parent.Find("ToolsPanel").gameObject;
        tools.SetActive(true);
        tools.AddComponent<Tools>();

        gameObject.GetComponent<Button>().onClick.AddListener(() =>
            Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.Save));
    }
    #region Method:Awake()
    private void SetHierarchyObject()
    {
        HierarchyObject.Instance.GameManagerObject = gameObject;

        HierarchyObject.Instance.BtnLineGroup = GameObject.Find("BtnLineGroup");
        HierarchyObject.Instance.StateGroup = GameObject.Find("StateGroup");
        HierarchyObject.Instance.PlaneLineGroup = GameObject.Find("PlaneLineGroup");

        HierarchyObject.Instance.ContentPanel = transform.parent.Find("ContentPanel").gameObject;
        goMenuPanel = transform.parent.Find("MenuPanel").gameObject;
        HierarchyObject.Instance.TopicInfoPanel = transform.parent.Find("TopicInfoPanel").gameObject;

        //先让其开启一下，是为了让其启用脚本
        //顺序：查找物体---启用物体---添加脚本
        HierarchyObject.Instance.ContentPanel.SetActive(true);
        goMenuPanel.SetActive(true);
        HierarchyObject.Instance.TopicInfoPanel.SetActive(true);

        HierarchyObject.Instance.ContentPanel.AddComponent<ContentPanelController>();
        goMenuPanel.AddComponent<MenuPanelController>();
        HierarchyObject.Instance.TopicInfoPanel.AddComponent<TopicInfoPanel>();

        btnCreateState = transform.Find("BtnCreateState").GetComponent<Button>();
        btnCreateState.onClick.AddListener(BtnCreateStateOnClick);
        btnCreateState.gameObject.SetActive(false);
    }
    private void BtnCreateStateOnClick()
    {
        btnCreateState.gameObject.SetActive(false);

        GameObject newItemState = Instantiate(
            Resources.Load<GameObject>("Prefabs/ItemState"),
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0),
            Quaternion.identity,
            //将StateGroup作为新生成的ItemState的父物体
            HierarchyObject.Instance.StateGroup.transform);
        newItemState.AddComponent<ItemState>();

        StateEntity state = new StateEntity
        {
            goItemState = newItemState,
        };
        Entities.Instance.listState.Add(state);
    }
    #endregion
    void Update()
    {
        //Using up is to close the button after the click event of 
        //creating the status button is executed.

        //V2.0 : Don't use Input.GetMouseButtonDown(1) method anymore, 
        //so don't need the comments above.
        if (Input.GetMouseButtonUp(0))
        {
            btnCreateState.gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!goMenuPanel.transform.GetChild(1).gameObject.activeSelf &&
                !HierarchyObject.Instance.TopicInfoPanel.activeSelf)
            {
                goMenuPanel.SetActive(!goMenuPanel.activeSelf);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            btnCreateState.gameObject.SetActive(true);
            btnCreateState.transform.position = Input.mousePosition;
        }
    }
}
