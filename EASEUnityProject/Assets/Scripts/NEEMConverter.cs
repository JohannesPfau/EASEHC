using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NEEMConverter : MonoBehaviour
{
    private void Start()
    {
        foreach(string file in Directory.GetFiles("C:/Users/Jo/Dropbox/PhD/5_EasePipeline/StudyResults/JSONs/"))
        {
            if(file.Contains(".mp4.json"))
            {
                List<NEEMEntry> neemList = new List<NEEMEntry>();
                List<NEEMTransform> tfList = new List<NEEMTransform>();
                string episodeID = createEpisode(neemList);

                VideoDescriptionData vdd = convertKCjsonToNEEM(file, episodeID, neemList, tfList);

                printNeem(episodeID, neemList, vdd, tfList);
            }
        }
    }

    //void addPickPlaceAction(string episodeID, List<NEEMEntry> neemList, double timeStart, double timeEnd, string objType)
    //{
    //    string actionID = createAction(episodeID, timeStart, timeEnd, neemList);
    //    //createTransportingTask(actionID, neemList);
    //    createObject(objType, neemList);
    //}

    void printNeem(string id, List<NEEMEntry> neemList, VideoDescriptionData vdd, List<NEEMTransform> tfList)
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

        NEEMMeta meta = new NEEMMeta();
        meta.id = id;
        meta.created_by = "jpfau";
        meta.created_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "+00:00";
        meta.model_version = "0.1";
        meta.description = SceneNameToTaskDescription(vdd.sceneName);
        meta.keywords = new string[] { "KitchenClash" };

        string filePath = "NEEMS/"+id+"/triples/roslog/";
        new FileInfo(filePath).Directory.Create();
        File.WriteAllText(filePath + "triples.metadata.json", JsonUtility.ToJson(meta,true));
        File.WriteAllText(filePath + "triples.json", s);

        string tfPath = "NEEMS/" + id + "/ros_tf/roslog/";
        new FileInfo(tfPath).Directory.Create();
        s = "";
        foreach(NEEMTransform tf in tfList)
            s += JsonUtility.ToJson(tf, false) + "\r\n";
        File.WriteAllText(tfPath + "tf.json", s);



        Debug.Log("NEEM " + id + " written");
    }

    VideoDescriptionData convertKCjsonToNEEM(string file, string episodeID, List<NEEMEntry> neemList, List<NEEMTransform> tfList)
    {
        // parse events
        VideoDescriptionData vdd = JsonUtility.FromJson<VideoDescriptionData>(File.ReadAllText(file));
        //DateTime startTime = DateTime.MinValue;
        //string targetObj = "";

        foreach(string eventStr in vdd.eventsTracked)
        {
            if (eventStr.Contains("PICKUP"))
            {
                DateTime time = DateTime.Parse(eventStr.Split('(')[1].Split(')')[0]);
                string targetObj = eventStr.Split('|')[1].Split('(')[0].Replace(" ", "");

                string actionID = createAction(episodeID, toUnixTime(time), toUnixTime(time), neemList);
                string objectID = createObject(convertObjDescription(targetObj), neemList);
                string affectedObjectID = createAffectedObject(objectID, convertObjDescription(targetObj), neemList);
                createPickingUpTask(actionID, neemList, affectedObjectID);
                continue;
            }
            if (eventStr.Contains("PUTDOWN"))// && targetObj == eventStr.Split('|')[1].Split('(')[0].Replace(" ", ""))
            {
                DateTime time = DateTime.Parse(eventStr.Split('(')[1].Split(')')[0]);
                string targetObj = eventStr.Split('|')[1].Split('(')[0].Replace(" ", "");
                string actionID = createAction(episodeID, toUnixTime(time), toUnixTime(time), neemList);
                string objectID = createObject(convertObjDescription(targetObj), neemList);
                string affectedObjectID = createAffectedObject(objectID, convertObjDescription(targetObj), neemList);
                createPuttingDownTask(actionID, neemList, affectedObjectID);
                continue;
            }
            if(eventStr.Contains("MOVEMENT"))
            {
                DateTime time = DateTime.Parse(eventStr.Split('(')[1].Split(')')[0]);
                tfList.Add(convertStringToNEEMTransform(eventStr.Split('|')[1], toUnixTime(time)));
                tfList.Add(convertStringToNEEMTransform(eventStr.Split('|')[2], toUnixTime(time)));
                tfList.Add(convertStringToNEEMTransform(eventStr.Split('|')[3], toUnixTime(time)));
                tfList.Add(convertStringToNEEMTransform(eventStr.Split('|')[4], toUnixTime(time)));

                continue;
            }
            if(eventStr.Contains("COLLISION_WHILE_HELD") && collisionEventToTask(eventStr) == "CUTTING")
            {
                DateTime time = DateTime.Parse(eventStr.Split('(')[1].Split(')')[0]);
                string targetObj = eventStr.Split('|')[1].Split('(')[0].Replace(" ", "");
                string actionID = createAction(episodeID, toUnixTime(time), toUnixTime(time), neemList);
                string objectID = createObject(convertObjDescription(targetObj), neemList);
                string affectedObjectID = createAffectedObject(objectID, convertObjDescription(targetObj), neemList);

                createCuttingTask(actionID, neemList, affectedObjectID);
                continue;
            }
        }
        return vdd;
    }

    // ----- ----- ----- //

    NEEMTransform convertStringToNEEMTransform(string s, double time)
    {
        // eg: Player (pos):(-1.6, -0.9, 6.6) (rot):(0.0, 270.0, 0.0)
        string objName = s.Split('(')[0].Replace(" ", "").Replace("VRCamera","Head").Replace("Hand1","LHand").Replace("Hand2","RHand");
        float px = float.Parse(s.Split('(')[2].Split(',')[0]);
        float py = float.Parse(s.Split('(')[2].Split(',')[1]);
        float pz = float.Parse(s.Split('(')[2].Split(',')[2].Replace(")","").Replace(" ",""));

        float rx = float.Parse(s.Split('(')[4].Split(',')[0]);
        float ry = float.Parse(s.Split('(')[4].Split(',')[1]);
        float rz = float.Parse(s.Split('(')[4].Split(',')[2].Replace(")", "").Replace(" ", ""));        
        return new NEEMTransform(objName, time, new Vector3(px, py, pz), Quaternion.Euler(rx, ry, rz));
    }

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
        neemList.Add(new NEEMEntry(id, "http://www.ease-crc.org/ont/SOMA.owl#Episode_" + id, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/SOMA.owl#Episode"));
        neemList.Add(new NEEMEntry(id, "http://www.ease-crc.org/ont/SOMA.owl#Episode_" + id, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        return id;
    }

    string createAction(string episodeID, double timeStart, double timeEnd, List<NEEMEntry> neemList)
    {
        string actionID = getRndIdentifier();
        neemList.Add(new NEEMEntry(episodeID, "http://www.ease-crc.org/ont/SOMA.owl#Episode_" + episodeID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#includesAction", "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID));
        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action"));
        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        // time
        string timeIntervalID = createTimeInterval(timeStart, timeEnd, neemList);
        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#hasTimeInterval", "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#TimeInterval_" + timeIntervalID));

        // phys task
        string physicalTaskID = createPhysicalTask(neemList);
        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#executesTask", "http://www.ease-crc.org/ont/SOMA.owl#PhysicalTask_" + physicalTaskID));
        
        return actionID;
    }

    string createTimeInterval(double timeStart, double timeEnd, List<NEEMEntry> neemList)
    {
        string timeIntervalID = getRndIdentifier();
        neemList.Add(new NEEMEntry(timeIntervalID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#TimeInterval_" + timeIntervalID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#TimeInterval"));
        neemList.Add(new NEEMEntry(timeIntervalID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#TimeInterval_" + timeIntervalID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));
        neemList.Add(new NEEMEntry(timeIntervalID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#TimeInterval_" + timeIntervalID, "http://www.ease-crc.org/ont/SOMA.owl#hasIntervalBegin", "{\"$numberDecimal\": \"" + timeStart + "\"}"));
        neemList.Add(new NEEMEntry(timeIntervalID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#TimeInterval_" + timeIntervalID, "http://www.ease-crc.org/ont/SOMA.owl#hasIntervalEnd", "{\"$numberDecimal\": \"" + timeEnd + "\"}"));

        return timeIntervalID;
    }

    string createPhysicalTask(List<NEEMEntry> neemList)
    {
        string physicalTaskID = getRndIdentifier();
        neemList.Add(new NEEMEntry(physicalTaskID, "http://www.ease-crc.org/ont/SOMA.owl#PhysicalTask_" + physicalTaskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/SOMA.owl#PhysicalTask"));
        neemList.Add(new NEEMEntry(physicalTaskID, "http://www.ease-crc.org/ont/SOMA.owl#PhysicalTask_" + physicalTaskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        return physicalTaskID;
    }

    //string createTransportingTask(string actionID, List<NEEMEntry> neemList) // TODO: Dont use this anymore (go for pick and place) (mit executesTask vorher)
    //{
    //    string transportingTaskID = getRndIdentifier();

    //    neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#executesTask", "http://www.ease-crc.org/ont/EASE-ACT.owl#Transporting_" + transportingTaskID));

    //    neemList.Add(new NEEMEntry(transportingTaskID, "http://www.ease-crc.org/ont/EASE-ACT.owl#Transporting_" + transportingTaskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/EASE-ACT.owl#Transporting"));
    //    neemList.Add(new NEEMEntry(transportingTaskID, "http://www.ease-crc.org/ont/EASE-ACT.owl#Transporting_" + transportingTaskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));
    //    return transportingTaskID;
    //}

    string createPickingUpTask(string actionID, List<NEEMEntry> neemList, string affectedObjectID)
    {
        string taskID = getRndIdentifier();

        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#executesTask", "http://www.ease-crc.org/ont/SOMA.owl#PickingUp_" + taskID));

        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#PickingUp_" + taskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/SOMA.owl#PickingUp"));
        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#PickingUp_" + taskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#PickingUp_" + taskID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#defines", "http://www.ease-crc.org/ont/EASE-OBJ.owl#AffectedObject_" + affectedObjectID));

        return taskID;
    }

    string createPuttingDownTask(string actionID, List<NEEMEntry> neemList, string affectedObjectID)
    {
        string taskID = getRndIdentifier();

        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#executesTask", "http://www.ease-crc.org/ont/SOMA.owl#PuttingDown_" + taskID));

        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#PuttingDown_" + taskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/SOMA.owl#PuttingDown"));
        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#PuttingDown_" + taskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#PickingUp_" + taskID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#defines", "http://www.ease-crc.org/ont/EASE-OBJ.owl#AffectedObject_" + affectedObjectID));
        return taskID;
    }

    string createCuttingTask(string actionID, List<NEEMEntry> neemList, string affectedObjectID)
    {
        string taskID = getRndIdentifier();

        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#executesTask", "http://www.ease-crc.org/ont/SOMA.owl#Cutting_" + taskID));

        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#Cutting_" + taskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/SOMA.owl#Cutting"));
        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#Cutting_" + taskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#Cutting_" + taskID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#defines", "http://www.ease-crc.org/ont/EASE-OBJ.owl#AffectedObject_" + affectedObjectID));
        return taskID;
    }

    string createObject(string objType, List<NEEMEntry> neemList)
    {
        string objectID = getRndIdentifier();
        neemList.Add(new NEEMEntry(objectID, "http://www.ease-crc.org/ont/EASE-OBJ.owl#" + objType + "_" + objectID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/EASE-OBJ.owl#" + objType));
        neemList.Add(new NEEMEntry(objectID, "http://www.ease-crc.org/ont/EASE-OBJ.owl#" + objType + "_" + objectID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));
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

    string SceneNameToTaskDescription(string sceneName)
    {
        switch(sceneName)
        {
            case "RatingEvaluation_Task0":
                return "Set the table for two persons.";
            case "RatingEvaluation_Task1":
                return "Prepare a basic cucumber salad.";
            case "RatingEvaluation_Task2":
                return "Prepare a steak.";
        }
        return "";
    }

    double toUnixTime(DateTime dt)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var unixDateTime = (dt.ToUniversalTime() - epoch).TotalSeconds;
        return unixDateTime;
    }

    string convertObjDescription(string s)
    {
        switch(s)
        {
            case "Teller":
            case "Kleiner Teller":
            case "Grosser Teller":
                return "PLATE";
            case "Glas":
                return "GLASS";
            case "Gabel":
                return "FORK";
            case "Essloeffel":
                return "SPOON";
            case "Messer":
                return "KNIFE";
            case "Hackmesser":
                return "BUTCHER_KNIFE";
            case "Gurke":
                return "CUCUMBER";
            case "Kuechenmesser":
                return "KITCHEN_KNIFE";
            case "Öl":
            case "Oel":
                return "OLIVE_OIL";
            case "Pfanne":
                return "PAN";
            case "Steak":
            case "Steak (medium)":
            case "Steak (well done)":
                return "STEAK";
            case "Pflanze":
                return "PLANT";
            case "Schuessel":
            case "Kleine Schuessel":
            case "Grosse Schuessel":
                return "BOWL";
            case "Tomate":
                return "TOMATO";
            case "Kartoffel":
                return "POTATO";
            case "Stuhl":
                return "CHAIR";
            case "Aubergine":
                return "EGGPLANT";
            case "Banane":
                return "BANANA";
            case "Karotte":
                return "CARROT";
            case "Tablett":
                return "TABLET";
            case "Schwamm":
                return "SPONGE";
            case "Apfel":
                return "APPLE";
        }
        return s;
    }

    string collisionEventToTask(string cEvent)
    {
        if (cEvent.Contains("messer"))
            return "CUTTING";
        return "";
    }
}
