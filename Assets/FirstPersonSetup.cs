using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonSetup : MonoBehaviour {

    // Use this for initialization
    void Start () {
        GetComponent<VRM.VRMFirstPerson>().Setup();
        foreach (var renderer in GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            renderer.updateWhenOffscreen = true;
        }
    }

    // Update is called once per frame
    void Update () {

    }
}