using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NEEMConverter : MonoBehaviour
{
    private void Start()
    {
        List<NEEMEntry> neemList = new List<NEEMEntry>();
        string episodeID = createEpisode(neemList);

        //test
        //addPickPlaceAction(episodeID, neemList);

        // test2
        convertKCjsonToNEEM("C:/Users/Jo/Dropbox/PhD/5_EasePipeline/StudyResults/JSONs/2019-06-14-16-12-57-M2S14.mp4.json", episodeID, neemList);

        printNeem(episodeID, neemList);
    }

    void addPickPlaceAction(string episodeID, List<NEEMEntry> neemList, long timeStart, long timeEnd, string objType)
    {
        string actionID = createAction(episodeID, timeStart, timeEnd, neemList);        // in what time format is time?
        createTransportingTask(actionID, neemList);
        createObject(objType, neemList);

        // dont we need positions/relations to other objects/relation to the current task?
    }

    void printNeem(string id, List<NEEMEntry> neemList)
    {
        string s = "";
        foreach (NEEMEntry ne in neemList)
        {
            string str = JsonUtility.ToJson(ne, false).Replace("\\\"", "\"");
            if (str.Contains("hasInterval"))
                str = str.Replace("\"{\"$numberDecimal", "{\"$numberDecimal").Replace("\",\"graph", ",\"graph");
            str = str.Replace("\"{\"time", "{\"time").Replace("}}}\"}", "}}}}");
            s += str + "\r\n"; ;
        }
        File.WriteAllText("NEEM_" + id + ".json", s);
        Debug.Log("file written as: " + "NEEM_" + id + ".json");
    }

    void convertKCjsonToNEEM(string file, string episodeID, List<NEEMEntry> neemList)
    {
        // parse events
        VideoDescriptionData vdd = JsonUtility.FromJson<VideoDescriptionData>(File.ReadAllText(file));
        DateTime startTime = DateTime.MinValue;
        string targetObj = "";

        foreach(string eventStr in vdd.eventsTracked)
        {
            if (eventStr.Contains("PICKUP"))
            {
                startTime = DateTime.Parse(eventStr.Split('(')[1].Split(')')[0]);
                targetObj = eventStr.Split('|')[1].Split('(')[0].Replace(" ", "");
                continue;
            }
            if (eventStr.Contains("PUTDOWN") && targetObj == eventStr.Split('|')[1].Split('(')[0].Replace(" ", ""))
            {
                DateTime endTime = DateTime.Parse(eventStr.Split('(')[1].Split(')')[0]);
                addPickPlaceAction(episodeID, neemList, startTime.ToFileTime(), endTime.ToFileTime(), targetObj);

                targetObj = "";
                continue;
            }

        }
    }

    // ----- ----- ----- //

    string getRndIdentifier()
    {
        string id = "";
        for (int i = 0; i < 6; i++)
            id += (char)UnityEngine.Random.Range('A', 'Z');
        return id;
    }

    string createEpisode(List<NEEMEntry> neemList)
    {
        string id = getRndIdentifier();
        neemList.Add(new NEEMEntry(id, "http://www.ease-crc.org/ont/EASE.owl#Episode_" + id, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/EASE.owl#Episode"));
        neemList.Add(new NEEMEntry(id, "http://www.ease-crc.org/ont/EASE.owl#Episode_" + id, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        return id;
    }

    string createAction(string episodeID, long timeStart, long timeEnd, List<NEEMEntry> neemList)
    {
        string actionID = getRndIdentifier();
        neemList.Add(new NEEMEntry(episodeID, "http://www.ease-crc.org/ont/EASE.owl#Episode_" + episodeID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#includesAction", "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID));
        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action"));
        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        // time
        string timeIntervalID = createTimeInterval(timeStart, timeEnd, neemList);
        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#hasTimeInterval", "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#TimeInterval_" + timeIntervalID));

        // phys task
        string physicalTaskID = createPhysicalTask(neemList);
        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#executesTask", "http://www.ease-crc.org/ont/EASE-ACT.owl#PhysicalTask_" + timeIntervalID));

        // constituent?

        return actionID;
    }

    string createTimeInterval(long timeStart, long timeEnd, List<NEEMEntry> neemList)
    {
        string timeIntervalID = getRndIdentifier();
        neemList.Add(new NEEMEntry(timeIntervalID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#TimeInterval_" + timeIntervalID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#TimeInterval"));
        neemList.Add(new NEEMEntry(timeIntervalID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#TimeInterval_" + timeIntervalID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));
        neemList.Add(new NEEMEntry(timeIntervalID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#TimeInterval_" + timeIntervalID, "http://www.ease-crc.org/ont/EASE.owl#hasIntervalBegin", "{\"$numberDecimal\": \"" + timeStart + "\"}"));
        neemList.Add(new NEEMEntry(timeIntervalID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#TimeInterval_" + timeIntervalID, "http://www.ease-crc.org/ont/EASE.owl#hasIntervalEnd", "{\"$numberDecimal\": \"" + timeEnd + "\"}"));

        return timeIntervalID;
    }

    string createPhysicalTask(List<NEEMEntry> neemList)
    {
        string physicalTaskID = getRndIdentifier();
        neemList.Add(new NEEMEntry(physicalTaskID, "http://www.ease-crc.org/ont/EASE-ACT.owl#PhysicalTask_" + physicalTaskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/EASE-ACT.owl#PhysicalTask"));
        neemList.Add(new NEEMEntry(physicalTaskID, "http://www.ease-crc.org/ont/EASE-ACT.owl#PhysicalTask_" + physicalTaskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        return physicalTaskID;
    }

    string createTransportingTask(string actionID, List<NEEMEntry> neemList)
    {
        string transportingTaskID = getRndIdentifier();

        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#executesTask", "http://www.ease-crc.org/ont/EASE-ACT.owl#Transporting_" + transportingTaskID));

        neemList.Add(new NEEMEntry(transportingTaskID, "http://www.ease-crc.org/ont/EASE-ACT.owl#Transporting_" + transportingTaskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/EASE-ACT.owl#Transporting"));
        neemList.Add(new NEEMEntry(transportingTaskID, "http://www.ease-crc.org/ont/EASE-ACT.owl#Transporting_" + transportingTaskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));
        return transportingTaskID;
    }

    string createObject(string objType, List<NEEMEntry> neemList)
    {
        string objectID = getRndIdentifier();
        neemList.Add(new NEEMEntry(objectID, "http://www.ease-crc.org/ont/EASE-OBJ.owl#" + objType + "_" + objectID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/EASE-OBJ.owl#" + objType));
        neemList.Add(new NEEMEntry(objectID, "http://www.ease-crc.org/ont/EASE-OBJ.owl#" + objType + "_" + objectID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        // do EASE-Objs need a lifetime?

        // do EASE-Objs need an "affected_object"?
        string affectedObjectID = createAffectedObject(objectID, objType, neemList);

        return objectID;
    }

    string createAffectedObject(string objectID, string objType, List<NEEMEntry> neemList)
    {
        string affectedObjectID = getRndIdentifier();
        neemList.Add(new NEEMEntry(affectedObjectID, "http://www.ease-crc.org/ont/EASE-OBJ.owl#AffectedObject_" + affectedObjectID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#classifies", "http://www.ease-crc.org/ont/EASE-OBJ.owl#" + objType + "_" + objectID));

        neemList.Add(new NEEMEntry(affectedObjectID, "http://www.ease-crc.org/ont/EASE-OBJ.owl#AffectedObject_" + affectedObjectID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/EASE-OBJ.owl#AffectedObject"));
        neemList.Add(new NEEMEntry(affectedObjectID, "http://www.ease-crc.org/ont/EASE-OBJ.owl#AffectedObject_" + affectedObjectID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        return affectedObjectID;
    }
}
