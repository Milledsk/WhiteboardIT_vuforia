using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

    Screenshot screenshot;

    UIMethods uiMethods; 

	// Use this for initialization
	void Start () {
        screenshot = FindObjectOfType<Screenshot>();        
        uiMethods = FindObjectOfType<UIMethods>();
    }

    public void OnPhotoButtonPressed()
    {
        screenshot.CaptureScreen();
    }

    public void OnCloseButtonPressed()
    {
        uiMethods.Hide(GameObject.Find("ImageObject"));
    }

    public void OnMatchImageButtonPressed()
    {
        screenshot.MatchAndPost();
    }

    public void OnButtonBackPressed()
    {
        SceneManager.LoadScene("StartWithQR");
    }


}
