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
        foreach(string s in jData.json_parses.Keys)
        {
            Debug.Log(s + ": ");
            foreach(jsonParse j in jData.json_parses[s])
            {
                Debug.Log(j.__class__);
                Debug.Log(j.type);
                Debug.Log(j.name);

                foreach (jRole r in j.roles)
                    debugRole(r);
            }
        }
    }

    void debugRole(jRole r)
    {
        if (r.__class__ != null)
            Debug.Log(r.__class__ + ": " + r.type);
        if (r.target != null)
            if(r.target.type != null)
            {
                Debug.Log("target: " + r.target.type);
                debugVariable(r.target);
            }
            else
                Debug.Log("alt target: " + r.target.alternativeTarget);
    }

    void debugVariable(jVariable v)
    {
        if (v.__class__ != null)
            Debug.Log(v.__class__ + ": " + v.name + " (" + v.type + ")");

        if (v.roles != null)
            foreach (jRole vr in v.roles)
                debugRole(vr);
    }

}
