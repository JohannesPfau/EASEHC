using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserIDinputLogic : MonoBehaviour
{
    public InputField inputField;

    private void Start()
    {
        // shortcut:
        PlayerPrefs.SetString("userID", Random.Range(100,int.MaxValue)+"");
        SceneManager.LoadScene("RatingEvaluation_KITCHEN_CLASH_VR");
    }

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
