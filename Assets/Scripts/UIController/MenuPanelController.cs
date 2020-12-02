using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.IO;

public class MenuPanelController : MonoBehaviour
{
    #region BtnGroupOne
    private GameObject btnGroupOne;
    private Button btnExport;
    private Button btnClear;
    private Button btnHelp;
    private Button btnExit;
    #endregion

    #region BtnGroupTwo
    private GameObject btnGroupTwo;
    private Button btnNew;
    private Button btnImport;
    #endregion

    private Transform stateMachineUI;

    private void Awake()
    {
        FindObject();
        ObjectEvent();
        InitObjectActive();
    }
    #region ↑↑↑Method↑↑↑
    private void FindObject()
    {
        btnGroupOne = transform.Find("BtnGroupOne").gameObject;
        btnGroupTwo = transform.Find("BtnGroupTwo").gameObject;

        btnExport = btnGroupOne.transform.Find("BtnExport").GetComponent<Button>();
        btnClear = btnGroupOne.transform.Find("BtnClear").GetComponent<Button>();
        btnHelp = btnGroupOne.transform.Find("BtnHelp").GetComponent<Button>();
        btnExit = btnGroupOne.transform.Find("BtnExit").GetComponent<Button>();

        btnNew = btnGroupTwo.transform.Find("BtnNew").GetComponent<Button>();
        btnImport = btnGroupTwo.transform.Find("BtnImport").GetComponent<Button>();

        stateMachineUI = transform.Find("SelctStateMachineUI");
    }
    private void ObjectEvent()
    {
        btnExport.onClick.AddListener(BtnExportOnClick);
        btnClear.onClick.AddListener(BtnClearOnClick);
        btnHelp.onClick.AddListener(BtnHelpOnClick);
        btnExit.onClick.AddListener(BtnExitOnClick);

        btnNew.onClick.AddListener(BtnNewOnClick);
        btnImport.onClick.AddListener(BtnImportOnClick);
    }
    private void InitObjectActive()
    {
        SetChildUIActive(false, true, false);
    }

    #endregion

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

        STListSort();

        HierarchyObject.Instance.TopicInfoPanel.SetActive(true);
        gameObject.SetActive(false);
    }
    #region ↑↑↑Method↑↑↑
    /// <summary>
    /// 检查所有状态的名称是否为空
    /// </summary>
    /// <returns></returns>
    private bool CheckStateName()
    {
        //判断当前是否没有任何一个状态存在
        if (Entities.Instance.listState.Count == 0)
        {
            Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.NoState);
            return false;
        }

        //判断所有状态的名称是否重复
        HashSet<string> checkSameStateName = new HashSet<string>();
        foreach (StateEntity state in Entities.Instance.listState)
        {
            InputField ipt = state.goItemState.transform.Find("IptName").GetComponent<InputField>();
            if (ipt.text.Trim() == "")
            {
                Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.StateNameEmpty);
                return false;
            }
            if (!checkSameStateName.Add(ipt.text.Trim()))
            {
                Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.SameStateName);
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
            return x.stateName.Trim().CompareTo(y.stateName.Trim());
        });
        Entities.Instance.listTransition.Sort((TransitionEntity x, TransitionEntity y) =>
        {
            int result = x.topic.Trim().Split('#')[0].CompareTo(y.topic.Trim().Split('#')[0]);
            if (result != 0)
                return result;
            return x.topic.Trim().Split('#')[1].CompareTo(y.topic.Trim().Split('#')[1]);
        });
    }
    #endregion
    private void BtnClearOnClick()
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
    #region 公共函数
    private void SetChildUIActive(params bool[] vs)
    {
        btnGroupOne.SetActive(vs[0]);
        btnGroupTwo.SetActive(vs[1]);
        stateMachineUI.gameObject.SetActive(vs[2]);
    }
    #endregion
    private void BtnNewOnClick()
    {
        if (Tools.Instance.SelectXmlFile(false))
        {
            SetChildUIActive(true, false, false);
            gameObject.SetActive(false);
        }
    }
    private void BtnImportOnClick()
    {
        if (!Tools.Instance.SelectXmlFile(true))
            return;

        SetChildUIActive(false, false, true);

        XmlReaderSettings settings = new XmlReaderSettings()
        {
            IgnoreComments = true
        };

        XmlDocument xmlDoc = new XmlDocument();
        XmlReader reader = XmlReader.Create(CurrentVariable.Instance.TargetFileName, settings);
        xmlDoc.Load(reader);
        reader.Close();

        XmlElement elemAppData = xmlDoc.SelectSingleNode("AppData") as XmlElement;

        ShowCustomSelctStateMachineUI(elemAppData);
    }
    #region 导入xml文件后的物体生成
    private void ShowCustomSelctStateMachineUI(XmlElement elemAppData)
    {
        for (int i = 0; i < stateMachineUI.childCount; i++)
        {
            Destroy(stateMachineUI.GetChild(i).gameObject);
        }

        //通过多个CustomStateMachine的StateMachine/StateMachine的name属性的值，生成可供选择的按钮菜单
        foreach (XmlElement csmElem in elemAppData.ChildNodes)
        {
            if (csmElem.Name != "CustomStateMachine")
                continue;
            XmlElement elem = csmElem.SelectSingleNode("StateMachine").SelectSingleNode("StateMachine") as XmlElement;

            string name = elem.GetAttribute("name");
            GameObject goBtnCSMItem = Instantiate(
                Resources.Load<GameObject>("Prefabs/BtnAC"), Vector3.zero,
                Quaternion.identity, stateMachineUI);

            goBtnCSMItem.transform.GetChild(0).GetComponent<Text>().text = name;
            goBtnCSMItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                BtnCSMItemOnClick(elemAppData, name);
                HierarchyObject.Instance.StateGroup.GetComponent<GridLayoutGroup>().enabled = false;
            });
        }
    }
    private void BtnCSMItemOnClick(XmlElement elemAppData, string name)
    {
        XmlElement elemSMChild = null;
        //寻找目标 CustomStateMachine
        foreach (XmlElement csmElem in elemAppData.ChildNodes)
        {
            if (csmElem.Name != "CustomStateMachine")
                continue;
            XmlElement elem = csmElem.SelectSingleNode("StateMachine").SelectSingleNode("StateMachine") as XmlElement;
            if (name == elem.GetAttribute("name"))
            {
                elemSMChild = elem;
                break;
            }
        }

        //重头戏：实例化 state 和 transition
        foreach (XmlElement elem in elemSMChild.ChildNodes)
        {
            if (elem.Name == "State")
            {
                InstantiateState(elem);
            }
            else if (elem.Name == "Transition")
            {
                InstantiateTransition(elem);
            }
        }

        SetChildUIActive(true, false, false);
        gameObject.SetActive(false);
    }
    private void InstantiateState(XmlElement elem)
    {
        GameObject newItemState = Instantiate(
            Resources.Load<GameObject>("Prefabs/ItemState"),
            //将StateGroup作为新生成的ItemState的父物体
            HierarchyObject.Instance.StateGroup.transform);

        HierarchyObject.Instance.StateGroup.GetComponent<GridLayoutGroup>().cellSize =
            newItemState.transform.GetComponent<RectTransform>().rect.size;
        LayoutRebuilder.ForceRebuildLayoutImmediate(HierarchyObject.Instance.StateGroup.GetComponent<RectTransform>());

        string stateName = elem.GetAttribute("name");
        newItemState.transform.Find("IptName").GetComponent<InputField>().text = stateName;
        newItemState.AddComponent<ItemState>();

        StateEntity state = new StateEntity
        {
            goItemState = newItemState,
            stateName = stateName,
            content = elem.InnerXml
        };
        Entities.Instance.listState.Add(state);
    }
    private void InstantiateTransition(XmlElement elem)
    {
        //return;
        LineRenderer lineRenderer = Instantiate(
            Resources.Load<GameObject>("Prefabs/ItemLine"),
            Vector3.zero,
            Quaternion.identity,
            HierarchyObject.Instance.PlaneLineGroup.transform).GetComponent<LineRenderer>();

        float r = UnityEngine.Random.Range(0f, 1f);
        float g = UnityEngine.Random.Range(0f, 1f);
        float b = UnityEngine.Random.Range(0f, 1f);
        Color randomColor = new Color(r, g, b);
        lineRenderer.startColor = randomColor;
        lineRenderer.endColor = randomColor;

        GameObject pre = null;
        GameObject next = null;
        string src = elem.GetAttribute("src");
        string dest = elem.GetAttribute("dest");
        foreach (StateEntity state in Entities.Instance.listState)
        {
            if (state.stateName == src)
            {
                pre = state.goItemState;
            }
            else if (state.stateName == dest)
            {
                next = state.goItemState;
            }
        }

        TransitionEntity transition = new TransitionEntity()
        {
            line = lineRenderer,
            btnLine = null,
            pre = pre,
            next = next,
            topic = src + "#" + dest,
            content = elem.InnerXml
        };

        transition.line.SetPosition(0, GetRayPoint(pre.transform.Find("PaintPos").position));
        transition.line.SetPosition(1, GetRayPoint(next.transform.Find("PaintPos").position));

        InstantiateBtnLine(transition);

        Entities.Instance.listTransition.Add(transition);

        Vector3 GetRayPoint(Vector3 vector3)
        {
            Ray ray = Camera.main.ScreenPointToRay(vector3);
            bool isCollider = Physics.Raycast(ray, out RaycastHit hitInfo);
            if (isCollider)
                return hitInfo.point;
            return Vector3.zero;
        }
    }
    private void InstantiateBtnLine(TransitionEntity transition)
    {
        Vector2 prePos = transition.pre.transform.Find("PaintPos").position;
        Vector2 nextPos = transition.next.transform.Find("PaintPos").position;
        float distanceScale = 0.3f;
        float x = (nextPos.x - prePos.x) * distanceScale + prePos.x;
        float y = (nextPos.y - prePos.y) * distanceScale + prePos.y;

        //Rect rect = GetComponent<RectTransform>().rect;
        //Vector2 leftBottom = new Vector2(prePos.x - rect.width * 0.5f, prePos.y - rect.height * 0.5f);
        //Vector2 rightTop = new Vector2(prePos.x + rect.width * 0.5f, prePos.y + rect.height * 0.5f);
        //if (x > leftBottom.x && x < rightTop.x)
        //{
        //    //x = leftBottom.x;
        //}
        //if (y > leftBottom.y && y < rightTop.y)
        //{
        //    //y = leftBottom.y;
        //}

        GameObject goBtnLine = Instantiate(Resources.Load<GameObject>("Prefabs/BtnLine"),
           new Vector2(x, y), Quaternion.identity, HierarchyObject.Instance.BtnLineGroup.transform);
        goBtnLine.AddComponent<ItemTransitionBtnLine>();
        transition.btnLine = goBtnLine.GetComponent<Button>();
    }
    #endregion

}
