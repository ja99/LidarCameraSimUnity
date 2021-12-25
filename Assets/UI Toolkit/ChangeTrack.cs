using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChangeTrack : MonoBehaviour
{
    private VisualElement rootVisualElement;
    private Button buttonBack;

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
        throw new System.NotImplementedException();
    }

    private void StartSkidpad()
    {
        throw new System.NotImplementedException();
    }

    private void StartAccerleration()
    {
        throw new System.NotImplementedException();
    }
}
