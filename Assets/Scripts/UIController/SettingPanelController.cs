﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.IO;

public class SettingPanelController : MonoBehaviour
{
    private GameObject btnGroup;
    private GameObject topicInfo;

    private Transform imgBg;

    private InputField iptSMName;
    private InputField iptSceneID;
    private void Awake()
    {
        FindObject();
        ObjectEvent();
        InitObjectActive();
    }
    #region Method:Awake()
    private void FindObject()
    {
        btnGroup = transform.Find("BtnGroup").gameObject;
        topicInfo = transform.Find("TopicInfo").gameObject;

        imgBg = transform.Find("TopicInfo/SelctDefaultStateMenu/ImgBg");

        iptSMName = transform.Find("TopicInfo/TopicMenu/SMName/IptSMName").GetComponent<InputField>();
        iptSceneID = transform.Find("TopicInfo/TopicMenu/SceneID/IptSceneID").GetComponent<InputField>();
    }
    private void ObjectEvent()
    {
        transform.Find("BtnGroup/BtnExport").GetComponent<Button>().onClick.AddListener(BtnExportOnClick);
        transform.Find("BtnGroup/BtnNew").GetComponent<Button>().onClick.AddListener(BtnNewOnClick);
        transform.Find("BtnGroup/BtnHelp").GetComponent<Button>().onClick.AddListener(BtnHelpOnClick);
        transform.Find("BtnGroup/BtnExit").GetComponent<Button>().onClick.AddListener(BtnExitOnClick);

        transform.Find("TopicInfo/BtnOK").GetComponent<Button>().onClick.AddListener(BtnOKOnClick);
    }
    private void InitObjectActive()
    {
        gameObject.SetActive(false);
        SetOneTwoActive(true);
    }

    #endregion

    #region BtnGroup
    /// <summary>
    /// 主要检查面板中所有的状态名是否符合要求；
    /// 并对Entities中的所有数据进行排序
    /// </summary>
    private void BtnExportOnClick()
    {
        if (!CheckStateName())
        {
            return;
        }
        SetOneTwoActive(false);

        STListSort();

        ShowSelctDefaultStateMenu();
    }
    #region ↑↑↑Method↑↑↑
    /// <summary>
    /// 检查所有状态的名称是否为空
    /// </summary>
    /// <returns></returns>
    private bool CheckStateName()
    {
        if (Entities.Instance.listState.Count == 0)
        {
            Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.NoState);
            return false;
        }

        HashSet<string> checkSameName = new HashSet<string>();
        foreach (StateEntity state in Entities.Instance.listState)
        {
            InputField ipt = state.goItemState.transform.Find("IptName").GetComponent<InputField>();
            if (ipt.text.Trim() == "")
            {
                Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.StateNameEmpty);
                return false;
            }
            if (!checkSameName.Add(ipt.text.Trim()))
            {
                Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.SameName);
                return false;
            }
        }

        return true;
    }
    /// <summary>
    /// 逐个设置Entities中的 所有 Transition 的topic字段；
    /// 将ListState和ListTransition按照 StateName 进行排序
    /// </summary>
    private void STListSort()
    {
        SetTransitionTopic();
        void SetTransitionTopic()
        {
            foreach (TransitionEntity transition in Entities.Instance.listTransition)
            {
                transition.topic =
                    transition.pre.transform.Find("IptName").GetComponent<InputField>().text + "#" +
                    transition.next.transform.Find("IptName").GetComponent<InputField>().text;
            }
        }
        Entities.Instance.listState.Sort((StateEntity x, StateEntity y) =>
        {
            return x.iptName.text.Trim().CompareTo(y.iptName.text.Trim());
        });
        Entities.Instance.listTransition.Sort((TransitionEntity x,TransitionEntity y) =>
        {
            int result= x.topic.Trim().Split('#')[0].CompareTo(y.topic.Trim().Split('#')[0]);
            if (result != 0)
                return result;
            return x.topic.Trim().Split('#')[1].CompareTo(y.topic.Trim().Split('#')[1]);
        });
    }
    /// <summary>
    /// 打开选择默认状态的UI
    /// </summary>
    private void ShowSelctDefaultStateMenu()
    {
        for (int i = 0; i < imgBg.childCount; i++)
        {
            Destroy(imgBg.GetChild(i).gameObject);
        }

        foreach (StateEntity state in Entities.Instance.listState)
        {
            GameObject toggleState = Instantiate(
                Resources.Load<GameObject>("Prefabs/ToggleState"), Vector3.zero, Quaternion.identity, imgBg);
            toggleState.GetComponent<Toggle>().group = imgBg.GetComponent<ToggleGroup>();
            toggleState.transform.Find("Label").GetComponent<Text>().text = state.iptName.text;
        }
    }
    #endregion

    private void BtnNewOnClick()
    {
        if (HierarchyObject.Instance.ContentPanel.activeSelf)
            HierarchyObject.Instance.ContentPanel.SetActive(false);

        Entities.Instance.listState.Clear();
        Entities.Instance.listTransition.Clear();

        DestroyAllChilds(HierarchyObject.Instance.StateGroup);
        DestroyAllChilds(HierarchyObject.Instance.BtnLineGroup);
        DestroyAllChilds(HierarchyObject.Instance.PlaneLineGroup);

        gameObject.SetActive(false);

        void DestroyAllChilds(GameObject go)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Destroy(go.transform.GetChild(i).gameObject);
            }
        }
    }
    private void BtnHelpOnClick()
    {
        gameObject.SetActive(false);

        string url =
            "http://note.youdao.com/noteshare?id=776559f906a009afc108ba7aa10ef1c1&sub=C833CC45DFD44CD7B5C39A92024A5CFB";
        Application.OpenURL(url);
    }
    private void BtnExitOnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion

    #region TopicInfo
    private XmlDocument xmlDoc = null;
    /// <summary>
    /// 主要跟xml文件有关
    /// </summary>
    private void BtnOKOnClick()
    {
        if (iptSMName.text.Trim() == "" || iptSceneID.text.Trim() == "")
        {
            Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.IptValuteEmpty);
            return;
        }
        if (!SelectXmlFile())
        {
            Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.NoSelectXml);
            return;
        }

        //2020-11-26 08:45:29
        //下面这样实现向同一个xml文件中多次写入 xml内容
        if (xmlDoc == null)
        {
            xmlDoc = CreateXmlDoc();
        }
        XmlElement element = CreateItemTopic(xmlDoc);
        try
        {
            CreateItemContent(element, xmlDoc);
        }
        catch (System.Exception)
        {
            Debug.Log("Export Failure");
        }

        xmlDoc.Save(destFileName);

        //SelectSingleNode("XXX")：只能选择某结点的第一子级层级的结点
        GUIUtility.systemCopyBuffer = (xmlDoc.SelectSingleNode("AppData").SelectSingleNode("CustomStateMachine") as XmlElement).OuterXml;

        SetOneTwoActive(true);
        gameObject.SetActive(false);

    }
    #region ↑↑↑Method↑↑↑
    private string destFileName = "";
    /// <summary>
    /// 选择目标xml文件
    /// </summary>
    /// <returns></returns>
    private bool SelectXmlFile()
    {
        if (destFileName == "")
        {
            destFileName = gameObject.GetComponent<DialogTest>().OpenSelectFileDialog();
            if (destFileName == "")
                return false;
            File.Copy(GlobalVariable.Instance.SourceXmlPath, destFileName, true);
        }
        return true;
    }
    #region 创建Xml
    private XmlDocument CreateXmlDoc()
    {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
            IgnoreComments = true
        };

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(XmlReader.Create(GlobalVariable.Instance.SourceXmlPath, settings));
        return xmlDoc;
    }
    /// <summary>
    /// AppData/CustomStateMachine/StateMachine/StateMachine/
    /// </summary>
    /// <param name="xmlDoc"></param>
    /// <returns></returns>
    private XmlElement CreateItemTopic(XmlDocument xmlDoc)
    {
        XmlElement elemAppdata = xmlDoc.SelectSingleNode("AppData") as XmlElement;

        XmlElement elemA = xmlDoc.CreateElement("CustomStateMachine");
        elemAppdata.AppendChild(elemA);

        XmlElement elemB = xmlDoc.CreateElement("StateMachine");
        elemA.AppendChild(elemB);

        XmlElement elemC = xmlDoc.CreateElement("StateMachine");
        elemC.SetAttribute("name", iptSMName.text);
        elemC.SetAttribute("sceneid", iptSceneID.text);
        elemB.AppendChild(elemC);

        return elemC;
    }
    /// <summary>
    /// xml的每一个State
    /// </summary>
    /// <param name="elemC"></param>
    /// <param name="xmlDoc"></param>
    private void CreateItemContent(XmlElement elemC, XmlDocument xmlDoc)
    {
        string defaultStateName = "";
        for (int i = 0; i < imgBg.childCount; i++)
        {
            if (imgBg.GetChild(i).GetComponent<Toggle>().isOn)
            {
                defaultStateName = imgBg.GetChild(i).Find("Label").GetComponent<Text>().text;
                break;
            }
        }
        for (int i = 0; i < Entities.Instance.listState.Count; i++)
        {
            StateEntity state = Entities.Instance.listState[i];
            XmlElement elem = xmlDoc.CreateElement("State");
            elem.SetAttribute("name", state.iptName.text);
            if (state.iptName.text.Trim() == defaultStateName)
                elem.SetAttribute("isDefaultState", "");
            elem.InnerXml = state.content;
            elemC.AppendChild(elem);
        }
        foreach (TransitionEntity transition in Entities.Instance.listTransition)
        {
            XmlElement elem = xmlDoc.CreateElement("Transition");
            string[] strs = transition.topic.Split('#');
            elem.SetAttribute("src", strs[0]);
            elem.SetAttribute("dest", strs[1]);
            elem.InnerXml = transition.content;
            elemC.AppendChild(elem);
        }
    }
    #endregion
    #endregion

    #endregion
    private void OnEnable()
    {
        SetOneTwoActive(true);
    }
    /// <summary>
    /// 选择显示 BtnGroup 还是 TopicInfo
    /// </summary>
    /// <param name="isBtnGroupShow"></param>
    private void SetOneTwoActive(bool isBtnGroupShow)
    {
        btnGroup.SetActive(isBtnGroupShow);
        topicInfo.SetActive(!isBtnGroupShow);
    }
}
