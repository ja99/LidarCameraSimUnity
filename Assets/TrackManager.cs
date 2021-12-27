using System.Collections;
using System.Collections.Generic;
using PathCreation;
using Track;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This Class is Responsible for changing the Track
/// </summary>
public class TrackManager : MonoBehaviour
{
    public GameObject TrackEndurance;
    public GameObject TrackAcceleration;
    public GameObject TrackSkidpad;

    public AutoPathFollower autoPathFollower;
    

    public void SetTrack(string track)
    {
        TrackEndurance.SetActive(false);
        TrackAcceleration.SetActive(false);
        TrackSkidpad.SetActive(false);

        switch (track)
        {
            case "endurance":
                TrackEndurance.SetActive(true);
                // Set the path for the Car
                autoPathFollower.pathCreator = TrackEndurance.GetComponent<PathCreator>();
                
                break;
            case "acceleration":
                TrackEndurance.SetActive(true);
                autoPathFollower.pathCreator = TrackAcceleration.GetComponent<PathCreator>();

                break;
            case "skidpad":
                TrackSkidpad.SetActive(true);
                autoPathFollower.pathCreator = TrackSkidpad.GetComponent<PathCreator>();

                break;
            default:
                break;
            // TODO: Reset Car 
            
        }
    }
}
