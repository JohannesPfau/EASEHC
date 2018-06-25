using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandDisplay : MonoBehaviour {

	public void handDisplay(string text)
    {
        GetComponent<Text>().text = text;
    }
}
