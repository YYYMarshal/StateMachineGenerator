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
        SetChildActive(false);
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
    private void BtnNewOnClick()
    {
        SetChildActive(true);
    }
    private void BtnImportOnClick()
    {
        SetChildActive(true);
    }

    private void OnEnable()
    {
    }
    #region 公共函数
    private void SetChildActive(bool isOne)
    {
        btnGroupOne.SetActive(isOne);
        btnGroupTwo.SetActive(!isOne);
    }
    #endregion
}
