using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Vector3 resetPosition;
    [SerializeField]
    private float mouseSensitivity = 0.25f, movingSpeed;

    private Vector2 mousePosition, lastMousePosition;

    private void Start()
    {
        mousePosition = Input.mousePosition;
    }

    void Update()
    {
        lastMousePosition = mousePosition;
        mousePosition = Input.mousePosition;

        // check if reset
        if (Input.GetKeyDown(KeyCode.Space)) ResetCam();
        // check if rotating
        else if (Input.GetMouseButton(1)) RotateCam();
        // check if moving
        else if (Input.GetMouseButton(1)) MoveCam();

        if (Input.GetMouseButtonDown(0)) DebugMouse();

    }

    private void DebugMouse()
    {
        Debug.Log(mousePosition);
        Debug.Log(Input.mousePosition);
    }

    private void RotateCam()
    {
        Vector2 difference = mousePosition - lastMousePosition;
        transform.Rotate(mouseSensitivity * difference.y, -mouseSensitivity * difference.x, 0f);
        
        //transform.Rotate(-transform.up * difference.x * mouseSensitivity);
        //transform.Rotate(transform.right * difference.y * mouseSensitivity);
    }
    private void MoveCam()
    {

    }

    private void ResetCam()
    {
        transform.position = resetPosition;
        transform.LookAt(Vector3.zero);
    }
}
