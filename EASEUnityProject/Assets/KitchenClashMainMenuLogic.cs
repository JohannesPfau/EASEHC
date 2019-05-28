using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenClashMainMenuLogic : MonoBehaviour {
    
    int progress = 0;
    public GameObject plane1;
    public GameObject plane2;
    public GameObject spawnGO;
    public GameObject stage2;
    public GameObject stage2_1;
    public GameObject stage2_2;
    public GameObject stage2_3;
    public GameObject stage2_4;
    public GameObject stage2_4_a;
    public GameObject stage2_4_b;
    public GameObject stage2_4_c;
    public GameObject debugSpawnGO;
    public GameObject go_1;
    public GameObject go_2;
    public GameObject go_3;
    public GameObject go_4;
    // Use this for initialization
    void Start () {
        Invoke("destroyPlanes", 5);
        Invoke("spawnRndFruit", 1);
        timeSinceLastScroll = 0;
        progress++;        
    }

    float scrollDelay = 0.3f;
    float timeSinceLastScroll;

    // Update is called once per frame
    void Update()
    {
        switch (progress)
        {
            case 1:
                if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Cancel"))
                    destroyPlanes();
                break;

            case 2:
                if (Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") > 0)
                    previousMenuItem();
                if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") < 0)
                    nextMenuItem();
                if (Input.GetButtonDown("Submit"))
                    selectSelectedMenuItem();
                if (Input.GetButtonDown("Cancel"))
                    if (selectedMenuItem == stage2_4_c)
                        Application.Quit();
                    else
                    {
                        selectedMenuItem = stage2_4_c;
                        showSelectedMenuItem();
                    }
                break;
        }
    }

    GameObject selectedMenuItem;
    List<GameObject> menuItems;
    void destroyPlanes()
    {
        if(progress == 1)
        {
            //plane1.SetActive(false);
            //plane2.SetActive(false);
            stage2.SetActive(true);
            Invoke("showStage2_1", 0.25f);
            Invoke("showStage2_2", 0.5f);
            Invoke("showStage2_3", 0.75f);
            Invoke("showStage2_4", 1f);
            selectedMenuItem = stage2_1;
            menuItems = new List<GameObject>();
            menuItems.Add(stage2_1);
            menuItems.Add(stage2_2);
            menuItems.Add(stage2_3);
            menuItems.Add(stage2_4_a);
            menuItems.Add(stage2_4_b);
            menuItems.Add(stage2_4_c);

            Invoke("spawnRndFruitPeriodically", .5f);
            progress++;
        }
    }

    void showStage2_1()
    {
        stage2_1.SetActive(true);
    }
    void showStage2_2()
    {
        stage2_2.SetActive(true);
    }
    void showStage2_3()
    {
        stage2_3.SetActive(true);
    }
    void showStage2_4()
    {
        stage2_4.SetActive(true);
    }
    
    void nextMenuItem()
    {
        if (Time.time - timeSinceLastScroll < scrollDelay)
            return;
        timeSinceLastScroll = Time.time;

        int index = menuItems.IndexOf(selectedMenuItem);
        index++;
        index = index % menuItems.Count;
        selectedMenuItem = menuItems[index];
        showSelectedMenuItem();
    }

    void previousMenuItem()
    {
        if (Time.time - timeSinceLastScroll < scrollDelay)
            return;
        timeSinceLastScroll = Time.time;

        int index = menuItems.IndexOf(selectedMenuItem);
        index--;
        if (index < 0)
            index = menuItems.Count - 1;
        selectedMenuItem = menuItems[index];
        showSelectedMenuItem();
    }

    public Material inactiveMaterial;
    public Material activeMaterial;
    public Material activeMaterial_Fruit;
    void showSelectedMenuItem()
    {
        foreach (GameObject menuItem in menuItems)
            foreach (MeshRenderer mesh in menuItem.GetComponentsInChildren<MeshRenderer>())
                mesh.material = inactiveMaterial;

        foreach (MeshRenderer mesh in selectedMenuItem.GetComponentsInChildren<MeshRenderer>())
            if(mesh.GetComponent<MeshFilter>().mesh.name.StartsWith("SI_"))
                mesh.material = activeMaterial;
            else
                mesh.material = activeMaterial_Fruit;
    }

    void selectSelectedMenuItem()
    {
        switch(selectedMenuItem.name)
        {
            case "STORY":
                SceneManager.LoadScene("TASK_SCENE");
                break;
            case "SANDBOX":
                SceneManager.LoadScene("TASK_SCENE");
                break;
            case "DAILY_CHALLENGE":
                SceneManager.LoadScene("TASK_SCENE");
                break;

            //Rating Evaluation:
            case "START":
                PlayerPrefs.SetInt("progress", -1);
                SceneManager.LoadScene("TASK_SCENE");
                break;
            case "BEWERTEN1":
                SceneManager.LoadScene("RatingEvaluation_Rating1Scene");
                break;
            case "BEWERTEN2":
                SceneManager.LoadScene("RatingEvaluation_Rating2Scene");
                break;
            case "QUIT":
                Application.Quit();
                break;
        }
    }

    public GameObject[] fruitsToSpawn;
    void spawnRndFruit()
    {
        plane1.SetActive(true);
        GameObject go = Instantiate(fruitsToSpawn[Random.Range(0, fruitsToSpawn.Length - 1)], spawnGO.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        if (!go.GetComponent<Rigidbody>())
            go.AddComponent<Rigidbody>();
        go.GetComponent<Rigidbody>().useGravity = true;
        if (!go.GetComponent<Collider>())
            go.AddComponent<MeshCollider>();
        if (go.GetComponentInChildren<isCuttingObject>())
            Destroy(go.GetComponentInChildren<isCuttingObject>());
    }

    void spawnRndFruitPeriodically()
    {
        spawnRndFruit();
        Invoke("spawnRndFruitPeriodically", 1f);
    }

}
