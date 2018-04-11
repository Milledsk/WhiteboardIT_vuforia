using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class Screenshot : MonoBehaviour {
    int screenshotCount = 0;
    public GameObject screenshotPreview;
    public GameObject hideGameObject;
    public GameObject wait;

    private string javaScriptString; 

    private UIMethods ui;
    private byte[] imageData;
    private byte[] imagePart; 
    Texture2D screenshotTexture;
    private FeatureMatcher featureMatcher;
    ImageList imageList;

    // Use this for initialization
    void Start () {
        featureMatcher = new FeatureMatcher();
        imageList = new ImageList();

        ui = GameObject.Find("UIScripts").GetComponent<UIMethods>();

        InAppBrowserBridge bridge = GameObject.Find("InAppBrowserBridge").GetComponent<InAppBrowserBridge>();
        bridge.onJSCallback.AddListener(OnMessageFromJS);
    }
	
	public void CaptureScreen() {
        
        StartCoroutine(TakeScreenshot());
    }

    public IEnumerator TakeScreenshot()
    {
        string imageName = Application.dataPath + screenshotCount + ".png";

        //Hide canvas
        ui.Hide(hideGameObject);

        //Wait until rendering is done
        yield return new WaitForEndOfFrame();

        // Take the screenshot
        screenshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        screenshotTexture.Apply();

        //Show canvas
        ui.Show(hideGameObject);

        // Create a sprite
        Sprite screenshotSprite = Sprite.Create(screenshotTexture, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0.5f, 0.5f));

        // Set the sprite to the screenshotPreview
        screenshotPreview.GetComponent<Image>().sprite = screenshotSprite;

        //Show the image
        ui.Show(screenshotPreview);
    }

    private void ImagePartToByteArray()
    {
        //Get the proporties of the drawn square (if none, the whole picture)
        int[] proporties = screenshotPreview.GetComponent<DrawSquare>().GetSquareProporties();
        int buttonLeft_x = proporties[0];
        int buttonLeft_y = proporties[1];
        int width = proporties[2];
        int height = proporties[3];

        if (width == 0 || height == 0)
        {
            buttonLeft_x = 0;
            buttonLeft_y = 0;
            width = Screen.width;
            height = Screen.height;
        }
        Texture2D newImageTexture = new Texture2D(width, height);

        newImageTexture.SetPixels(screenshotTexture.GetPixels(buttonLeft_x, buttonLeft_y, width, height), 0);

        //Make the texture smaller for faster upload
        Texture2D resizedTexture = ScaleTexture(newImageTexture, width / 2, height / 2);
        //Texture2D resizedTexture = ScaleTexture(newImageTexture, width / 3 * 2, height / 3 * 2);

        // Encode texture into PNG
        imagePart = newImageTexture.EncodeToPNG();

        //Destroy objects
        GameObject.Destroy(newImageTexture);
        GameObject.Destroy(resizedTexture);
    }

    //InAppBrpwser Upload
    public void UploadToInAppBrowser(string base64Image, int id)
    {
        
        BrowserOpener browserOpener = GameObject.Find("InAppBrowserBridge").GetComponent<BrowserOpener>();

        javaScriptString = "window.addImage(\"data:image/png;base64," + base64Image + "\" , " + id + ")";
        browserOpener.OpenBrowser();   
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = ((float)1 / source.width) * ((float)source.width / targetWidth);
        float incY = ((float)1 / source.height) * ((float)source.height / targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth),
                              incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    public void MatchAndPost()
    {
        StartCoroutine(MatchFeatures());
    }

    //Match features from canvas
    public IEnumerator MatchFeatures()
    {
        ImageString image;
        GameObject instantiatedWait = Instantiate(wait, screenshotPreview.transform);

        //Pick the pixels inside the square
        ImagePartToByteArray();

        //Get imageList
        yield return imageList.GetTextFromURL();
        Debug.Log("Image list count: " + imageList.getImageList().Count);

        //Don't match features if no images is on the canvas
        if (imageList.getImageList().Count == 0)
        {
            image = new ImageString(Convert.ToBase64String(imagePart), -1);
        }
        // Match feature from images on canvas. 
        else
        {
             image = featureMatcher.MatchFeatures(Convert.ToBase64String(imagePart), imageList.getImageList());
        }
        Destroy(instantiatedWait);
        UploadToInAppBrowser(image.getImageString(), image.getWinnerIndex());
        
    }

    //Listener : check if webstrate page is loaded
    void OnMessageFromJS(string jsMessage)
    {
        if (jsMessage.Equals("pageLoaded"))
        {
            Debug.Log("PageLoaded message received!");
            InAppBrowser.ExecuteJS(javaScriptString);
            ui.Hide(screenshotPreview);
        }
    }

}
