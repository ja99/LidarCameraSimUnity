using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChangeTrack : MonoBehaviour
{
    private VisualElement rootVisualElement;
    private Button buttonBack;

    public TrackManager trackManager;

    private void OnEnable()
    {
        rootVisualElement = this.GetComponent<UIDocument>().rootVisualElement;
        buttonBack = rootVisualElement.Q<Button>("ButtonBack");

        Button buttonAcceleration = rootVisualElement.Q<Button>("ButtonAcceleration");
        Button buttonSkidpad = rootVisualElement.Q<Button>("ButtonSkidpad");
        Button buttonEndurance = rootVisualElement.Q<Button>("ButtonEndurance");

        buttonAcceleration.clicked += StartAccerleration;
        buttonSkidpad.clicked += StartSkidpad;
        buttonEndurance.clicked += StartEndurance;

        buttonBack.clicked += () => {this.gameObject.SetActive(false);};
    }

    private void StartEndurance()
    {
        trackManager.SetTrack("endurance");
    }

    private void StartSkidpad()
    {
        trackManager.SetTrack("skidpad");
    }

    private void StartAccerleration()
    {
        trackManager.SetTrack("acceleration");
    }
}
