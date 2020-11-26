using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class TopicInfoPanel : MonoBehaviour
{
    #region Properties
    private XmlDocument xmlDoc = null;
    private string destFileName = "";
    #endregion
    private Transform imgBg;
    private InputField iptSMName;
    private InputField iptSceneID;
    private void Awake()
    {
        FindObject();
    }
    #region ↑↑↑Method↑↑↑
    private void FindObject()
    {
        imgBg = transform.Find("TopicInfo/SelctDefaultStateMenu/ImgBg");
        iptSMName = transform.Find("TopicInfo/TopicMenu/SMName/IptSMName").GetComponent<InputField>();
        iptSceneID = transform.Find("TopicInfo/TopicMenu/SceneID/IptSceneID").GetComponent<InputField>();
    }
    #endregion
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
        if (!SelectXmlFile(false))
        {
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

        gameObject.SetActive(false);

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
    #region ↑↑↑Method↑↑↑
    /// <summary>
    /// 选择目标xml文件
    /// </summary>
    /// <returns></returns>
    private bool SelectXmlFile(bool isImport)
    {
        //如果成功选择了一次目标文件，则不管是导入后的导出，还是空文件后的导出，都会直接返回true
        if (destFileName == "")
        {
            destFileName = Tools.Instance.OpenSelectFileDialog();
            if (destFileName == "")
            {
                Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.NoSelectXml);
                return false;
            }
            if (!isImport)
                File.Copy(GlobalVariable.Instance.TemplateXmlPath, destFileName, true);
        }
        return true;
    }
    private XmlDocument CreateXmlDoc()
    {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
            IgnoreComments = true
        };

        XmlDocument xmlDoc = new XmlDocument();
        //xmlDoc.Load(XmlReader.Create(GlobalVariable.Instance.TemplateXmlPath, settings));
        XmlReader reader = XmlReader.Create(destFileName, settings);
        xmlDoc.Load(reader);
        reader.Close();
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

}
