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
using System.Net;
using System.IO;
using System.Text;

public class GameManager : MonoBehaviour, IPointerClickHandler
{
    //创建状态的按钮
    private Button btnCreateState;

    private GameObject goSettingPanel;
    private void Awake()
    {
        Debug.Log(Test());
        string Test()
        {
            string sa = "";
            WebRequest wr = WebRequest.Create("http://note.youdao.com/noteshare?id=776559f906a009afc108ba7aa10ef1c1&sub=C833CC45DFD44CD7B5C39A92024A5CFB");
            Stream s = wr.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(s, Encoding.Default);
            string all = sr.ReadToEnd(); //读取网站的数据
            int start = 0;
            int end = 0;
            while (all.Contains("<i>"))
            {
                start = all.IndexOf("<i>");
                end = all.IndexOf("</i>");
                string content = all.Substring(start + 3, end - start - 3);
                all = all.Substring(end + 1, all.Length - (end + 1));
                sa += content + "。                    ";
            }
            sr.Close();
            s.Close();
            return sa;
        }

        SetHierarchyObject();
        void SetHierarchyObject()
        {
            HierarchyObject.Instance.StateGroup = GameObject.Find("StateGroup");
            HierarchyObject.Instance.TransitionGroup = GameObject.Find("TransitionGroup");
            HierarchyObject.Instance.PlaneLineGroup = GameObject.Find("PlaneLineGroup");

            HierarchyObject.Instance.ContentPanel = transform.parent.Find("ContentPanel").gameObject;
            HierarchyObject.Instance.MenuPanel = transform.parent.Find("MenuPanel").gameObject;

            HierarchyObject.Instance.ContentPanel.AddComponent<ContentPanelController>();
            HierarchyObject.Instance.MenuPanel.AddComponent<MenuPanelController>();
        }

        btnCreateState = GameObject.Find("BtnCreateState").GetComponent<Button>();
        btnCreateState.onClick.AddListener(BtnCreateStateOnClick);
        btnCreateState.gameObject.SetActive(false);

        goSettingPanel = transform.parent.Find("SettingPanel").gameObject;
        goSettingPanel.AddComponent<SettingPanelController>();

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
