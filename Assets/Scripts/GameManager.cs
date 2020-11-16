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

public class GameManager : MonoBehaviour, IPointerClickHandler
{
    //创建状态的按钮
    private Button btnCreateState;

    private GameObject goSettingPanel;
    private void Awake()
    {
        SetGlobalObject();
        void SetGlobalObject()
        {
            HierarchyObject.Instance.BtnLineGroup = GameObject.Find("BtnLineGroup");
            HierarchyObject.Instance.StateGroup = GameObject.Find("StateGroup");
            HierarchyObject.Instance.PlaneLineGroup = GameObject.Find("PlaneLineGroup");

            HierarchyObject.Instance.ContentPanel = transform.parent.Find("ContentPanel").gameObject;
            HierarchyObject.Instance.MenuPanel = transform.parent.Find("MenuPanel").gameObject;

            HierarchyObject.Instance.ContentPanel.AddComponent<ContentPanelController>();
            HierarchyObject.Instance.MenuPanel.SetActive(false);
        }

        btnCreateState = GameObject.Find("BtnCreateState").GetComponent<Button>();
        btnCreateState.onClick.AddListener(BtnCreateStateOnClick);
        btnCreateState.gameObject.SetActive(false);

        goSettingPanel = transform.parent.Find("SettingPanel").gameObject;
        goSettingPanel.AddComponent<SettingPanelController>();
        goSettingPanel.SetActive(false);

        #region 本地函数：点击事件
        void BtnCreateStateOnClick()
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
                iptName = newItemState.transform.Find("IptName").GetComponent<InputField>()
            };
            Entities.Instance.listState.Add(state);
        }
        #endregion
    }
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
            goSettingPanel.SetActive(!goSettingPanel.activeSelf);
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
