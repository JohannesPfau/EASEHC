using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NEEMEntry
{
    [NonSerialized]
    public string id;

    public string s; // ontology state, e.g. http://www.ease-crc.org/ont/SOMA.owl#Episode_ABCDEFGH
    public string o; // ontology target, e.g. http://www.w3.org/1999/02/22-rdf-syntax-ns#type
    public string p; // ontology relation. e.g. http://www.ease-crc.org/ont/SOMA.owl#Episode

    public string graph = "user"; // tentative
    public string scope = "{\"time\": {\"since\":{\"$numberDecimal\": \"0\"},\"until\":{\"$numberDecimal\": \"Infinity\"}}}"; // tentative

    public NEEMEntry(string id, string s, string o, string p)
    {
        this.s = s;
        this.o = o;
        this.p = p;
    }
}
