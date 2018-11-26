using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PipelineInput : MonoBehaviour {

    public string jsonFileName = "outputSebastian_20181115.json";

    // Use this for initialization
    void Start () {
        string jString = File.ReadAllText("Assets/PipelineSim/"+ jsonFileName);
        PipelineInputData jData = JsonConvert.DeserializeObject<PipelineInputData>(jString);

        Debug.Log(jData.version);
        Debug.Log(jData.application);
        Debug.Log(jData.uuid);
        Debug.Log(jData.sentence);
        Debug.Log("--- PARSES ---");
        foreach (string s in jData.parses.Keys)
            Debug.Log(s + ": " + jData.parses[s]);
        Debug.Log(jData.http_status);
        Debug.Log("--- GRAPHS ---");
        foreach (string s in jData.graphs.Keys)
            Debug.Log(s + ": " + jData.graphs[s]);
        Debug.Log("--- JSON PARSES ---");
        Debug.Log(jData.json_parses.smain.__class__);
        Debug.Log(jData.json_parses.smain.name);

        foreach (jRole r in jData.json_parses.smain.roles)
            debugRole(r);
    }

    void debugRole(jRole r)
    {
        if (r.__class__ != null)
            Debug.Log(r.__class__ + ": " + r.type);
        if (r.target != null)
            if(r.target.type != null)
            {
                Debug.Log("target: " + r.target.type);
                debugRole(r.target);
            }
            else
                Debug.Log("alt target: " + r.target.alternativeTarget);

        //if (r.alternativeTarget != null && r.alternativeTarget != "")
    }

}
