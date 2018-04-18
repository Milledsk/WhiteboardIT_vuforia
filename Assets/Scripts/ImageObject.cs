using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using System;

public class ImageObject
{

    public string image;
    public int index = -1;
    public List<MatOfDMatch> matches;
    public MatOfKeyPoint keyPoints;
    public Mat imageMat;
    public int value = 0; 

    public ImageObject()
    {

    }

    public ImageObject(string image, int index)
    {
        this.image = image;
        this.index = index;
    }
}