using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuManger : MonoBehaviour
{

    private VisualElement rootVisualElement;

    private TextField frequencyLidarTextField;
    private TextField frequencyCameraTextField;

    private Button resetCarButton;
    private Button changeTrackButton;

    private RadioButtonGroup radioButtonGroupMode;


    

    private void OnEnable()
    {
        rootVisualElement = this.GetComponent<UIDocument>().rootVisualElement;
        Querries();
        
        
        SetFrequencyLidar(42);
        SetFrequencyCamera(69);
        SetDrivingMode(DrivingMode.Autonomous);
    }

    /// <summary>
    /// A Method for assigning all the Buttons of the UI Elements
    /// </summary>
    private void Querries()
    {
        resetCarButton = rootVisualElement.Q<Button>("ButtonResetCar");
        changeTrackButton = rootVisualElement.Q<Button>("ButtonChangeTrack");

        frequencyLidarTextField = rootVisualElement.Q<TextField>("FrequencyLidar");
        frequencyCameraTextField = rootVisualElement.Q<TextField>("FrequencyCamera");

        radioButtonGroupMode = rootVisualElement.Q<RadioButtonGroup>("RadioButtonGroupMode");
    }


    public void SetFrequencyLidar(int freq)
    {
        frequencyLidarTextField.value = freq.ToString();
    }
    public void SetFrequencyCamera(int freq)
    {
        frequencyCameraTextField.value = freq.ToString();
    }

    public void SetDrivingMode(DrivingMode mode)
    {
        radioButtonGroupMode.value = (int)mode;
    }
}

public enum DrivingMode
{
    Auto, 
    Keyboard,
    Autonomous
}
