using UnityEngine;
using System;
using System.Collections;
using Vuforia;

using ZXing;
using UnityEngine.SceneManagement;

/*  ///////////////// QR detection does not work in editor //////////////// */

[AddComponentMenu("System/QRScanner")]
public class QRScannerVuforia : MonoBehaviour
{
    public GameObject succes; 

    private bool cameraInitialized;
    private IBarcodeReader barCodeReader;
    private Vuforia.Image.PIXEL_FORMAT mPixelFormat = Vuforia.Image.PIXEL_FORMAT.UNKNOWN_FORMAT;
    private Vuforia.Image image;
    private Result result;
    private int i = 0; 

    bool QRVisible = false;

    static Material lineMaterial;

    void Start()
    {
        Debug.Log("Start bliver kaldt igen");
        barCodeReader = new BarcodeReader();

        StartCoroutine(InitializeCamera());

    }

    private IEnumerator InitializeCamera()
    {
        // Waiting a little seem to avoid the Vuforia's crashes.
        yield return new WaitForSeconds(3f);


        mPixelFormat = Vuforia.Image.PIXEL_FORMAT.RGB888; // Use RGB888 for mobile

        var isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(mPixelFormat, true);
        

        if (isFrameFormatSet)
        {
            cameraInitialized = true;
            Debug.Log("Initilaze camera with format: " + mPixelFormat);
        }

    }

    void Update()
    {
        
        if (cameraInitialized)
        {
            i++;
            image = CameraDevice.Instance.GetCameraImage(mPixelFormat);
            
            if (image == null)
            {
                Debug.Log("No camera image found");
                return; 
            }

            result = barCodeReader.Decode(image.Pixels, image.BufferWidth, image.BufferHeight, RGBLuminanceSource.BitmapFormat.RGB24);

            image = null; 

            if (result != null)
            {
                // QRCode detected.
                QRVisible = true;

                Debug.Log(i + " QR code: " + result.Text);
                //Do something with the QR code. 

                GameObject instantiatedSucces = Instantiate(succes, GameObject.Find("Canvas").transform);

                Manager.Instance.controllerUrl = result.Text;
                int firstIndex = 0;
                int secondIndex = result.Text.IndexOf("can");
                int thirdIndex = result.Text.IndexOf("=") + 1;

                string canvasUrl = result.Text.Substring(firstIndex, secondIndex) + result.Text.Substring(thirdIndex);
                Debug.Log(i + " Canvas string: " + canvasUrl);
                Manager.Instance.canvasUrl = canvasUrl + "?raw";
                result = null;  // clear data
                //yield return new WaitForSeconds(2);
                Destroy(instantiatedSucces);
                cameraInitialized = false;
                SceneManager.LoadScene("SnapshotMedFeatureMatcher");
                
            }
            //Debug.Log(i + " No QR code found");
        }

    }
    /*
    /// Called when app is paused / resumed
    void OnPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("App was paused");
            UnregisterFormat();
        }
        else
        {
            Debug.Log("App was resumed");
            RegisterFormat();
        }
    }

    /// Register the camera pixel format
    void RegisterFormat()
    {
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
            cameraInitialized = true;
        }
        else
        {
            Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
            cameraInitialized = false;
        }
    }

    /// Unregister the camera pixel format (e.g. call this when app is paused)
    void UnregisterFormat()
    {
        Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
        cameraInitialized = false;
    }
    */
}


/*
private void Update()
{
    if (cameraInitialized)
    {
        try
        {
            Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);

            if (image == null)
            {
                Debug.Log("No camera image found");
                return;
            }

            var data = barCodeReader.Decode(image.Pixels, image.BufferWidth, image.BufferHeight, RGBLuminanceSource.BitmapFormat.RGB24);
            if (data != null)
            {
                // QRCode detected.
                QRVisible = true;

                Debug.Log("QR code: " + data.Text);
                //Do something with the QR code. 

                data = null;  // clear data

                GameObject instantiatedSucces = Instantiate(succes, GameObject.Find("Canvas").transform);

                Manager.Instance.controllerUrl = result.Text;
                int firstIndex = 0;
                int secondIndex = result.Text.IndexOf("can");
                int thirdIndex = result.Text.IndexOf("=") + 1;

                string canvasUrl = result.Text.Substring(firstIndex, secondIndex) + result.Text.Substring(thirdIndex);
                Debug.Log("Canvas string: " + canvasUrl);
                Manager.Instance.canvasUrl = canvasUrl + "?raw";

                yield return new WaitForSeconds(2);

                SceneManager.LoadScene("SnapshotMedFeatureMatcher");
                break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
*/
