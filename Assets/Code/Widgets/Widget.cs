using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// see: https://www.youtube.com/watch?v=kkkmX3_fvfQ

public class Widget : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int EntityId { get; set; }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("right clicked in stock Widget!");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // do nothing
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // do nothing
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // do nothing
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // do nothing
    }
}
