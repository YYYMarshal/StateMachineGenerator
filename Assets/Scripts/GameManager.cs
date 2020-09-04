
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
    private GameObject goBGGameManager;
    private Button btnCreateState;

    private void Awake()
    {
        goBGGameManager = GameObject.Find("BGGameManager");
        btnCreateState = GameObject.Find("BtnCreateState").GetComponent<Button>();
        btnCreateState.onClick.AddListener(BtnCreateStateOnClick);
        btnCreateState.gameObject.SetActive(false);
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
            btnCreateState.transform.localPosition = GetMousePosition2D(true);
            //2020-9-1 14:34:52
            //btnCreateState.transform.position = GetMousePosition2D(false);
        }
    }
    private Vector2 GetMousePosition2D(bool isBtn)
    {
        Vector2 transformParentPivot =
            btnCreateState.transform.parent.GetComponent<RectTransform>().pivot;
        Vector2 vec;
        //If it's a "BtnCreateState", the position of the current button should be
        //set according to the pivot of the parent object; if it's a state picture,
        //the position should be set to the current position of the mouse.

        //2020-9-1 14:34:52
        //Because the "BtnCreateState" use the localPosition, the code statement block in "if" must be used;
        //if the "BtnCreateState", the code statement block in "else" can be used, that is,
        //the parameter "false" in passed when calling
        if (isBtn)
        {
            vec = new Vector2(
                Input.mousePosition.x - Screen.width * transformParentPivot.x,
                Input.mousePosition.y - Screen.height * transformParentPivot.y);
        }
        else
            vec = Input.mousePosition;
        return vec;
    }
    private void BtnCreateStateOnClick()
    {
        btnCreateState.gameObject.SetActive(false);
        GameObject goItemState = Instantiate(Resources.Load<GameObject>("Prefabs/ItemState"),
           new Vector3(GetMousePosition2D(false).x, GetMousePosition2D(false).y, 0), Quaternion.identity);
        //Set Canvas as its parent
        goItemState.transform.SetParent(goBGGameManager.transform.parent);
        goItemState.AddComponent<ItemState>();

        StateClass state = new StateClass
        {
            goItemState = goItemState,
            iptName = goItemState.transform.Find("IptName").GetComponent<InputField>()
        };
        GlobalVariable.lstState.Add(state);
    }

}
