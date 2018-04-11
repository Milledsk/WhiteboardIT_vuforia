using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBrowserOpener : MonoBehaviour {

    // check readme file to find out how to change title, colors etc.
    public void OpenBrowser()
    {
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.displayURLAsPageTitle = false;

        InAppBrowser.OpenURL(Manager.Instance.controllerUrl, options);
    }

    public void ClearCache()
    {
        InAppBrowser.ClearCache();
    }
}
