using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class ImageList {

    private List<string> images;

    public List<string> getImageList()
    {
        return images; 
    }

    // Get data from website
    public IEnumerator GetTextFromURL()
    {
        Debug.Log("Get text from url has started");

        UnityWebRequest www = UnityWebRequest.Get(Manager.Instance.canvasUrl);
        Debug.Log("Canvas url: " + Manager.Instance.canvasUrl);


        using (www)
        {

            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log("Error while Receiving: " + www.error);
            }
            else
            {
                //get text
                string result = www.downloadHandler.text;

                //Debug.Log("website text: " + result);

                //Get <div id=\"sharedCanvas\" substring
                int startIndex = result.IndexOf("<ul id=\"sharedCanvas\"");
                Debug.Log("Startindex: " + startIndex);
                int endIndex = result.IndexOf("</ul>") + "</ul>".Length;
                string body = result.Substring(startIndex, endIndex - startIndex);
                // change & to &amp; for valid xml
                string xml = body.Replace("&", "&amp;");

                //Debug.Log("website xml: " + xml);

                ParseXLMToImageList(xml);

            }
        }
       
    }

    void ParseXLMToImageList(string xml)
    {
        images = new List<string>();
        //read xml
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        foreach (XmlElement node in xmlDoc.SelectNodes("ul"))
        {
            Debug.Log("Parent node found: " + node.OuterXml);

            foreach (XmlElement li in node)
            {
                Debug.Log("Li node found: " + li.OuterXml);

                foreach (XmlElement imgNode in li)
                {
                    Debug.Log("imgNode found: " + imgNode.OuterXml);
                    string tempString = imgNode.OuterXml;
                    //Debug.Log("image node string: " + tempString);
                    int startIndex = tempString.IndexOf("64") + 3;

                    int first = tempString.IndexOf("\"");
                    int second = tempString.IndexOf("\"", first + 1);

                    //Debug.Log("Start index: " + startIndex);
                    //Debug.Log("Second index: " + second);

                    string base64Substring = tempString.Substring(startIndex, second - startIndex);

                    //add image to list
                    images.Add(base64Substring);
                }
            }

        }

        //Debug.Log("Imagelist count: " + images.Count);
    }
}
