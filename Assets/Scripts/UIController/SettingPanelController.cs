using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.IO;

public class SettingPanelController : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
        transform.Find("BtnGroup/BtnExport").GetComponent<Button>().onClick.AddListener(BtnExportOnClick);
        transform.Find("BtnGroup/BtnNew").GetComponent<Button>().onClick.AddListener(BtnNewOnClick);
        transform.Find("BtnGroup/BtnHelp").GetComponent<Button>().onClick.AddListener(BtnHelpOnClick);
        transform.Find("BtnGroup/BtnExit").GetComponent<Button>().onClick.AddListener(BtnExitOnClick);
    }

    private bool isFirst = true;
    private string destFileName = "";
    #region 点击事件
    private void BtnExportOnClick()
    {
        if (isFirst)
        {
            destFileName = gameObject.GetComponent<DialogTest>().OpenSelectFileDialog();
            File.Copy(GlobalVariable.Instance.sourceXmlPath, destFileName, true);
            isFirst = false;
        }

        SetTransitionTopic();

        XmlDocument xmlDoc = CreateXmlDoc();
        XmlElement element = CreateXmlTopic(xmlDoc);
        try
        {
            CreateXmlContent(element, xmlDoc);
        }
        catch (System.Exception)
        {
            Debug.Log("Export Failure");
        }

        xmlDoc.Save(destFileName);

        //SelectSingleNode("XXX")：只能选择某结点的第一子级层级的结点
        GUIUtility.systemCopyBuffer = (xmlDoc.SelectSingleNode("AppData").SelectSingleNode("CustomStateMachine") as XmlElement).OuterXml;

        gameObject.SetActive(false);

        void SetTransitionTopic()
        {
            foreach (TransitionEntity transition in Entities.Instance.listTransition)
            {
                transition.topic = transition.pre.transform.Find("IptName").GetComponent<InputField>().text + "#" + transition.next.transform.Find("IptName").GetComponent<InputField>().text;
            }
        }
    }

    #region 创建Xml
    private XmlDocument CreateXmlDoc()
    {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
            IgnoreComments = true
        };

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(XmlReader.Create(GlobalVariable.Instance.sourceXmlPath, settings));
        return xmlDoc;
    }
    /// <summary>
    /// AppData/CustomStateMachine/StateMachine/StateMachine/
    /// </summary>
    /// <param name="xmlDoc"></param>
    /// <returns></returns>
    private XmlElement CreateXmlTopic(XmlDocument xmlDoc)
    {
        XmlElement elemAppdata = xmlDoc.SelectSingleNode("AppData") as XmlElement;

        XmlElement elemA = xmlDoc.CreateElement("CustomStateMachine");
        elemAppdata.AppendChild(elemA);

        XmlElement elemB = xmlDoc.CreateElement("StateMachine");
        elemA.AppendChild(elemB);

        XmlElement elemC = xmlDoc.CreateElement("StateMachine");
        elemC.SetAttribute("name", "StateMachineName");
        elemC.SetAttribute("sceneid", "1");
        elemB.AppendChild(elemC);

        return elemC;
    }
    /// <summary>
    /// 每一个State
    /// </summary>
    /// <param name="elemC"></param>
    /// <param name="xmlDoc"></param>
    private void CreateXmlContent(XmlElement elemC, XmlDocument xmlDoc)
    {
        foreach (StateEntity state in Entities.Instance.listState)
        {
            XmlElement elem = xmlDoc.CreateElement("State");
            elem.SetAttribute("name", state.iptName.text);
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
}
