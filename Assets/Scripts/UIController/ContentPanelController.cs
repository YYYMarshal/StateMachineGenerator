/********************************************************************
	created:	2020/08/30
	created:	30:8:2020   4:15
	filename: 	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts\SettingPanel.cs
	file path:	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts
	file base:	SettingPanel
	file ext:	cs
	author:		YYYMarshal
	
	purpose:	包含 关闭按钮 和 StateUI 以及 TransitionUI
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ContentPanelController : MonoBehaviour
{
    #region State Properties
    private GameObject goState;
    private Text txtStateName;
    #endregion

    #region Transition Properties
    private GameObject goTransition;
    private Text txtTransitionTopic;
    #endregion

    private void Awake()
    {
        gameObject.SetActive(false);
        transform.Find("BtnCloseSettingPanel").GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false));

        SetStateUI();
        SetTransitionUI();

        void SetStateUI()
        {
            goState = transform.Find("State").gameObject;
            txtStateName = goState.transform.Find("ImgStateName/TxtStateName").GetComponent<Text>();

            goState.transform.Find("BtnAddAction").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!HierarchyObject.Instance.MenuPanel.activeSelf)
                {
                    HierarchyObject.Instance.MenuPanel.GetComponent<MenuPanelController>().ShowMenuPanel(true);
                }
            });
        }
        void SetTransitionUI()
        {
            goTransition = transform.Find("Transition").gameObject;
            txtTransitionTopic = goTransition.transform.Find("ImgLineTopic/TxtTransitionTopic").GetComponent<Text>();
        }
    }
    #region State Methods
    /// <summary>
    /// 重载函数：从 GlobalVariable.Instance.lstLine 中读取数据，并显示到右侧面板上
    /// </summary>
    public void SetContentPanel(StateEntity state)
    {
        ShowUI_StateTransition(true);
        txtStateName.text = $"State Name : \n{state.iptName.text}";
    }
    #endregion

    #region Line Methods
    /// <summary>
    /// 重载函数：从 GlobalVariable.Instance.lstLine 中读取数据，并显示到右侧面板上
    /// </summary>
    public void SetContentPanel(TransitionEntity line)
    {
        ShowUI_StateTransition(false);

        int spLI = line.line.transform.GetSiblingIndex();

        SetTopText();
        void SetTopText()
        {
            StateEntity preStateClass = null;
            StateEntity nextStateClass = null;
            foreach (StateEntity stateClass in Entities.Instance.listState)
            {
                if (line.pre.Equals(stateClass.goItemState))
                    preStateClass = stateClass;
                if (line.next.Equals(stateClass.goItemState))
                    nextStateClass = stateClass;
            }
            txtTransitionTopic.text = $"From : {preStateClass.iptName.text}\n" +
                $"    To : {nextStateClass.iptName.text}";
        }
    }
    #endregion

    #region 公共代码部分
    private void ShowUI_StateTransition(bool isState)
    {
        goState.SetActive(isState);
        goTransition.SetActive(!isState);
    }
    #endregion
}
