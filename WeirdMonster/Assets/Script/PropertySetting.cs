using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PropertySetting : MonoBehaviourPunCallbacks
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] string propertyKey;
    [SerializeField] float initialValue = 50;
    [SerializeField] float maxValue = 100;
    [SerializeField] float minValue = 0;
    [SerializeField] bool wholeNumber = true;

    private void Start()
    {
        // setup semua UI
        slider.maxValue = maxValue;
        slider.minValue = minValue;
        slider.wholeNumbers = wholeNumber;
        inputField.contentType = wholeNumber ? TMP_InputField.ContentType.IntegerNumber : TMP_InputField.ContentType.DecimalNumber;

        // ambil initial dari server
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(propertyKey, out var value))
        {
            UpdateSliderInputField((float)value);
        }
        else
        {
            // kalau tidak ambil dari inspector
            UpdateSliderInputField(initialValue);
        }

        if (PhotonNetwork.IsMasterClient == false)
        {
            slider.interactable = false;
            inputField.interactable = false;
        }
    }

    public void InputFromSlider(float value)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }

        UpdateSliderInputField(value);
        SetCustomProperty(value);
    }

    public void InputFromField(string stringValue)
    {

        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }

        if (float.TryParse(stringValue, out var floatValue))
        {
            floatValue = Mathf.Clamp(floatValue, slider.minValue, slider.maxValue);
            UpdateSliderInputField(floatValue);
            SetCustomProperty(floatValue);
        }
    }

    private void SetCustomProperty(float value)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        var property = new Hashtable();
        property.Add(propertyKey, value);
        PhotonNetwork.CurrentRoom.SetCustomProperties(property);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.TryGetValue(propertyKey, out var value) && PhotonNetwork.IsMasterClient == false)
        {
            UpdateSliderInputField((float)value);
        }
    }

    private void UpdateSliderInputField(float value)
    {
        var floatValue = (float)value;
        slider.value = floatValue;
        if (wholeNumber)
        {
            inputField.text = (Mathf.RoundToInt(floatValue)).ToString("D");
        }
        else
        {
            inputField.text = floatValue.ToString("F2");
        }
    }
}
