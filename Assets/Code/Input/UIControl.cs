using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    private bool debugOpen = false;
    public GameObject debugMenu;
    // i NEED to switch to the new input system. holy lord i want to use events for input
    // rather than a giant if-else tree every frame

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.BackQuote))
        {
            Debug.Log("f");
            debugOpen = debugOpen ? false : true;
            debugMenu.SetActive(debugOpen);
        }
    }

    public void OnClickDebugButton()
    {
        Debug.Log("debug button clicked!");
    }
}
