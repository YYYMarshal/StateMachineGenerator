using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    RectTransform rt = null;
    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        GetOffSetMinMax();
    }

    private void GetOffSetMinMax()
    {
        Debug.Log($"offsetMin : {rt.offsetMin}");
        Debug.Log($"offsetMax : {rt.offsetMax}");
    }
    // Update is called once per frame
    void Update()
    {
        //SetRectTransformSize();
    }
    private void SetRectTransformSize()
    {
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 50);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50);
        
    }
}
