using System;
using UnityEngine;
public class CameraController : MonoBehaviour 
{
    [Header("이동 세팅")] 
    public float moveSpeed = 10f; 
    public float fastSpeed = 2f; 

    [Header("회전 세팅")] 
    public float rotateSpeed = 3f; 

    [Header("줌 세팅")] 
    public float zoomSpeed = 2f; 

    private float yaw; 
    private float pitch; 

    void Start() 
    { 
        Vector3 angles = transform.eulerAngles; 
        yaw = angles.y;
        pitch = angles.x; 
    }

    void Update() 
    {
        HandleKeyboardMove();
        HandleMouseMove(); 
        HandleScrollZoom(); 
    } 

    private void HandleScrollZoom() 
    {
        float scroll = zoomSpeed * Input.GetAxis("Mouse ScrollWheel") * (Input.GetKey(KeyCode.LeftShift) ? fastSpeed : 1f); 
        if (Mathf.Abs(scroll) > 0.01f) 
        {
            transform.position += transform.forward * scroll * zoomSpeed; 
        }
    }

    void HandleKeyboardMove() 
    { 
        if (!Input.anyKey) return; float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? fastSpeed : 1f); Vector3 moveDir = Vector3.zero; 
        if (Input.GetKey(KeyCode.W)) moveDir += transform.forward; 
        if (Input.GetKey(KeyCode.S)) moveDir -= transform.forward; 
        if (Input.GetKey(KeyCode.A)) moveDir -= transform.right; 
        if (Input.GetKey(KeyCode.D)) moveDir += transform.right; 
        if (Input.GetKey(KeyCode.Q)) moveDir -= transform.up; 
        if (Input.GetKey(KeyCode.E)) moveDir += transform.up; transform.position += moveDir * speed * Time.deltaTime; 
    }

    void HandleMouseMove() 
    {
        if (Input.GetMouseButton(01) || Input.GetMouseButton(02)) 
        {
            Cursor.lockState = CursorLockMode.Confined; 
            float mouseX = Input.GetAxis("Mouse X"); float mouseY = Input.GetAxis("Mouse Y");
            if (Input.GetMouseButton(01)) 
            {
                yaw += mouseX * rotateSpeed; 
                pitch -= mouseY * rotateSpeed;
                pitch = Mathf.Clamp(pitch, -90f, 90f); 
                transform.rotation = Quaternion.Euler(pitch, yaw, 0);
            }
            if (Input.GetMouseButton(02)) 
            {
                transform.position += transform.right * mouseX; transform.position += transform.up * mouseY;
            }
        }
        else 
        {
            Cursor.lockState = CursorLockMode.None; 
        }
    }
}