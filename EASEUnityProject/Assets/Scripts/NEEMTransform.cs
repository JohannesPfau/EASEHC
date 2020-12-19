using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NEEMTransform
{
    public NEEMTransform(string objName, double unixTimestamp, Vector3 pos, Quaternion rot)
    {
        header = new tfHeader();
        header.stamp = "Date("+unixTimestamp+")";
        header.frame_id = "KitchenClashScene";
        child_frame_id = objName;
        transform = new tfTransform();
        transform.translation = new tfTransform.tfVector3();
        transform.rotation = new tfTransform.tfQuaternion();
        transform.translation.x = pos.x;
        transform.translation.y = pos.y;
        transform.translation.z = pos.z;
        transform.rotation.x = rot.x;
        transform.rotation.y = rot.y;
        transform.rotation.z = rot.z;
        transform.rotation.w = rot.w;
        __recorded = "Date(" + unixTimestamp + ")";
    }

    public tfHeader header;
    public string child_frame_id;
    public tfTransform transform;
    public string __recorded;
    public string topic = "tf";

    [Serializable]
    public class tfHeader
    {
        public int seq = 0;
        public string stamp;
        public string frame_id;
    }

    [Serializable]
    public class tfTransform
    {
        public tfVector3 translation;
        public tfQuaternion rotation;

        [Serializable]
        public class tfVector3
        {
            public float x;
            public float y;
            public float z;
        }
        [Serializable]
        public class tfQuaternion
        {
            public float x;
            public float y;
            public float z;
            public float w;
        }
    }
}
