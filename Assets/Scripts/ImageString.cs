using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using System;

public class ImageString {

    private string image;
    private int index;

    public ImageString(string image, int index)
    {
        this.image = image;
        this.index = index;
    }

    public string getImageString()
    {
        return image;
    }

    public int getWinnerIndex()
    {
        return index;
    }
}
