using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ZXing;

public class CameraTexture : MonoBehaviour {

    public RawImage rawimage;
    private WebCamTexture webcamTexture;
    public GameObject succes;

    public Quaternion baseRotation;

    void Start()
    {
        Debug.Log("Start");
        webcamTexture = new WebCamTexture();
        ((RectTransform)transform).sizeDelta = new Vector2(Screen.height / 3 * 4, Screen.height);
        rawimage.texture = webcamTexture;
        rawimage.transform.localRotation = Quaternion.Euler(0, 0, -90);
        rawimage.material.mainTexture = webcamTexture;
        //baseRotation = transform.rotation;
        webcamTexture.Play();
        Debug.Log(webcamTexture);

        if (SceneManager.GetActiveScene().name == "StartWithQR")
        {
            StartCoroutine(ReadQR());
            Debug.Log("Read QR");
        }

    }

    void Update()
    {
        //transform.rotation = baseRotation * Quaternion.AngleAxis(webcamTexture.videoRotationAngle, Vector3.up);
    }

    IEnumerator ReadQR()
    {
        while (true)
        {

            IBarcodeReader barcodeReader = new BarcodeReader();
            // decode the current frame
            var result = barcodeReader.Decode(webcamTexture.GetPixels32(),
                webcamTexture.width, webcamTexture.height);

            if (result != null)
            {
                Debug.Log("DECODED TEXT FROM QR: " + result.Text);

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
            yield return new WaitForSeconds(0.5f);

        }

    }

    void OnDestroy()
    {
        webcamTexture.Stop();
        print("Script was destroyed");
    }



}
