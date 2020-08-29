
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

public class GameManager : MonoBehaviour
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
    public float minFov = 15f;
    public float maxFov = 90f;
    public float sensitivity = 10f;
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            btnCreateState.gameObject.SetActive(true);
            btnCreateState.transform.localPosition = GetMousePosition2D(true);
        }
        //Using up is to close the button after the click event of 
        //creating the status button is executed
        if (Input.GetMouseButtonUp(0))
            btnCreateState.gameObject.SetActive(false);

        float fov = Camera.main.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;
    }

    private Vector2 GetMousePosition2D(bool isBtn)
    {
        Vector2 transformParentPivot =
            btnCreateState.transform.parent.GetComponent<RectTransform>().pivot;
        Vector2 vec;
        //If it's a "Create State" button, the position of the current button should be
        //set according to the pivot of the parent object; if it's a state picture,
        //the position should be set to the current position of the mouse.
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
        //UnityEditor.AssetDatabase.LoadAssetAtPath
        btnCreateState.gameObject.SetActive(false);
        GameObject itemState = Instantiate(Resources.Load<GameObject>("Prefabs/ItemState"),
            GetMousePosition2D(false), Quaternion.identity);
        itemState.transform.SetParent(goBGGameManager.transform);
        itemState.AddComponent<ItemState>();
    }

}
