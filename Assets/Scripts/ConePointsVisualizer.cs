using System.Collections;
using System.Collections.Generic;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class ConePointsVisualizer : MonoBehaviour
{
    private Camera _camera;

    public ROSConnection ros;

    [Range(0.01f, 1f)]public float rayDuration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        ros.Subscribe<MString>("conePoints", ConePointsCallback);
    }

    void ConePointsCallback(MString message)
    {
        var conePoints = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConePoint>>(message.data);

        foreach (var conePoint in conePoints)
        {
            Ray ray = _camera.ScreenPointToRay(new Vector2((float)conePoint.x, YFlip((float)conePoint.y, _camera.pixelHeight)));
        
            Color c;
            if (conePoint.cone_type == "blue_cone") c = Color.blue;
            else if (conePoint.cone_type == "yellow_cone") c = Color.yellow;
            else c = Color.red;
        
            UnityEngine.Debug.DrawRay(ray.origin, ray.direction * 100, c, duration:rayDuration);
        }
    }

    private float YFlip(float y, float screenHeight)
    {
        var t = Mathf.InverseLerp(screenHeight, 0, y);
        return Mathf.Lerp(0, screenHeight, t);
    }
}
