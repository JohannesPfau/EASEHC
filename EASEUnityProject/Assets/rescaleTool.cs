using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rescaleTool : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            transform.localScale = new Vector3(1f, 1f, 1f) * 1.25f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            transform.localScale = new Vector3(1f, 1f, 1f) * 1.5f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            transform.localScale = new Vector3(1f, 1f, 1f) * 1.75f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            transform.localScale = new Vector3(1f, 1f, 1f) * 2f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            transform.localScale = new Vector3(1f, 1f, 1f) * 2.25f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            transform.localScale = new Vector3(1f, 1f, 1f) * 2.5f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            transform.localScale = new Vector3(1f, 1f, 1f) * 2.75f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            transform.localScale = new Vector3(1f, 1f, 1f) * 3f;
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            transform.localScale = new Vector3(1f, 1f, 1f) * 3.25f;
        }
    }
}
