using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSize : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ((RectTransform)transform).sizeDelta = new Vector2(Screen.width, Screen.height);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
