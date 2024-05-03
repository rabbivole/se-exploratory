using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResourceNode : Widget
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("right clicked in ResourceNode!");
        }
    }
}
