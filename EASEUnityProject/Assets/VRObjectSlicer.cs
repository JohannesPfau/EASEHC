using EzySlice;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class VRObjectSlicer : MonoBehaviour {

    float minimalPercentageCutoff = 0.15f;
    float minimalSizeTotal = 0.1f;

    public void Slice(GameObject source)
    {
        PlaneUsageExample plane = GetComponent<PlaneUsageExample>();
        Material crossMat = source.GetComponent<isCuttableObject>().innerMaterial;

        SlicedHull hull = plane.SliceObject(source, crossMat);

        if (hull != null)
        {
            GameObject lower = hull.CreateLowerHull(source, crossMat);
            GameObject upper = hull.CreateUpperHull(source, crossMat);

            try
            { 
                if (checkIfOverMinimal(lower, source) && checkIfOverMinimal(upper, source))
                {
                    source.GetComponent<isCuttableObject>().isCut = true;
                    applyChangesToHullPart(lower, source);
                    applyChangesToHullPart(upper, source);
                    Debug.Log("Slicing " + source.name);
                    source.SetActive(false);
                }
                else
                {
                    Destroy(lower);
                    Destroy(upper);
                }
            }
            catch (Exception e)
            {
                Destroy(lower);
                Destroy(upper);
            }
        }
    }

    public void applyChangesToHullPart(GameObject part, GameObject source)
    {
        part.name = source.name;
        InteractableVRObject ivro = part.AddComponent<InteractableVRObject>();
        ivro.grabbable = source.GetComponent<InteractableVRObject>().grabbable;
        ivro.throwable = source.GetComponent<InteractableVRObject>().throwable;
        ivro.displayedName = source.GetComponent<InteractableVRObject>().displayedName;
        ivro.commandOnTrigger = source.GetComponent<InteractableVRObject>().commandOnTrigger;
        isCuttableObject ico = part.AddComponent<isCuttableObject>();
        ico.innerMaterial = source.GetComponent<isCuttableObject>().innerMaterial;
        part.tag = source.tag;
    }

    public bool checkIfOverMinimal(GameObject part, GameObject source)
    {
        MeshCollider mc = part.AddComponent<MeshCollider>();
        mc.convex = true;
        part.AddComponent<VolumeOf>();
        //float percentage = part.GetComponent<VolumeOf>().getVolume() / source.GetComponent<VolumeOf>().getVolume();
        //if (percentage >= minimalPercentageCutoff)
        if(part.GetComponent<VolumeOf>().getVolume() > minimalSizeTotal)
            return true;

        return false;
    }



}
