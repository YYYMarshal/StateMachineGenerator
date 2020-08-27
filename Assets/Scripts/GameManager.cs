using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject BGGameManager;
    private GameObject btnCreateState;
    private void Awake()
    {
        BGGameManager = GameObject.Find("BGGameManager");
        btnCreateState = GameObject.Find("BtnCreateState");
        BGGameManager.GetComponent<Button>().onClick.AddListener(BGGameManagerOnClick);
        btnCreateState.GetComponent<Button>().onClick.AddListener(BtnCreateStateOnClick);
        btnCreateState.SetActive(false);
        //Button btn = null;
        string str = $"$:{BGGameManager.name}";
        Debug.Log(str);
    }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            btnCreateState.SetActive(true);
            btnCreateState.transform.localPosition = GetMousePosition2D(true);
        }
        //if (Input.GetMouseButtonDown(0) && btnCreateState.activeSelf)
        //    btnCreateState.SetActive(false);
    }

    private Vector2 GetMousePosition2D(bool isReduce)
    {
        float X = 0f;
        float Y = 0f;
        if (isReduce)
        {
            X = Input.mousePosition.x - Screen.width / 2f;
            Y = Input.mousePosition.y - Screen.height / 2f;
        }
        else
        {
            X = Input.mousePosition.x;
            Y = Input.mousePosition.y;
        }
        return new Vector2(X, Y);
    }
    private void BGGameManagerOnClick()
    {
        //if (btnCreateState.activeSelf)
        //    btnCreateState.SetActive(false);
    }
    private void BtnCreateStateOnClick()
    {
        //UnityEditor.AssetDatabase.LoadAssetAtPath
        btnCreateState.SetActive(false);
        GameObject itemState = Instantiate(Resources.Load<GameObject>("Prefabs/ItemState"),
            GetMousePosition2D(false), this.transform.rotation);
        itemState.transform.SetParent(BGGameManager.transform);
        itemState.AddComponent<ItemStateOnDrag>();
    }
}
