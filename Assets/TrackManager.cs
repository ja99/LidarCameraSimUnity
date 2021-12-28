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
    
    public AutoPathFollower autoPathFollower;  // Autopath follower of the car
    

    public void SetTrack(string track)
    {
        // hide all tracks
        TrackEndurance.GetComponent<ConePathPlacer>().holder.SetActive(false);
        TrackAcceleration.GetComponent<ConePathPlacer>().holder.SetActive(false);
        TrackSkidpad.GetComponent<ConePathPlacer>().holder.SetActive(false);

        switch (track)
        {
            case "endurance":
                TrackEndurance.GetComponent<ConePathPlacer>().holder.SetActive(true);
                // Set the path for the Car
                autoPathFollower.pathCreator = TrackEndurance.GetComponent<PathCreator>();
                
                break;
            case "acceleration":
                TrackAcceleration.GetComponent<ConePathPlacer>().holder.SetActive(true);
                autoPathFollower.pathCreator = TrackAcceleration.GetComponent<PathCreator>();

                break;
            case "skidpad":
                TrackSkidpad.GetComponent<ConePathPlacer>().holder.SetActive(true);
                autoPathFollower.pathCreator = TrackSkidpad.GetComponent<PathCreator>();

                break;
            default:
                break;
            // TODO: Reset Car 
            
        }
    }
}
