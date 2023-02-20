using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAround : MonoBehaviour
{
    
    [Header("Sensitivity")]
    [SerializeField] float zoomSpeed;
    [SerializeField] float _mouseX_Sensitivity;
    [SerializeField] float _mouseY_Sensitivity;

    [Header("ClampCameraUpDown")]
    [SerializeField] float maxClamp;
    [SerializeField] float minClamp;

    [Header("Target")]
    float _rotationY;
    float _rotationX;

    [Header("Zoom")]
    [SerializeField] int a;

    [Header("Move")]
    [SerializeField] float speedX;
    [SerializeField] float speedZ;
    bool bootSpeed;
    void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //LookWithMouse();
        //Move();
        Zoom();
    }

    void LookWithMouse()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * _mouseX_Sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _mouseY_Sensitivity;

        _rotationY += mouseX;
        _rotationX -= mouseY;

        _rotationX = Mathf.Clamp(_rotationX, minClamp, maxClamp);
        transform.localEulerAngles = new Vector3(_rotationX, _rotationY, 0);
    }

    void Move()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            speedX *= 2;
            speedZ *= 2;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            speedX /= 2;
            speedZ /= 2;
        }
        float horizontrol = Input.GetAxis("Horizontal") * Time.deltaTime * speedX;
        float vertical = Input.GetAxis("Vertical") * Time.deltaTime * speedZ;
        transform.position += (transform.forward * vertical) + (transform.right * horizontrol);
    }

    void Zoom()
    {
        
    }
}
