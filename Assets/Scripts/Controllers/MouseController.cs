using UnityEngine;

public class MouseController : MonoBehaviour {

    // The world-position of the mouse last frame.
    Vector3 lastFramePosition;
    Vector3 currFramePosition;

    // The world-position start of our left-mouse drag operation
    //Vector3 dragStartPosition;

    void Update () {

        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        UpdateDragging();
  
        // Save the mouse position from this frame
        // We don't use currFramePosition because we may have moved the camera.
        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;
    }

    void UpdateDragging()
    {
        // If we're over a UI element, then bail out from this.
        //if (EventSystem.current.IsPointerOverGameObject()) {
        //    return;
        //}

        // Start Drag
        if (Input.GetMouseButtonDown(0)) {
            //dragStartPosition = currFramePosition;
        }

  
        // End Drag
        if (Input.GetMouseButtonUp(0)) {
        }

    }

}
