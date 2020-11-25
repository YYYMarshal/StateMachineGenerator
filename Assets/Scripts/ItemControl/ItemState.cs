﻿
/********************************************************************
	created:	2020/08/30
	created:	30:8:2020   4:15
	filename: 	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts\ItemState.cs
	file path:	F:\YYYMARS\DEMO\Unity\StateMachineGenerator\Assets\Scripts
	file base:	ItemState
	file ext:	cs
	author:		YYYMarshal
	
	purpose:	ItemState预制体
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ItemState : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    private void Awake()
    {
        transform.Find("IptName").GetComponent<InputField>().onEndEdit.AddListener(
            (value) => Entities.Instance.listState[GetCurtStateIndex()].iptName.text = value);

        transform.Find("BtnSelected").GetComponent<Button>().onClick.AddListener(BtnStateSelectedOnClick);

        transform.Find("BtnStateDelete").GetComponent<Button>().onClick.AddListener(BtnStateDeleteOnClick);
    }
    #region 点击事件
    //ShowSettingPanel
    void BtnStateSelectedOnClick()
    {
        if (transform.Find("IptName").GetComponent<InputField>().text.Trim() == "")
        {
            Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.StateNameEmpty);
            if (HierarchyObject.Instance.ContentPanel.activeSelf)
                HierarchyObject.Instance.ContentPanel.SetActive(false);
            return;
        }
        HierarchyObject.Instance.ContentPanel.GetComponent<ContentPanelController>().ShowContentPanel(Entities.Instance.listState[GetCurtStateIndex()]);
    }
    void BtnStateDeleteOnClick()
    {
        GameObject state = gameObject;
        //若在for循环中动态删除 GlobalVariable.Instance.lstLine 的元素，则会导致循环次数与预期不符，
        //因为 GlobalVariable.Instance.lstLine 的Count在减少
        //If the element of "GlobalVariable.Instance.lstLine" deleted dynamically in the for loop, The number of
        //cycles is not as expected, because "GlobalVariable.Instance.lstLine.Count" is decreasing.
        List<TransitionEntity> listTransitionTemp = new List<TransitionEntity>();
        for (int i = 0; i < Entities.Instance.listTransition.Count; i++)
        {
            TransitionEntity transition = Entities.Instance.listTransition[i];
            int curtLineIndex = GetCurtLineIndex(transition.line);
            if (state == transition.pre)
            {
                listTransitionTemp.Add(transition);
                DestroyLineAndBtnDel(curtLineIndex);
            }
            else if (state == transition.next)
            {
                listTransitionTemp.Add(transition);
                DestroyLineAndBtnDel(curtLineIndex);
            }
        }
        for (int i = 0; i < listTransitionTemp.Count; i++)
            Entities.Instance.listTransition.Remove(listTransitionTemp[i]);
        listTransitionTemp.Clear();

        Destroy(gameObject);
        Entities.Instance.listState.Remove(Entities.Instance.listState[GetCurtStateIndex()]);
        HierarchyObject.Instance.ContentPanel.SetActive(false);
    }
    #endregion
    private void Update()
    {
        if (CurrentVariable.Instance.isLineStartPaint)
        {
            CurrentVariable.Instance.line.SetPosition(1, GetRayPoint(Input.mousePosition));
        }
    }

    #region Interface
    public void OnDrag(PointerEventData eventData)
    {
        //2020-11-16 15:26:39
        //如果不加 !isLineStartPaint 的判断，那么在LineRenderer的绘制过程中（LineRenderer只开始，未结束）去拖拽StateUI，则会报错。
        if (Input.GetMouseButton(0) && !CurrentVariable.Instance.isLineStartPaint)
        {
            RectTransform stateRectTrans = gameObject.GetComponent<RectTransform>();

            stateRectTrans.position = StatePositionControl(eventData);

            LinePositionControl();
        }
    }

    #region Method:OnDarg()
    /// <summary>
    /// Control state objects will not move out of the window
    /// </summary>
    /// <returns></returns>
    private Vector2 StatePositionControl(PointerEventData eventData)
    {
        //2020-11-23 16:28:42
        //新的解决方案
        Vector3 pos = transform.position;
        Vector2 targetPos = new Vector2(pos.x + eventData.delta.x, pos.y + eventData.delta.y);
        Vector2 half = new Vector2(
            gameObject.GetComponent<RectTransform>().rect.width * 0.5f,
            gameObject.GetComponent<RectTransform>().rect.height * 0.5f);

        if (targetPos.x - half.x < 0)
            targetPos.x = half.x;
        if (targetPos.x + half.x > Screen.width)
            targetPos.x = Screen.width - half.x;

        if (targetPos.y - half.y < 0)
            targetPos.y = half.y;
        if (targetPos.y + half.y > Screen.height)
            targetPos.y = Screen.height - half.y;
        return targetPos;
    }
    private void LinePositionControl()
    {
        GameObject goState = gameObject;
        foreach (TransitionEntity transition in Entities.Instance.listTransition)
        {
            if (goState == transition.pre)
            {
                transition.line.SetPosition(0, GetRayPoint(transform.Find("PaintPos").position));
            }
            if (goState == transition.next)
            {
                transition.line.SetPosition(1, GetRayPoint(transform.Find("PaintPos").position));
            }
            BtnLinePositionControl(transition, false);
        }
    }
    #endregion
    public void OnPointerClick(PointerEventData eventData)
    {
        //If right-click to the state image gameobject, start drawing ray.
        //如果不加 !isLineStartPaint 的判断，那么两次 右键鼠标，便会出错
        if (eventData.button == PointerEventData.InputButton.Right &&
            !CurrentVariable.Instance.isLineStartPaint)
        {
            CreateLine();
        }
        //If left-click to the state image gameobject, end drawing ray.
        else if (eventData.button == PointerEventData.InputButton.Left && CurrentVariable.Instance.isLineStartPaint)
        {
            EndDrawRay();
        }

    }
    #region Method:OnPointerClick()
    void CreateLine()
    {
        LineRenderer lineRenderer = Instantiate(
            Resources.Load<GameObject>("Prefabs/ItemLine"),
            Vector3.zero,
            Quaternion.identity,
            HierarchyObject.Instance.PlaneLineGroup.transform).GetComponent<LineRenderer>();

        TransitionEntity transition = new TransitionEntity()
        {
            line = lineRenderer,
            pre = gameObject
        };

        transition.line.SetPosition(0, GetRayPoint(transform.Find("PaintPos").position));

        Entities.Instance.listTransition.Add(transition);

        CurrentVariable.Instance.line = lineRenderer;
        CurrentVariable.Instance.isLineStartPaint = true;
        CurrentVariable.Instance.itemLineIndex = GetCurtLineIndex(lineRenderer);
    }
    void EndDrawRay()
    {
        int curtLineIndex = CurrentVariable.Instance.itemLineIndex;
        TransitionEntity transition = Entities.Instance.listTransition[curtLineIndex];
        transition.line.SetPosition(1, GetRayPoint(transform.Find("PaintPos").position));
        CurrentVariable.Instance.isLineStartPaint = false;

        transition.next = gameObject;
        BtnLinePositionControl(transition);

        bool isRepeated = false;
        //If the line is repeated, it will be deleted.
        for (int i = 0; i < Entities.Instance.listTransition.Count; i++)
        {
            if (curtLineIndex == i)
                continue;
            if (Entities.Instance.listTransition[i].pre == transition.pre &&
                Entities.Instance.listTransition[i].next == transition.next)
            {
                isRepeated = true;
                Entities.Instance.listTransition.Remove(transition);
                DestroyLineAndBtnDel(curtLineIndex);

                //2020-9-2 12:40:01
                //break is necessary!!!
                break;
            }
        }
        //If the line is not repeated, it is judged whether it is self jump. If so, delete.
        if (!isRepeated && transition.pre == transition.next)
        {
            Entities.Instance.listTransition.Remove(transition);
            DestroyLineAndBtnDel(curtLineIndex);
        }
    }
    #endregion
    #endregion
    #region 公共代码部分
    /// <summary>
    /// 控制LineRenderer线段中间位置的BtnLine的位置或实例化
    /// </summary>
    private void BtnLinePositionControl(TransitionEntity transition, bool isCreate = true)
    {
        Vector2 prePos = transition.pre.transform.Find("PaintPos").position;
        Vector2 nextPos = transition.next.transform.Find("PaintPos").position;
        float distanceScale = 0.2f;
        float x = (nextPos.x - prePos.x) * distanceScale + prePos.x;
        float y = (nextPos.y - prePos.y) * distanceScale + prePos.y;

        Rect rect = GetComponent<RectTransform>().rect;
        Vector2 leftBottom = new Vector2(prePos.x - rect.width * 0.5f, prePos.y - rect.height * 0.5f);
        Vector2 rightTop = new Vector2(prePos.x + rect.width * 0.5f, prePos.y + rect.height * 0.5f);
        if (x > leftBottom.x && x < rightTop.x)
        {
            //x = leftBottom.x;
        }
        if (y > leftBottom.y && y < rightTop.y)
        {
            //y = leftBottom.y;
        }
        if (isCreate)
        {
            GameObject goBtnLine = Instantiate(Resources.Load<GameObject>("Prefabs/BtnLine"),
               new Vector2(x, y), Quaternion.identity, HierarchyObject.Instance.BtnLineGroup.transform);
            goBtnLine.AddComponent<ItemTransitionBtnLine>();
            transition.btnLine = goBtnLine.GetComponent<Button>();
        }
        else
        {
            transition.btnLine.transform.position = new Vector2(x, y);
        }

    }
    private void DestroyLineAndBtnDel(int curtLineIndex)
    {
        //因为生成LineRenderer物体后该LineRenderer便开始绘制，在该LineRenderer物体结束绘制后，会生成BtnLine物体，
        //则这两个物体在其父物体上的索引是一样的，可以用相同的索引值来删除物体
        Destroy(HierarchyObject.Instance.BtnLineGroup.transform.GetChild(curtLineIndex).gameObject);
        Destroy(HierarchyObject.Instance.PlaneLineGroup.transform.GetChild(curtLineIndex).gameObject);
    }
    /// <summary>
    /// LineRenderer所需的点的位置与UI位置不同，需要转换成RaycastHit.point
    /// </summary>
    /// <param name="vector3"></param>
    /// <returns></returns>
    private Vector3 GetRayPoint(Vector3 vector3)
    {
        Ray ray = Camera.main.ScreenPointToRay(vector3);
        bool isCollider = Physics.Raycast(ray, out RaycastHit hitInfo);
        if (isCollider)
            return hitInfo.point;
        return Vector3.zero;
    }
    #endregion
    #region GetCurt
    /// <summary>
    /// 如果当前的ItemState游戏物体与 listState中的 某个物体相等，则返回其索引值
    /// </summary>
    /// <returns></returns>
    private int GetCurtStateIndex()
    {
        for (int i = 0; i < Entities.Instance.listState.Count; i++)
        {
            if (gameObject.Equals(Entities.Instance.listState[i].goItemState))
                return i;
        }
        return -1;
    }
    private int GetCurtLineIndex(LineRenderer line)
    {
        for (int i = 0; i < Entities.Instance.listTransition.Count; i++)
        {
            if (line == Entities.Instance.listTransition[i].line)
                return i;
        }
        return -1;
    }
    #endregion
}
