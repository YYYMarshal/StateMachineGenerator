
/********************************************************************
	created:	2020/09/17
	created:	17:9:2020   14:12
	filename: 	E:\DEMO\yuxuebing\StateMachineGenerator\Assets\Scripts\ItemCondition.cs
	file path:	E:\DEMO\yuxuebing\StateMachineGenerator\Assets\Scripts
	file base:	ItemCondition
	file ext:	cs
	author:		YYYMarshal
	
	purpose:	
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCondition : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private GameObject goContent;
    private void Awake()
    {
        goContent = transform.parent.gameObject;
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector2(transform.position.x, Input.mousePosition.y);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(goContent.GetComponent<RectTransform>());
    }
}
