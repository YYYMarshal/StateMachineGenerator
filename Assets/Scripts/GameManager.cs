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
        SetGlobalObject();
        void SetGlobalObject()
        {
            GlobalObject.Instance.SettingPanel = GameObject.Find("SettingPanel").gameObject;
            GlobalObject.Instance.BtnLineGroup = GameObject.Find("BtnLineGroup").gameObject;
            GlobalObject.Instance.StateGroup = GameObject.Find("StateGroup").gameObject;
            GlobalObject.Instance.PlaneLineGroup = GameObject.Find("PlaneLineGroup").gameObject;
        }

        GlobalObject.Instance.StateGroup = GameObject.Find("StateGroup");

        GameObject.Find("SettingPanel").AddComponent<SettingPanel>();

        btnCreateState = GameObject.Find("BtnCreateState").GetComponent<Button>();
        btnCreateState.onClick.AddListener(BtnCreateStateOnClick);
        btnCreateState.gameObject.SetActive(false);

        void BtnCreateStateOnClick()
        {
            btnCreateState.gameObject.SetActive(false);
            GameObject newItemState = Instantiate(
                Resources.Load<GameObject>("Prefabs/ItemState"),
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0),
                Quaternion.identity,
                //将StateGroup作为新生成的ItemState的父物体
                GlobalObject.Instance.StateGroup.transform);
            newItemState.AddComponent<ItemState>();

            StateEntity state = new StateEntity
            {
                goItemState = newItemState,
                iptName = newItemState.transform.Find("IptName").GetComponent<InputField>()
            };
            Entities.Instance.listState.Add(state);
        }
    }
    void Update()
    {
        //Using up is to close the button after the click event of 
        //creating the status button is executed.

        //V2.0 : Don't use Input.GetMouseButtonDown(1) method anymore, 
        //so don't need the comments above.
        if (Input.GetMouseButtonUp(0))
            btnCreateState.gameObject.SetActive(false);
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
