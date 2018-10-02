using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLogic : MonoBehaviour {

    bool currentlyVertical;
    protected int progress;
    protected GameObject progressGO;
    protected ProgressMinigame pm;
        
    public void Start()
    {
        progressGO = GameObject.Find("PROGRESS");
        if(progressGO)
            progressGO.SetActive(false);
    }

    public void Update () {
        if (DialogSys.isSelecting())
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Fire1"))
                DialogSys.chooseMultipleChoice();
            if (Input.GetAxis("Vertical") < 0 && !currentlyVertical)
            {
                DialogSys.moveSelectedDown();
                currentlyVertical = true;
            }
            if (Input.GetAxis("Vertical") > 0 && !currentlyVertical)
            {
                DialogSys.moveSelectedUp();
                currentlyVertical = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Fire1"))
                DialogSys.proceed();
        }

        if (currentlyVertical && Input.GetAxis("Vertical") == 0)
            currentlyVertical = false;

        if (Input.GetAxis("Horizontal") != 0f) //WMR: Horizontal
        GameObject.Find("Player").transform.Rotate(0, 40 * Time.deltaTime * Input.GetAxis("Horizontal"), 0);

        // restart level
        if (Input.GetButtonDown("Menu"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void cmd_dark()
    {
        GameObject[] physicalObjects = GameObject.FindGameObjectsWithTag("Physical");
        float timePerObject = 5.0f / physicalObjects.Length;
        foreach (GameObject go in physicalObjects)
            go.GetComponentInChildren<Rigidbody>().useGravity = false;

        for (int i = 0; i < physicalObjects.Length; i++)
            Invoke("regravity", timePerObject * i);


        Physics.gravity = new Vector3(0, 4, 0);
        GameObject.Find("DarkFade").GetComponent<Animator>().SetTrigger("Start");
    }

    public void regravity()
    {
        // find first non-gravity object and reapply gravity
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Physical"))
            if (!go.GetComponentInChildren<Rigidbody>().useGravity)
            {
                go.GetComponentInChildren<Rigidbody>().useGravity = true;
                return;
            }

    }

    public void cmd_addProgress()
    {
        progress++;
    }

    public void runProgressMinigame()
    {
        pm.updateAndDisplay();
        if (!pm.isFinished())
            Invoke("runProgressMinigame", 0.1f);
    }    
}
