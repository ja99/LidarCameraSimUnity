using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraExternalCalibration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 6; x <= 16; x += 5)
        {
            for (int y = -3; y <= 3; y += 3)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = Vector3.one * 0.1f;
                cube.transform.position = new Vector3(y + transform.position.x, 0, x + transform.position.z);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}