using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemStateOnDrag : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        if(Input.GetMouseButton(0))
            SetDraggedPosition(eventData);
    }

    //https://www.cnblogs.com/ylwshzh/p/4460915.html
    private void SetDraggedPosition(PointerEventData eventData)
    {
        RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
        // transform the screen point to world point int rectangle
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTrans, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos))
        {
            rectTrans.position = globalMousePos;
        }
    }
}
