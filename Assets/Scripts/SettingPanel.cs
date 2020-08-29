using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    private void Awake()
    {
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(BtnCloseSettingPanelOnClick);
        gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void BtnCloseSettingPanelOnClick()
    {
        this.gameObject.SetActive(false);
    }
}
