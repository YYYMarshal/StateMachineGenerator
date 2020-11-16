
/********************************************************************
	created:	2020/11/16
	created:	16:11:2020   11:44
	filename: 	E:\DEMO\yuxuebing\StateMachineGenerator\Assets\Scripts\UIController\MenuPanelController.cs
	file path:	E:\DEMO\yuxuebing\StateMachineGenerator\Assets\Scripts\UIController
	file base:	MenuPanelController
	file ext:	cs
	author:		YYYXB
	
	purpose:	
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanelController : MonoBehaviour
{
    #region List
    private readonly List<KeyValuePair<string, string>> listAction =
        new List<KeyValuePair<string, string>>();
    private readonly List<KeyValuePair<string, string>> listCondition =
        new List<KeyValuePair<string, string>>();
    #endregion

    private void Awake()
    {
        gameObject.SetActive(false);
        GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false));

        InitListByXmlFile();
    }
    public void ShowMenuPanel(bool isAction)
    {
        //竟然可以通过自身打开。。。
        gameObject.SetActive(true);
        Transform imgBgTrans = HierarchyObject.Instance.MenuPanel.transform.Find("ImgBg");
        while (imgBgTrans.childCount != 0)
        {
            Destroy(imgBgTrans.GetChild(0).gameObject);
        }
        if (isAction)
        {
        }
        else
        {

        }


        foreach (KeyValuePair<string, string> item in listAction)
        {
            GameObject btnAction = Instantiate(
                Resources.Load<GameObject>("Prefabs/BtnAction"),
                Vector3.zero, Quaternion.identity, imgBgTrans);
            btnAction.transform.GetChild(0).GetComponent<Text>().text =
                item.Key;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(imgBgTrans.GetComponent<RectTransform>());

    }
    private void InitListByXmlFile()
    {
        //通过 XmlReaderSettings 对象来忽略掉注释
        XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
        {
            IgnoreComments = true
        };
        XmlReader xmlReader = XmlReader.Create(GlobalVariable.Instance.PathXml, xmlReaderSettings);
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(xmlReader);
        XmlNodeList nodLst = xmlDocument.SelectSingleNode("YYYXB").ChildNodes;
        foreach (XmlElement elem in nodLst)
        {
            switch (elem.Name)
            {
                case "Action":
                    listAction.Add(
                        new KeyValuePair<string, string>(
                            elem.Attributes["type"].InnerXml, elem.OuterXml));
                    break;
                case "Condition":
                    listCondition.Add(
                        new KeyValuePair<string, string>(
                            elem.Attributes["type"].InnerXml, elem.OuterXml));
                    break;
            }
        }
        xmlReader.Close();
    }
}
