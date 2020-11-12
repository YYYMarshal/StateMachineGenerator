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

    private GameObject imgESC;
    private void Awake()
    {
        SetGlobalObject();
        void SetGlobalObject()
        {
            GlobalObject.Instance.SettingPanel = GameObject.Find("SettingPanel");
            GlobalObject.Instance.BtnLineGroup = GameObject.Find("BtnLineGroup");
            GlobalObject.Instance.StateGroup = GameObject.Find("StateGroup");
            GlobalObject.Instance.PlaneLineGroup = GameObject.Find("PlaneLineGroup");
        }


        GameObject.Find("SettingPanel").AddComponent<SettingPanel>();

        btnCreateState = GameObject.Find("BtnCreateState").GetComponent<Button>();
        btnCreateState.onClick.AddListener(BtnCreateStateOnClick);
        btnCreateState.gameObject.SetActive(false);

        imgESC = transform.parent.Find("ImgESC").gameObject;
        imgESC.transform.Find("BtnGroup/BtnExit").GetComponent<Button>().onClick.AddListener(BtnExitOnClick);
        imgESC.transform.Find("BtnGroup/BtnHelp").GetComponent<Button>().onClick.AddListener(BtnHelpOnClick);

        #region 本地函数：点击事件
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
        void BtnExitOnClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        void BtnHelpOnClick()
        {
            string url =
                "http://note.youdao.com/noteshare?id=776559f906a009afc108ba7aa10ef1c1&sub=C833CC45DFD44CD7B5C39A92024A5CFB";
            Application.OpenURL(url);
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
            imgESC.SetActive(!imgESC.activeSelf);
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
