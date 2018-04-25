using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using System;

public class ImageProcessor {

    public Texture2D removeColor(Color color, Texture2D image)
    {
        Texture2D newImage = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);
        int pixelsChanged = 0;

        Color[] pixels = image.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i] == color)
            {
                
                pixelsChanged++;
                pixels[i] = Color.clear;
            } else
            {
                //pixels[i] = Color.red;
            }
        }
        Debug.Log("Pixels changed: " + pixelsChanged);


        newImage.SetPixels(0, 0, image.width, image.height, pixels, 0);
        newImage.Apply();

        return newImage;
    }

    public Mat ShowAnnotationsOnly(Mat image)
    {
        Mat maskRed = new Mat();
        Core.inRange(image, new Scalar(90, 0, 0), new Scalar(256, 75, 75), maskRed);

        //Mat maskGreen = new Mat();
        //Core.inRange(image, new Scalar(0, 80, 0), new Scalar(40, 256, 140), maskGreen);

        Mat maskBlue = new Mat();
        Core.inRange(image, new Scalar(0, 0, 80), new Scalar(75, 75, 256), maskBlue);

        /* Take original color from image
        Mat mask = new Mat();
        Core.bitwise_or(maskRed, maskRed, mask);
        //Core.bitwise_or(maskGreen, mask, mask);
        Core.bitwise_or(maskBlue, mask, mask);

        Mat output = new Mat();
        Core.bitwise_and(image, image, output, mask);
        */
        
        // Use rgb red and blue instead of original colors to make annotations more visible
        Mat redImage = image.clone();
        redImage.setTo(new Scalar(0, 0, 255)); //bgr color format

        Mat blueImage = image.clone();
        redImage.setTo(new Scalar(255, 0, 0)); //bgr color format

        Mat outputRed = new Mat();
        Core.bitwise_and(redImage, redImage, outputRed, maskRed);

        Mat outputBlue = new Mat();
        Core.bitwise_and(blueImage, blueImage, outputBlue, maskBlue);

        Mat output = new Mat();
        Core.bitwise_or(outputRed, outputBlue, output);
        
        return output;
    }


}
