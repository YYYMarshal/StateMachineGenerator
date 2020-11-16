using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelController : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
        transform.Find("BtnGroup/BtnExport").GetComponent<Button>().onClick.AddListener(BtnExportOnClick);
        transform.Find("BtnGroup/BtnHelp").GetComponent<Button>().onClick.AddListener(BtnHelpOnClick);
        transform.Find("BtnGroup/BtnExit").GetComponent<Button>().onClick.AddListener(BtnExitOnClick);

        #region 本地函数：点击事件
        void BtnExportOnClick()
        {

        }
        void BtnHelpOnClick()
        {
            string url =
                "http://note.youdao.com/noteshare?id=776559f906a009afc108ba7aa10ef1c1&sub=C833CC45DFD44CD7B5C39A92024A5CFB";
            Application.OpenURL(url);
        }
        void BtnExitOnClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        #endregion
    }
}
