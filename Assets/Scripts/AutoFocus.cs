using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class AutoFocus : MonoBehaviour {

	// Use this for initialization
	void Start () {
        CameraDevice myCam = CameraDevice.Instance;

        myCam.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
