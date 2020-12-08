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

    private void Awake()
    {
        InitHierarchyObject();


        gameObject.GetComponent<Button>().onClick.AddListener(() =>
            Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.Save));
    }
    #region ↑↑↑METHOD↑↑↑
    private void InitHierarchyObject()
    {
        HierarchyObject.Instance.GameManagerObject = gameObject;

        btnCreateState = transform.Find("BtnCreateState").GetComponent<Button>();
        btnCreateState.onClick.AddListener(BtnCreateStateOnClick);
        btnCreateState.gameObject.SetActive(false);

        HierarchyObject.Instance.BtnLineGroup = GameObject.Find("BtnLineGroup");
        HierarchyObject.Instance.StateGroup = GameObject.Find("StateGroup");
        HierarchyObject.Instance.PlaneLineGroup = GameObject.Find("PlaneLineGroup");

        Transform canvasTrans = transform.parent;
        HierarchyObject.Instance.ContentPanel = canvasTrans.Find("ContentPanel").gameObject;
        HierarchyObject.Instance.MenuPanel = canvasTrans.Find("MenuPanel").gameObject;
        HierarchyObject.Instance.TopicInfoPanel = canvasTrans.Find("TopicInfoPanel").gameObject;

        //先让其开启一下，是为了让其启用脚本
        //顺序：查找物体---启用物体---添加脚本
        HierarchyObject.Instance.ContentPanel.SetActive(true);
        HierarchyObject.Instance.MenuPanel.SetActive(true);
        HierarchyObject.Instance.TopicInfoPanel.SetActive(true);

        HierarchyObject.Instance.ContentPanel.AddComponent<ContentPanelController>();
        HierarchyObject.Instance.MenuPanel.AddComponent<MenuPanelController>();
        HierarchyObject.Instance.TopicInfoPanel.AddComponent<TopicInfoPanelController>();

        GameObject tools = canvasTrans.Find("ToolsPanel").gameObject;
        tools.SetActive(true);
        tools.AddComponent<Tools>();
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

        //随机颜色的RGB值。即刻得到一个随机的颜色
        float r = UnityEngine.Random.Range(0f, 1f);
        float g = UnityEngine.Random.Range(0f, 1f);
        float b = UnityEngine.Random.Range(0f, 1f);
        Color randomColor = new Color(r, g, b);
        //newItemState.GetComponent<Image>().color = randomColor;
        StateEntity state = new StateEntity
        {
            goItemState = newItemState,
            color = randomColor
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
            //NEW IMPORT
            //MENU: SELECT CSM
            //TOPIC INFO PANEL
            GameObject menuPanel = HierarchyObject.Instance.MenuPanel;
            if (!menuPanel.transform.GetChild(1).gameObject.activeSelf &&
                !menuPanel.transform.GetChild(2).gameObject.activeSelf &&
                !HierarchyObject.Instance.TopicInfoPanel.activeSelf)
            {
                menuPanel.SetActive(!menuPanel.activeSelf);
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
