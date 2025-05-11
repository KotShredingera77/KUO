using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;
    [SerializeField] private float lerp=1;
    float xRotation;
    float yRotation;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX*Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY*Time.deltaTime;
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90 );
        transform.rotation =  Quaternion.Slerp(transform.rotation,   Quaternion.Euler( xRotation, yRotation, 0 ),lerp*Time.deltaTime);
        orientation.rotation = Quaternion.Slerp(orientation.rotation,Quaternion.Euler(0, yRotation, 0),lerp*Time.deltaTime);
    }
}
