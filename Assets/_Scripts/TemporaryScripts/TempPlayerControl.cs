using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;        
    public float mouseSensitivity = 2f; 
    public Transform playerCamera;      
    private float verticalLookRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  
    }

    void Update()
    {
  
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

       
        transform.Rotate(Vector3.up * mouseX);

       
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);


        playerCamera.localEulerAngles = Vector3.right * verticalLookRotation;

  
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.position += move;
    }
}
