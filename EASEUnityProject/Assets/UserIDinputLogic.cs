using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserIDinputLogic : MonoBehaviour
{
    public InputField inputField;
    
    // Update is called once per frame
    void Update()
    {
        if((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && inputField.text != "")
        {
            PlayerPrefs.SetString("userID", inputField.text);
            SceneManager.LoadScene("RatingEvaluation_KITCHEN_CLASH_VR");
        }
    }
}
