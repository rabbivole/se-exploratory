using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;


/*
 * okay i'm going about this incorrectly, i think. i don't think youtube guy's solution is going
 * to work for my setup. so what i think i should do is:
 * - call the radii/heights for all 3 orbits the orbit values. come up with min orbit values,
 * middle orbit values, and max orbit values. store these in code somehow.
 * - when the mousewheel is spun, interpolate towards the next values 'step'
 * 
 * god cinemachine is the shit of my life
 */
public class CameraControl : MonoBehaviour
{
    public bool isDragging = false;
    public PlayerInput input;
    private InputAction cameraPanning;

    // orbit values go top to bottom:
    private float[] minZoomHeights = { 6, 4, 0 };
    private float[] minZoomRadii = { 0.5f, 4, 5 };
    private float[] maxZoomHeights = { 22, 18, 0 };
    private float[] maxZoomRadii = { 0.5f, 16, 20 };

    private float[] currentOrbitRadii = { 0.5f, 11f, 12f };
    private float[] currentOrbitHeights = { 14.5f, 7.5f, 0.5f };
    private float[] targetOrbitRadii = { 0.5f, 11f, 12f };
    private float[] targetOrbitHeights = { 14.5f, 7.5f, 0.5f };

    private float zoomSteps = 20; // how many increments are between min and max
    private float zoomAcceleration = 4f;

    // capture mousewheel events and adjust the zoom index:
    // this is how we'd do it with the new input system which i should probably switch to,
    // so i'm leaving it commented out. consult https://www.youtube.com/watch?v=bVoJ3-BMNi0
    // which i referenced for this whole file
    // (mousewheel data seems device-dependent if i do it the way i'm doing it, which is Bad)
    private float zoomYAxis = 0;
/*    public float ZoomYAxis
    {
        get { return zoomYAxis; }
        set 
        {
            if (zoomYAxis == value) return;
            zoomYAxis = value;
            UpdateZoomIndex(ZoomYAxis);
        }
    }
*/
    public CinemachineFreeLook cam;

    private void Start()
    {
        // to only situationally allow camera control
        CinemachineCore.GetInputAxis = CustomInputAxis;
        cameraPanning = input.actions.FindAction("Pan Camera");
    }

    public void MouseHold(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) // mouse button held
        {
            isDragging = true;
        }
        else if (context.phase == InputActionPhase.Canceled) // mouse button released
        {
            isDragging = false;
        }
    }

    // Intercepts mouse movement and only allows the camera to be orbited if a mouse button
    // is held down. (Also hands Cinemachine new input system data. They *might* have patched it 
    // to handle the new input system, based on this inspector pane? todo investigate later)
    private float CustomInputAxis(string axisName)
    {
        if (isDragging)
        {
            Vector2 cameraDelta = cameraPanning.ReadValue<Vector2>().normalized;
            if (axisName.Equals("Mouse X"))
            {
                return cameraDelta.x;
            }
            else if (axisName.Equals("Mouse Y"))
            {
                return cameraDelta.y;
            }
        }
        return 0;
    }

    void Update()
    {
        /*        if (Input.mouseScrollDelta.y != 0)
                {
                    UpdateZoomIndex(Input.mouseScrollDelta.y);
                }*/

        //Debug.Log(cameraPanning.ReadValue<Vector2>().normalized);
    }

    private void LateUpdate()
    {
        // smoothly lerp orbits towards their target settings, if necessary
        // todo acceleration helps with this, but this still feels a little sluggish. the nature
        // of lerp means it's like, zenos paradox-style infinitely approaching the target.
        // could possibly fix this with coroutines? which might be... messy
        for (int i = 0; i < 3; i++)
        {
            // clamp targets first
            targetOrbitHeights[i] = Mathf.Clamp(targetOrbitHeights[i], minZoomHeights[i], maxZoomHeights[i]);
            targetOrbitRadii[i] = Mathf.Clamp(targetOrbitRadii[i], minZoomRadii[i], maxZoomRadii[i]);

            if (currentOrbitHeights[i] != targetOrbitHeights[i])
            {
                currentOrbitHeights[i] = Mathf.Lerp(currentOrbitHeights[i], 
                    targetOrbitHeights[i], zoomAcceleration * Time.deltaTime);
                cam.m_Orbits[i].m_Height = currentOrbitHeights[i];
            }
            if (currentOrbitRadii[i] != targetOrbitRadii[i])
            {
                currentOrbitRadii[i] = Mathf.Lerp(currentOrbitRadii[i], 
                    targetOrbitRadii[i], zoomAcceleration *  Time.deltaTime);
                cam.m_Orbits[i].m_Radius = currentOrbitRadii[i];
            }
        }
    }

    private void UpdateZoomIndex(float mousewheelAction)
    {
        // shouldn't ever get here, but just in case
        if (mousewheelAction == 0) { return; }
        // we flip the direction here so mouseup = zoom in
        int direction = mousewheelAction < 0 ? 1 : -1;

        // adjust radius and height for each orbit
        for (int i = 0; i < 3; i++)
        {
            // compute how much to change the orbits by
            // todo - if either of these is 0, don't update it
            float heightStep = (maxZoomHeights[i] - minZoomHeights[i]) / zoomSteps;
            float radiusStep = (maxZoomRadii[i] - minZoomRadii[i]) / zoomSteps;

            // if applying this change WOULDN'T go past a min/max, update the target
            // (saved as vars because i was doing some condition checking + we might want to
            // again someday)
            float newOrbitHeight = targetOrbitHeights[i] + direction * heightStep;
            targetOrbitHeights[i] = newOrbitHeight;
            float newOrbitRadius = targetOrbitRadii[i] + direction * radiusStep;
            targetOrbitRadii[i] = newOrbitRadius;
        }
    }
}
