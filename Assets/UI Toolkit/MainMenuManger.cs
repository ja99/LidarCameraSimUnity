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
    private Button updateSettingsButton;

    private RadioButtonGroup radioButtonGroupMode;

    public GameObject changeTrackDocument;


    
    private void UpdateSettings()
    {
        int lidarFrequency = Convert.ToInt32(frequencyCameraTextField.text);
        int cameraFrequency = Convert.ToInt32(frequencyLidarTextField.text);

        DrivingMode mode = (DrivingMode)radioButtonGroupMode.value;

        var output = String.Format("lidar: {0}, camera {1}, drivingMode: {2}", lidarFrequency, cameraFrequency, mode);
        
        Debug.Log(output);

    }
    
    private void ChangeTrack()
    {
        changeTrackDocument.SetActive(true);
    }

    private void ResetCar()
    {
        throw new NotImplementedException();
    } 
    

    private void OnEnable()
    {
        rootVisualElement = this.GetComponent<UIDocument>().rootVisualElement;
        Querries();
        AddListeners();
        
        // How to set the values, when it comes e.g. from a configuration
        SetFrequencyLidar(42);
        SetFrequencyCamera(69);
        SetDrivingMode(DrivingMode.Autonomous);
    }

    private void AddListeners()
    {
        resetCarButton.clicked += ResetCar;
        changeTrackButton.clicked += ChangeTrack;
        updateSettingsButton.clicked += UpdateSettings;

    }

   

    /// <summary>
    /// A Method for assigning all the Buttons of the UI Elements
    /// </summary>
    private void Querries()
    {
        resetCarButton = rootVisualElement.Q<Button>("ButtonResetCar");
        changeTrackButton = rootVisualElement.Q<Button>("ButtonSwitchTrack");
        updateSettingsButton = rootVisualElement.Q<Button>("ButtonUpdateSettings");

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
