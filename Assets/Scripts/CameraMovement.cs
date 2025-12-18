using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CameraMovement : MonoBehaviour
{
    public float keyMoveSpeed = 0.1f;   // 키입력 이동 속도
    public float moveSpeed = 0.5f;      // 이동 속도
    public float rotateSpeed = 3.0f;    // 회전 속도
    public float zoomSpeed = 10.0f;     // 줌 속도

    private Vector3 lastMousePos;


    void Update()
    {
        CameraMove();
    }
    
    private void CameraMove()
    {
        // === 키보드 입력 이동 ===
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 pos = Camera.main.transform.forward * keyMoveSpeed;
            Camera.main.transform.position += pos;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 pos = Camera.main.transform.right * keyMoveSpeed;
            Camera.main.transform.position -= pos;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 pos = Camera.main.transform.forward * keyMoveSpeed;
            Camera.main.transform.position -= pos;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 pos = Camera.main.transform.right * keyMoveSpeed;
            Camera.main.transform.position += pos;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            Vector3 pos = Camera.main.transform.up * keyMoveSpeed;
            Camera.main.transform.position += pos;
        }
        if (Input.GetKey(KeyCode.E))
        {
            Vector3 pos = Camera.main.transform.up * keyMoveSpeed;
            Camera.main.transform.position -= pos;
        }

        // === 마우스 휠 줌 ===
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            Camera.main.transform.position += Camera.main.transform.forward * scroll * zoomSpeed;
        }

        // === 좌클릭 드래그 → 카메라 이동 ===
        if (Input.GetMouseButtonDown(02))
        {
            lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(02))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            Vector3 move = -Camera.main.transform.right * delta.x * Time.deltaTime * moveSpeed
                           - Camera.main.transform.up * delta.y * Time.deltaTime * moveSpeed;
            Camera.main.transform.position += move;
            lastMousePos = Input.mousePosition;
        }

        // === 우클릭 드래그 → 카메라 회전===
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            float rotX = -delta.y * rotateSpeed * Time.deltaTime;
            float rotY = delta.x * rotateSpeed * Time.deltaTime;

            Vector3 euler = Camera.main.transform.eulerAngles;
            euler.x += rotX;
            euler.y += rotY;
            Camera.main.transform.rotation = Quaternion.Euler(euler);

            lastMousePos = Input.mousePosition;
        }
    }
}
