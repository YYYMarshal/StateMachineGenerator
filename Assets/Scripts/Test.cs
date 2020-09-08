using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.hovered.ForEach(item => Debug.Log(item.name));
        Debug.LogWarning(eventData.pointerEnter.transform.name);
        Debug.LogError("DFDF");
    }

    void Start()
    {
    }
    void Update()
    {
    }
}
