
/********************************************************************
	created:	2020/08/29
	created:	29:8:2020   22:37
	filename: 	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts\GameManager.cs
	file path:	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts
	file base:	GameManager
	file ext:	cs
	author:		YYYMarshal
	
	purpose:	Created before the above time
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditorInternal;

public class GameManager : MonoBehaviour, IPointerClickHandler
{
    private GameObject stateGroup;
    private Button btnCreateState;

    private void Awake()
    {
        stateGroup = GameObject.Find("StateGroup");
        btnCreateState = GameObject.Find("BtnCreateState").GetComponent<Button>();
        btnCreateState.onClick.AddListener(BtnCreateStateOnClick);
        btnCreateState.gameObject.SetActive(false);

        GameObject.Find("SettingPanel").AddComponent<SettingPanel>();
    }

    void Start()
    {
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
    private void BtnCreateStateOnClick()
    {
        btnCreateState.gameObject.SetActive(false);
        GameObject goItemState = Instantiate(
            Resources.Load<GameObject>("Prefabs/ItemState"),
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0),
            Quaternion.identity,
            //Set Canvas as its parent
            stateGroup.transform);
        goItemState.AddComponent<ItemState>();

        StateClass state = new StateClass
        {
            goItemState = goItemState,
            iptName = goItemState.transform.Find("IptName").GetComponent<InputField>()
        };
        GlobalVariable.lstState.Add(state);
    }

}
