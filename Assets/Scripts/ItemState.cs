using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ItemState : MonoBehaviour, IDragHandler,IPointerClickHandler
{
    private struct PivotFromBorder
    {
        public float halfWidth;
        public float top;
        public float bottom;
    }
    private PivotFromBorder pivotFromBorder;
    private GameObject goSettingPanel;
    private void Awake()
    {
        Rect rt = GetComponent<RectTransform>().rect;
        pivotFromBorder.halfWidth = rt.width / 2f;
        pivotFromBorder.top = rt.height * (1 - GetComponent<RectTransform>().pivot.y);
        pivotFromBorder.bottom = rt.height * GetComponent<RectTransform>().pivot.y;

        goSettingPanel = transform.parent.Find("SettingPanel").gameObject;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
            SetDraggedPosition(eventData);
    }

    //https://www.cnblogs.com/ylwshzh/p/4460915.html
    private void SetDraggedPosition(PointerEventData eventData)
    {
        Vector2 targetPos;
        RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
        //Transform the screen point to world point int rectangle
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTrans, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint))
        {
            targetPos = worldPoint;

            //Control state objects will not move out of the window
            if (worldPoint.x - pivotFromBorder.halfWidth < 0)
                targetPos.x = pivotFromBorder.halfWidth;
            if (worldPoint.x + pivotFromBorder.halfWidth > Screen.width)
                targetPos.x = Screen.width - pivotFromBorder.halfWidth;

            if (worldPoint.y - pivotFromBorder.bottom < 0)
                targetPos.y = pivotFromBorder.bottom;
            if (worldPoint.y + pivotFromBorder.top > Screen.height)
                targetPos.y = Screen.height - pivotFromBorder.top;
            rectTrans.position = targetPos;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount==2)
            goSettingPanel.SetActive(true);
    }
}
