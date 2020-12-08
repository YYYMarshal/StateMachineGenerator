
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
using UnityEngine.UI;

public class ItemState : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    private void Awake()
    {
        transform.Find("IptName").GetComponent<InputField>().onEndEdit.AddListener(
            (value) => Entities.Instance.listState[GetCurtStateIndex()].stateName = value);

        transform.Find("BtnSelected").GetComponent<Button>().onClick.AddListener(BtnStateSelectedOnClick);
        transform.Find("BtnStateDelete").GetComponent<Button>().onClick.AddListener(BtnStateDeleteOnClick);
    }
    #region CLICK EVENT
    //ShowSettingPanel
    private void BtnStateSelectedOnClick()
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
    private void BtnStateDeleteOnClick()
    {
        //2020-12-7 13:19:51
        //线段绘制中，不允许删除状态机
        if (CurrentVariable.Instance.isLineStartDraw)
        {
            Tools.Instance.PlayTipAnimation(GlobalVariable.Instance.DrawingLine);
            return;
        }

        GameObject goState = gameObject;
        //若在for循环中动态删除 Entities.Instance.listLine 的元素，则会导致循环次数与预期不符，
        //因为 Entities.Instance.listLine 的Count在减少
        //If the element of "Entities.Instance.listLine" deleted dynamically in the for loop, The number of
        //cycles is not as expected, because "Entities.Instance.listLine.Count" is decreasing.
        List<TransitionEntity> listTransitionTemp = new List<TransitionEntity>();
        for (int i = 0; i < Entities.Instance.listTransition.Count; i++)
        {
            TransitionEntity transition = Entities.Instance.listTransition[i];
            int curtLineIndex = GetCurtLineIndex(transition.line);
            if (goState == transition.pre || goState == transition.next)
            {
                listTransitionTemp.Add(transition);
                DestroyLineAndBtnLine(curtLineIndex);
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
        if (CurrentVariable.Instance.isLineStartDraw)
        {
            CurrentVariable.Instance.line.SetPosition(1, GetRayPoint(Input.mousePosition));
        }
    }

    #region INTERFACE
    public void OnDrag(PointerEventData eventData)
    {
        //2020-11-16 15:26:39
        //如果不加 !isLineStartPaint 的判断，那么在LineRenderer的绘制过程中（LineRenderer只开始，未结束）去拖拽StateUI，则会报错。
        if (Input.GetMouseButton(0) && !CurrentVariable.Instance.isLineStartDraw)
        {
            gameObject.GetComponent<RectTransform>().position = PositionControlState(eventData);
            PositionControlLine();
        }
    }

    #region ↑↑↑METHOD↑↑↑
    /// <summary>
    /// Control state objects will not move out of the window
    /// </summary>
    /// <returns></returns>
    private Vector2 PositionControlState(PointerEventData eventData)
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
    private void PositionControlLine()
    {
        GameObject goState = gameObject;
        Vector3 targetPos = GetRayPoint(transform.Find("PaintPos").position);
        foreach (TransitionEntity transition in Entities.Instance.listTransition)
        {
            if (goState == transition.pre)
            {
                transition.line.SetPosition(0, targetPos);
            }
            if (goState == transition.next)
            {
                transition.line.SetPosition(1, targetPos);
            }
            PositionControlBtnLine(transition, false);
        }
    }
    #endregion
    public void OnPointerClick(PointerEventData eventData)
    {
        //If right-click to the state image gameobject, start drawing ray.
        //如果不加 !isLineStartPaint 的判断，那么两次 右键鼠标，便会出错
        if (eventData.button == PointerEventData.InputButton.Right &&
            !CurrentVariable.Instance.isLineStartDraw)
        {
            CreateLine();
        }
        //If left-click to the state image gameobject, end drawing ray.
        else if (eventData.button == PointerEventData.InputButton.Left && CurrentVariable.Instance.isLineStartDraw)
        {
            EndDrawRayLine();
        }

    }
    #region ↑↑↑METHOD↑↑↑
    private void CreateLine()
    {
        LineRenderer lineRenderer = Instantiate(
            Resources.Load<GameObject>("Prefabs/ItemLine"),
            Vector3.zero, Quaternion.identity,
            HierarchyObject.Instance.PlaneLineGroup.transform).GetComponent<LineRenderer>();

        Color color = Entities.Instance.listState[GetCurtStateIndex()].color;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        TransitionEntity transition = new TransitionEntity()
        {
            line = lineRenderer,
            pre = gameObject
        };

        transition.line.SetPosition(0, GetRayPoint(transform.Find("PaintPos").position));

        Entities.Instance.listTransition.Add(transition);

        CurrentVariable.Instance.line = transition.line;
        CurrentVariable.Instance.isLineStartDraw = true;
    }
    private void EndDrawRayLine()
    {
        //2020-12-7 12:42:15
        //当前正在绘制的LineRenderer只能是最后一个LineRenderer，所以可以省去CurrentVariable类中的curtLineIndex变量的使用
        int curtDrawingLineIndex = Entities.Instance.listTransition.Count - 1;
        TransitionEntity transition = Entities.Instance.listTransition[curtDrawingLineIndex];
        transition.line.SetPosition(1, GetRayPoint(transform.Find("PaintPos").position));
        CurrentVariable.Instance.isLineStartDraw = false;

        transition.next = gameObject;
        PositionControlBtnLine(transition, true);

        bool isRepeated = false;
        //If the line is repeated, it will be deleted.
        for (int i = 0; i < Entities.Instance.listTransition.Count; i++)
        {
            if (curtDrawingLineIndex == i)
                continue;
            if (Entities.Instance.listTransition[i].pre == transition.pre &&
                Entities.Instance.listTransition[i].next == transition.next)
            {
                isRepeated = true;
                Entities.Instance.listTransition.Remove(transition);
                DestroyLineAndBtnLine(curtDrawingLineIndex);

                //2020-9-2 12:40:01
                //break is necessary!!!
                break;
            }
        }
        //If the line is not repeated, it is judged whether it is self jump. If so, delete.
        if (!isRepeated && transition.pre == transition.next)
        {
            Entities.Instance.listTransition.Remove(transition);
            DestroyLineAndBtnLine(curtDrawingLineIndex);
        }
    }
    #endregion
    #endregion

    #region REUSE FUNCTION
    /// <summary>
    /// 控制LineRenderer线段中间位置的BtnLine的位置或实例化
    /// </summary>
    private void PositionControlBtnLine(TransitionEntity transition, bool isCreate)
    {
        Vector2 prePos = transition.pre.transform.Find("PaintPos").position;
        Vector2 nextPos = transition.next.transform.Find("PaintPos").position;
        float distanceScale = 0.3f;
        float x = (nextPos.x - prePos.x) * distanceScale + prePos.x;
        float y = (nextPos.y - prePos.y) * distanceScale + prePos.y;

        //目标坐标与当前坐标差的向量
        Vector3 targetDir = nextPos - prePos;
        //返回当前坐标与目标坐标的角度
        float angle = Vector3.Angle(Vector3.right, targetDir);
        if (nextPos.y < prePos.y)
            angle = -angle;

        if (isCreate)
        {
            GameObject goBtnLine = Instantiate(Resources.Load<GameObject>("Prefabs/BtnLine"),
               new Vector2(x, y), Quaternion.Euler(new Vector3(0, 0, angle - 45)),
               HierarchyObject.Instance.BtnLineGroup.transform);
            goBtnLine.AddComponent<ItemTransitionBtnLine>();
            transition.btnLine = goBtnLine.GetComponent<Button>();
        }
        else
        {
            transition.btnLine.transform.position = new Vector2(x, y);
        }
        transition.btnLine.transform.localEulerAngles = new Vector3(0, 0, angle - 45);

        Transform btnLineDelTrans = transition.btnLine.transform.Find("BtnLineDel");
        btnLineDelTrans.eulerAngles = Vector3.zero;
        Transform btnLineTrans = transition.btnLine.transform;
        btnLineDelTrans.position = new Vector3(
            btnLineTrans.position.x,
            btnLineTrans.position.y - btnLineDelTrans.GetComponent<RectTransform>().rect.height * 0.5f - btnLineTrans.GetComponent<RectTransform>().rect.height * 0.5f,
            btnLineDelTrans.position.z);
    }
    private void DestroyLineAndBtnLine(int curtLineIndex)
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
    /// 如果当前的 ItemState 游戏物体与 listState 中的某个物体相等，则返回其索引值
    /// </summary>
    /// <returns></returns>
    private int GetCurtStateIndex()
    {
        return Entities.Instance.listState.FindIndex(
            (StateEntity state) => gameObject.Equals(state.goItemState));

        //for (int i = 0; i < Entities.Instance.listState.Count; i++)
        //{
        //    if (gameObject.Equals(Entities.Instance.listState[i].goItemState))
        //    {
        //        return i;
        //    }
        //}
        //return -1;
    }
    private int GetCurtLineIndex(LineRenderer line)
    {
        return Entities.Instance.listTransition.FindIndex(
            (TransitionEntity transition) => line.Equals(transition.line));

        //for (int i = 0; i < Entities.Instance.listTransition.Count; i++)
        //{
        //    if (line == Entities.Instance.listTransition[i].line)
        //        return i;
        //}
        //return -1;
    }
    #endregion
}
