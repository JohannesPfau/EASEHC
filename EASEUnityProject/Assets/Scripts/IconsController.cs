using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconsController : MonoBehaviour {

    public GameObject[] iconTexts;

	// Use this for initialization
	void Start () {
        RndIcon();
    }
	
    public void RndIcon()
    {
        foreach(GameObject go in iconTexts)
            if(go.activeSelf)
                go.SetActive(false);

        iconTexts[Random.Range(0, iconTexts.Length)].SetActive(true);
        Invoke("RndIcon", 4);
    }
}
