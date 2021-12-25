using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class FollowCar : MonoBehaviour
{
    // camera will follow this object
    public Transform Target;
    //camera transform
    public Transform camTransform;
    // offset between camera and target
    public Vector3 Offset;
    // change this value to get desired smoothness
    public float SmoothTime = 0.3f;
 
    // This value will change at the runtime depending on target movement. Initialize with zero vector.
    private Vector3 velocity = Vector3.zero;
    
    public float horizontalSpeed = 10.0F;
    public float verticalSpeed = 10.0F;
    public float zoomSpeed = 2.0F;
    
    private bool isDrag = false;
    
    private void Start()
    {
        Offset = camTransform.position - Target.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDrag = !isDrag;
        }
    }

    private void FixedUpdate()
    {
        // update position
         Vector3 targetPosition = Target.position + Offset;
         camTransform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);
 
        // update rotation
        transform.LookAt(Target);
        
        Rotate();
    }

   

    private void Rotate()
    {
        // Rotate Camera around Gamobject
        // Get Mouse Rotation 
        float h = horizontalSpeed * Input.GetAxis("Mouse X");
        float v = verticalSpeed * Input.GetAxis("Mouse Y");
        float zoom = zoomSpeed * Input.GetAxis("Mouse ScrollWheel");

        if (isDrag)
        {
            //transform.Rotate(v, h, 0);
            var pos1 = transform.position;
            transform.RotateAround(Target.position, Vector3.up, h);
            transform.RotateAround(Target.position, Vector3.forward, v);
            
            // Calculate Vector between Camera and Car
            var direction = transform.position - Target.position;
            transform.position += direction.normalized * zoom * zoomSpeed; 

            var pos2 = transform.position;
            // Calculate Offeset again after rotate
            Offset += pos2 - pos1;
            
        }
    }
}
