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
                str = str.Replace("\"{\"$numberDecimal", "{\"$numberDecimal").Replace("\",\"graph", ",\"graph").Replace("}\"," , "},").Replace("hasIntervalBegin,", "hasIntervalBegin\",")
                    .Replace("hasIntervalEnd,", "hasIntervalEnd\",");
            str = str.Replace("\"{\"time", "{\"time").Replace("}}}\"}", "}}}}").Replace(",\"since\":\"\"", "").Replace(",\"until\":\"\"", "");
            s += str + "\r\n"; ;
        }

        NEEMMeta meta = new NEEMMeta();
        meta.id = id;
        meta.created_by = "jpfau";
        meta.created_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "+00:00";
        meta.model_version = "0.1";
        meta.description = "Episodic memories of a human operator performing everyday kitchen activities in a simulated virtual reality environment.";
        meta.keywords = new string[] { "KitchenClash" };
        meta.name = SceneNameToTaskDescription(vdd.sceneName);
        meta.activity = new NEEMMeta.NEEMActivity(SceneNameToTaskDescription(vdd.sceneName));

        string filePath = "NEEMS/"+ SceneNameToTaskName(vdd.sceneName) + "/" + id+"/triples/roslog/";
        new FileInfo(filePath).Directory.Create();
        File.WriteAllText(filePath + "triples.metadata.json", JsonUtility.ToJson(meta,true));
        File.WriteAllText(filePath + "triples.json", s);

        string tfPath = "NEEMS/" + SceneNameToTaskName(vdd.sceneName) + "/" + id + "/ros_tf/roslog/";
        new FileInfo(tfPath).Directory.Create();
        s = "";
        foreach(NEEMTransform tf in tfList)
            s += JsonUtility.ToJson(tf, false).Replace("\"{'$date", "{'$date").Replace("+02:00'}\"", "+02:00'}") + "\r\n";
        File.WriteAllText(tfPath + "tf.json", s);


        Debug.Log("NEEM " + id + " written");
    }

    VideoDescriptionData convertKCjsonToNEEM(string file, string episodeID, List<NEEMEntry> neemList, List<NEEMTransform> tfList)
    {
        // parse events
        VideoDescriptionData vdd = JsonUtility.FromJson<VideoDescriptionData>(File.ReadAllText(file));
        //DateTime startTime = DateTime.MinValue;
        //string targetObj = "";
        DateTime initialTime = DateTime.MaxValue;

        string agentID = createAgent(neemList);

        foreach(string eventStr in vdd.eventsTracked)
        {
            if (eventStr.Contains("PICKUP"))
            {
                DateTime time = DateTime.Parse(eventStr.Split('(')[1].Split(')')[0]);
                if (time < initialTime)
                    initialTime = time;
                DateTime endTime = time + TimeSpan.FromSeconds(1);
                string targetObj = eventStr.Split('|')[1].Split('(')[0].Replace(" ", "");

                string actionID = createAction(episodeID, toUnixTime(time), toUnixTime(endTime), neemList, agentID);
                string objectID = createObject(getSOMAstringFor(targetObj), neemList);

                string affectedObjectID = createAffectedObject(objectID, getSOMAstringFor(targetObj), neemList, (time - initialTime).TotalSeconds.ToString(), (endTime - initialTime).TotalSeconds.ToString());
                createPickingUpTask(actionID, neemList, affectedObjectID);
                continue;
            }
            if (eventStr.Contains("PUTDOWN"))// && targetObj == eventStr.Split('|')[1].Split('(')[0].Replace(" ", ""))
            {
                DateTime time = DateTime.Parse(eventStr.Split('(')[1].Split(')')[0]);
                if (time < initialTime)
                    initialTime = time;
                DateTime endTime = time + TimeSpan.FromSeconds(1);
                string targetObj = eventStr.Split('|')[1].Split('(')[0].Replace(" ", "");
                string actionID = createAction(episodeID, toUnixTime(time), toUnixTime(endTime), neemList, agentID);
                string objectID = createObject(getSOMAstringFor(targetObj), neemList);
                string affectedObjectID = createAffectedObject(objectID, getSOMAstringFor(targetObj), neemList, (time - initialTime).TotalSeconds.ToString(), (endTime - initialTime).TotalSeconds.ToString());
                createPuttingDownTask(actionID, neemList, affectedObjectID);
                continue;
            }
            if(eventStr.Contains("MOVEMENT"))
            {
                DateTime time = DateTime.Parse(eventStr.Split('(')[1].Split(')')[0]);
                if (time < initialTime)
                    initialTime = time;
                //tfList.Add(convertStringToNEEMTransform(eventStr.Split('|')[1], toIsoTime(time))); // remove "Player" GameObject from NEEMs
                tfList.Add(convertStringToNEEMTransform(eventStr.Split('|')[2], toIsoTime(time)));
                tfList.Add(convertStringToNEEMTransform(eventStr.Split('|')[3], toIsoTime(time)));
                tfList.Add(convertStringToNEEMTransform(eventStr.Split('|')[4], toIsoTime(time)));

                continue;
            }
            if(eventStr.Contains("COLLISION_WHILE_HELD") && collisionEventToTask(eventStr) == "CUTTING")
            {
                DateTime time = DateTime.Parse(eventStr.Split('(')[1].Split(')')[0]);
                if (time < initialTime)
                    initialTime = time;
                DateTime endTime = time + TimeSpan.FromSeconds(1);
                string toolObj = eventStr.Split('|')[1].Split('(')[0].Replace(" ", "");
                string patientObj = eventStr.Split('|')[2].Split('(')[0].Replace(" ", "");


                string actionID = createAction(episodeID, toUnixTime(time), toUnixTime(endTime), neemList, agentID);
                string objectID = createObject(getSOMAstringFor(patientObj), neemList);
                string toolObjectID = createObject(getSOMAstringFor(toolObj), neemList);
                createTool(toolObjectID, neemList);

                string affectedObjectID = createAffectedObject(objectID, getSOMAstringFor(patientObj), neemList, (time - initialTime).TotalSeconds.ToString(), (endTime - initialTime).TotalSeconds.ToString());
                
                createCuttingTask(actionID, neemList, affectedObjectID, toolObjectID);
                continue;
            }
        }
        return vdd;
    }

    // ----- ----- ----- //

    NEEMTransform convertStringToNEEMTransform(string s, string time)
    {
        // eg: Player (pos):(-1.6, -0.9, 6.6) (rot):(0.0, 270.0, 0.0)
        string objName = s.Split('(')[0].Replace(" ", "").Replace("VRCamera","Head").Replace("Hand1","LHand").Replace("Hand2","RHand");
        float px = float.Parse(s.Split('(')[2].Split(',')[0]) * 0.3f;
        float pz = float.Parse(s.Split('(')[2].Split(',')[1]) * 0.3f;       // change y and z for rViz is right-handed
        float py = float.Parse(s.Split('(')[2].Split(',')[2].Replace(")","").Replace(" ","")) * 0.3f; // change y and z for rViz is right-handed

        float rx = float.Parse(s.Split('(')[4].Split(',')[0]);
        float rz = float.Parse(s.Split('(')[4].Split(',')[1]); // change y and z for rViz is right-handed
        float ry = float.Parse(s.Split('(')[4].Split(',')[2].Replace(")", "").Replace(" ", ""));  // change y and z for rViz is right-handed       
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

        neemList.Add(new NEEMEntry(id, "http://www.ease-crc.org/ont/SOMA.owl#Episode_" + id, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#isSettingFor", "http://knowrob.org/kb/Kitchen-clash.owl#iai_kitchen_vr_kitchen_root"));
   
        return id;
    }

    string createAgent(List<NEEMEntry> neemList)
    {
        string id = getRndIdentifier();
        neemList.Add(new NEEMEntry(id, "http://knowrob.org/kb/Kitchen-clash-agent.owl#agent_1_" + id, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://knowrob.org/kb/Kitchen-clash-agent.owl#agent_1"));
        neemList.Add(new NEEMEntry(id, "http://knowrob.org/kb/Kitchen-clash-agent.owl#agent_1_" + id, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        return id;
    }

    string createAction(string episodeID, string timeStart, string timeEnd, List<NEEMEntry> neemList, string agentID)
    {
        string actionID = getRndIdentifier();
        neemList.Add(new NEEMEntry(episodeID, "http://www.ease-crc.org/ont/SOMA.owl#Episode_" + episodeID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#includesAction", "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID));
        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action"));
        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ease-crc.org/ont/SOMA.owl#isPerformedBy", "http://knowrob.org/kb/Kitchen-clash-agent.owl#agent_1_"+agentID));

        // time
        string timeIntervalID = createTimeInterval(timeStart, timeEnd, neemList);
        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#hasTimeInterval", "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#TimeInterval_" + timeIntervalID));

        // phys task
        string physicalTaskID = createPhysicalTask(neemList);
        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#executesTask", "http://www.ease-crc.org/ont/SOMA.owl#PhysicalTask_" + physicalTaskID));
        
        return actionID;
    }

    string createTimeInterval(string timeStart, string timeEnd, List<NEEMEntry> neemList)
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

    //    neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#executesTask", "http://www.ease-crc.org/ont/soma.owl#Transporting_" + transportingTaskID));

    //    neemList.Add(new NEEMEntry(transportingTaskID, "http://www.ease-crc.org/ont/soma.owl#Transporting_" + transportingTaskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/soma.owl#Transporting"));
    //    neemList.Add(new NEEMEntry(transportingTaskID, "http://www.ease-crc.org/ont/soma.owl#Transporting_" + transportingTaskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));
    //    return transportingTaskID;
    //}

    string createPickingUpTask(string actionID, List<NEEMEntry> neemList, string affectedObjectID)
    {
        string taskID = getRndIdentifier();

        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#executesTask", "http://www.ease-crc.org/ont/SOMA.owl#PickingUp_" + taskID));

        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#PickingUp_" + taskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/SOMA.owl#PickingUp"));
        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#PickingUp_" + taskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#PickingUp_" + taskID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#hasParticipant", "http://www.ease-crc.org/ont/SOMA.owl#Patient_" + affectedObjectID));

        return taskID;
    }

    string createPuttingDownTask(string actionID, List<NEEMEntry> neemList, string affectedObjectID)
    {
        string taskID = getRndIdentifier();

        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#executesTask", "http://www.ease-crc.org/ont/SOMA.owl#PuttingDown_" + taskID));

        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#PuttingDown_" + taskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/SOMA.owl#PuttingDown"));
        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#PuttingDown_" + taskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#PickingUp_" + taskID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#hasParticipant", "http://www.ease-crc.org/ont/SOMA.owl#Patient_" + affectedObjectID));
        return taskID;
    }

    string createCuttingTask(string actionID, List<NEEMEntry> neemList, string affectedObjectID, string toolID)
    {
        string taskID = getRndIdentifier();

        neemList.Add(new NEEMEntry(actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#Action_" + actionID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#executesTask", "http://www.ease-crc.org/ont/SOMA.owl#Cutting_" + taskID));

        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#Cutting_" + taskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/SOMA.owl#Cutting"));
        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#Cutting_" + taskID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#Cutting_" + taskID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#hasParticipant", "http://www.ease-crc.org/ont/SOMA.owl#Patient_" + affectedObjectID));
        neemList.Add(new NEEMEntry(taskID, "http://www.ease-crc.org/ont/SOMA.owl#Cutting_" + taskID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#hasParticipant", "http://www.ease-crc.org/ont/SOMA.owl#Tool_" + toolID));
        return taskID;
    }

    string createObject(string objType, List<NEEMEntry> neemList)
    {
        string objectID = getRndIdentifier();
        neemList.Add(new NEEMEntry(objectID, objType + "_" + objectID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", objType));
        neemList.Add(new NEEMEntry(objectID, objType + "_" + objectID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));
        return objectID;
    }

    string createAffectedObject(string objectID, string objType, List<NEEMEntry> neemList, string startTime, string endTime)
    {
        string affectedObjectID = getRndIdentifier();
        NEEMEntry hasRoleEntry = new NEEMEntry(affectedObjectID, objType + "_" + objectID, "http://www.ontologydesignpatterns.org/ont/dul/DUL.owl#hasRole", "http://www.ease-crc.org/ont/SOMA.owl#Patient_" + affectedObjectID);
        hasRoleEntry.setTimeScope(startTime, endTime);
        neemList.Add(hasRoleEntry);

        neemList.Add(new NEEMEntry(affectedObjectID, "http://www.ease-crc.org/ont/SOMA.owl#Patient_" + affectedObjectID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/SOMA.owl#Patient"));
        neemList.Add(new NEEMEntry(affectedObjectID, "http://www.ease-crc.org/ont/SOMA.owl#Patient_" + affectedObjectID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));

        return affectedObjectID;
    }

    void createTool(string toolID, List<NEEMEntry> neemList)
    {
        neemList.Add(new NEEMEntry(toolID, "http://www.ease-crc.org/ont/SOMA.owl#Tool_" + toolID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.ease-crc.org/ont/SOMA.owl#Tool"));
        neemList.Add(new NEEMEntry(toolID, "http://www.ease-crc.org/ont/SOMA.owl#Tool_" + toolID, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "http://www.w3.org/2002/07/owl#NamedIndividual"));
    }

    string SceneNameToTaskDescription(string sceneName)
    {
        switch (sceneName)
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
    string SceneNameToTaskName(string sceneName)
    {
        switch (sceneName)
        {
            case "RatingEvaluation_Task0":
                return "Tablesetting";
            case "RatingEvaluation_Task1":
                return "CucumberSalad";
            case "RatingEvaluation_Task2":
                return "Steak.";
        }
        return "";
    }

    string toIsoTime(DateTime dt)
    {
        return dt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
    }

    string toUnixTime(DateTime dt)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var unixDateTime = (dt.ToUniversalTime() - epoch).TotalSeconds;
        return unixDateTime+"";
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

    string getSOMAstringFor(string s)
    {
        switch (s)
        {
            case "Teller":
            case "Kleiner Teller":
            case "Grosser Teller":
                return "http://www.ease-crc.org/ont/SOMA.owl#Plate";
            case "Glas":
                return "http://www.ease-crc.org/ont/SOMA.owl#Glass";
            case "Gabel":
                return "http://www.ease-crc.org/ont/SOMA.owl#Fork";
            case "Essloeffel":
                return "http://www.ease-crc.org/ont/SOMA.owl#Spoon";
            case "Messer":
                return "http://www.ease-crc.org/ont/SOMA.owl#Knife";
            case "Hackmesser":
                return "http://www.ease-crc.org/ont/SOMA.owl#ButcherKnife";
            case "Gurke":
                return "http://www.ease-crc.org/ont/SOMA.owl#Cucumber";
            case "Kuechenmesser":
                return "http://www.ease-crc.org/ont/SOMA.owl#KitchenKnife";
            case "Öl":
            case "Oel":
                return "http://www.ease-crc.org/ont/SOMA.owl#OliveOil";
            case "Pfanne":
                return "http://www.ease-crc.org/ont/SOMA.owl#Pan";
            case "Steak":
            case "Steak (medium)":
            case "Steak (well done)":
                return "http://www.ease-crc.org/ont/SOMA.owl#Steak";
            case "Pflanze":
                return "http://www.ease-crc.org/ont/SOMA.owl#Plant";
            case "Schuessel":
            case "Kleine Schuessel":
            case "Grosse Schuessel":
                return "http://www.ease-crc.org/ont/SOMA.owl#Bowl";
            case "Tomate":
                return "http://www.ease-crc.org/ont/SOMA.owl#Tomato";
            case "Kartoffel":
                return "http://www.ease-crc.org/ont/SOMA.owl#Potato";
            case "Stuhl":
                return "http://www.ease-crc.org/ont/SOMA.owl#Chair";
            case "Aubergine":
                return "http://www.ease-crc.org/ont/SOMA.owl#Eggplant";
            case "Banane":
                return "http://www.ease-crc.org/ont/SOMA.owl#Banana";
            case "Karotte":
                return "http://www.ease-crc.org/ont/SOMA.owl#Carrot";
            case "Tablett":
                return "http://www.ease-crc.org/ont/SOMA.owl#Tablet";
            case "Schwamm":
                return "http://www.ease-crc.org/ont/SOMA.owl#Sponge";
            case "Apfel":
                return "http://www.ease-crc.org/ont/SOMA.owl#Apple";
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
