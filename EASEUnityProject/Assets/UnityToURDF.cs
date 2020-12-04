using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class UnityToURDF : MonoBehaviour
{
    public bool convertAtStart;
    public string urdfName;
    public Transform rootTransform;
    List<string> uniqueNames = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        if (!convertAtStart)
            return;

        loadFolder("Assets/");
        string globalString = "<robot xmlns:xacro=\"http://ros.org/wiki/xacro\" name=\""+urdfName+"\">";

        foreach(MeshRenderer mr in rootTransform.GetComponentsInChildren<MeshRenderer>())
        {
            if (!mr.enabled || !mr.gameObject.activeSelf)
                continue;
            GameObject g = mr.gameObject;
            string uniqueName = g.name.Replace(" ", "_");
            string parentName = "";
            //if(g.transform.parent != null)
            //    parentName = g.transform.parent.gameObject.name.Replace(" ", "_");
            if (uniqueNames.Contains(uniqueName))
                uniqueName = uniqueName + "_" + Random.Range(0, 100000);
            uniqueNames.Add(uniqueName);

            string s = "<link name=\""+ uniqueName + "\" label=\"http://www.ease-crc.org/ont/SOMA.owl#PhysicalObject\">";
            s += "<sphere_inertia mass=\"0\" radius=\"0\"/>";
            s += "<visual>";
            s += "<origin rpy=\"" + g.transform.rotation.eulerAngles.x + " " + g.transform.rotation.eulerAngles.y + " " + g.transform.rotation.eulerAngles.z 
                + "\" xyz=\"" + g.transform.position.x + " " + g.transform.position.y + " " + g.transform.position.z + "\"/>";
            s += "<geometry>";

            string meshName = "UNKNOWN";
            foreach(GameObject asset in assetDatabase)
                if(asset.GetComponent<MeshFilter>() && (
                    asset.GetComponent<MeshFilter>().sharedMesh.bounds.Equals(mr.GetComponent<MeshFilter>().sharedMesh.bounds)))
                {
                    meshName = asset.name;
                    break;
                }
            if(meshName == "UNKNOWN")
            {
                foreach (GameObject asset in assetDatabase)
                    if (g.name.Contains(asset.name))
                    {
                        meshName = asset.name;
                        break;
                    }
            }
            if (meshName == "UNKNOWN")
            {
                foreach (GameObject asset in assetDatabase)
                    if (asset.GetComponent<MeshFilter>() &&
                        asset.GetComponent<MeshFilter>().sharedMesh.name.Equals(mr.GetComponent<MeshFilter>().sharedMesh.name))
                    {
                        meshName = asset.name;
                        break;
                    }
            }
            if (meshName == "UNKNOWN")
                continue;

            s += "<mesh filename=\"package://KitchenClashMeshes/"+ meshName +".fbx\"/>";           // TODO: .fbx?? TODO: "Sink_Drawer1" or just "sink"??!?!
            s += "</geometry>";
            s += "</visual>";
            s += "</link>";

            if (mr.GetComponent<LinearDrive>())
            {
                string j = "<joint name=\""+ uniqueName + "_joint\" type=\"prismatic\">";
                j += "<origin rpy=\"" + g.transform.rotation.eulerAngles.x + " " + g.transform.rotation.eulerAngles.y + " " + g.transform.rotation.eulerAngles.z
                    + "\" xyz=\"" + g.transform.position.x + " " + g.transform.position.y + " " + g.transform.position.z + "\"/>";
                j += "<child link=\""+ uniqueName + "\"/>";
                //if(parentName != "")
                    j += "<parent link=\"" + parentName + "\"/>";


                int axisX = 0, axisY = 0, axisZ = 0;
                float upperLimit = 0f;
                if (mr.GetComponent<LinearDrive>().endPosition.position.x > mr.GetComponent<LinearDrive>().startPosition.position.x)
                    axisX = 1;
                if (mr.GetComponent<LinearDrive>().endPosition.position.x < mr.GetComponent<LinearDrive>().startPosition.position.x)
                    axisX = -1;
                if (mr.GetComponent<LinearDrive>().endPosition.position.y > mr.GetComponent<LinearDrive>().startPosition.position.y)
                    axisY = 1;
                if (mr.GetComponent<LinearDrive>().endPosition.position.y < mr.GetComponent<LinearDrive>().startPosition.position.y)
                    axisY = -1;
                if (mr.GetComponent<LinearDrive>().endPosition.position.z > mr.GetComponent<LinearDrive>().startPosition.position.z)
                    axisZ = 1;
                if (mr.GetComponent<LinearDrive>().endPosition.position.z < mr.GetComponent<LinearDrive>().startPosition.position.z)
                    axisZ = -1;
                upperLimit = Mathf.Abs((mr.GetComponent<LinearDrive>().endPosition.position.x - mr.GetComponent<LinearDrive>().startPosition.position.x) // it can be just one of those cases
                    + (mr.GetComponent<LinearDrive>().endPosition.position.y - mr.GetComponent<LinearDrive>().startPosition.position.y)
                    + (mr.GetComponent<LinearDrive>().endPosition.position.z - mr.GetComponent<LinearDrive>().startPosition.position.z));

                j += "<axis xyz=\""+ axisX + " " + axisY + " " + axisZ + "\"/>";
                j += "<limit effort=\"300\" lower=\"0\" upper=\""+upperLimit+"\" velocity=\"10\"/>";
                j += "</joint>";
                s += j;
            }

            if(mr.GetComponent<CircularDrive>())
            {
                string j = "<joint name=\"" + uniqueName + "_joint\" type=\"fixed\">";
                j += "<origin rpy=\"" + g.transform.rotation.eulerAngles.x + " " + g.transform.rotation.eulerAngles.y + " " + g.transform.rotation.eulerAngles.z
                    + "\" xyz=\"" + g.transform.position.x + " " + g.transform.position.y + " " + g.transform.position.z + "\"/>";
                j += "<child link=\"" + uniqueName + "\"/>";
                //if (parentName != "")
                    j += "<parent link=\"" + parentName + "\"/>";
                
                int axisX = 0, axisY = 0, axisZ = 0;
                if (mr.GetComponent<CircularDrive>().axisOfRotation == CircularDrive.Axis_t.XAxis)
                    axisX = 1;
                if (mr.GetComponent<CircularDrive>().axisOfRotation == CircularDrive.Axis_t.YAxis)
                    axisY = 1;
                if (mr.GetComponent<CircularDrive>().axisOfRotation == CircularDrive.Axis_t.ZAxis)
                    axisZ = 1;
                j += "<axis xyz=\"" + axisX + " " + axisY + " " + axisZ + "\"/>";
                j += "<limit effort=\"300\" lower=\""+ mr.GetComponent<CircularDrive>().minAngle * Mathf.Deg2Rad + "\" upper=\"" + mr.GetComponent<CircularDrive>().maxAngle * Mathf.Deg2Rad + "\" velocity=\"10\"/>";            // in "pi"? (rad)
                j += "</joint>";
                s += j;
            }

            globalString += s;
        }
        globalString += "</robot>";

        File.WriteAllText(urdfName, globalString);
        Debug.Log("wrote " + urdfName);

    }

    List<GameObject> assetDatabase = new List<GameObject>();

    void loadFolder(string folder)
    {
        assetDatabase.AddRange(Resources.LoadAll<GameObject>("KitchenClashMeshes"));
        Debug.Log("size ADB:" + assetDatabase.Count);

        //return;
        //Debug.Log(folder);
        //Object[] os = AssetDatabase.LoadAllAssetsAtPath(folder);
        
        //foreach (Object asset in os)
        //{
        //    assetDatabase.Add(asset);
        //    Debug.Log("Added: " + asset.name);
        //}
        //Debug.Log("os: " + os.Length);

        //string[] subfolders = Directory.GetDirectories(folder);
        //foreach (string subf in subfolders)
        //    loadFolder(subf);
    }

}
