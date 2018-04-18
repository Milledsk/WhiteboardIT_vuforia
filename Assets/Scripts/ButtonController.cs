using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

    Screenshot screenshot;

    UIMethods uiMethods;

    MyBrowserOpener browserOpener;

	// Use this for initialization
	void Start () {
        screenshot = FindObjectOfType<Screenshot>();        
        uiMethods = FindObjectOfType<UIMethods>();
        browserOpener = FindObjectOfType<MyBrowserOpener>();
    }

    public void OnPhotoButtonPressed()
    {
        screenshot.CaptureScreen();
    }

    public void OnCloseButtonPressed()
    {
        uiMethods.Hide(GameObject.Find("Panel"));
        uiMethods.Hide(GameObject.Find("ImageObject"));
  
    }

    public void OnMatchImageButtonPressed()
    {
        StartCoroutine(screenshot.MatchWarpAndPost());
    }

    public void OnSelectImageButtonPressed()
    {
        StartCoroutine(screenshot.MatchAndOpenControls());
    }

    public void OnDownloadImageButtonPressed()
    {
        StartCoroutine(screenshot.MatchAndDownload());
    }


    public void OnButtonBackPressed()
    {
        SceneManager.LoadScene("StartWithQR");
    }

    public void OnButtonOpenControlPressed()
    {
        browserOpener.OpenBrowser();
    }


}
