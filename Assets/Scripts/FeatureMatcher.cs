using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using System;

//Inspired by OpenCV python feature matcher: https://docs.opencv.org/3.3.0/dc/dc3/tutorial_py_matcher.html

public class FeatureMatcher {

    private Texture2D photo;
    private List<Texture2D> photos;
    private int winnerThreshold = 10;

    public List<ImageObject> MatchFeatures(string base64image, List<string> base64imageList)
    {
        ImageObject myImage = new ImageObject();
        ImageObject winnerImage = new ImageObject();
        List<ImageObject> returnImageList = new List<ImageObject>();

        Texture2D imgTexture = base64ImageToTexture(base64image);
        List<Texture2D> imgTextures = new List<Texture2D>();
        for(int i = 0; i < base64imageList.Count; i++)
        {
            imgTextures.Add(base64ImageToTexture(base64imageList[i]));
        }

        //Create Mat from texture
        Mat img1Mat = new Mat(imgTexture.height, imgTexture.width, CvType.CV_8UC3);
        Utils.texture2DToMat(imgTexture, img1Mat);
        MatOfKeyPoint keypoints1 = new MatOfKeyPoint();
        Mat descriptors1 = new Mat();

        FeatureDetector detector = FeatureDetector.create(FeatureDetector.ORB);
        DescriptorExtractor extractor = DescriptorExtractor.create(DescriptorExtractor.ORB);

        //Detect keypoints and compute descriptors from photo. 
        detector.detect(img1Mat, keypoints1);
        extractor.compute(img1Mat, keypoints1, descriptors1);

        //Debug.Log("Billede features: " + descriptors1.rows());

        myImage.image = base64image;
        myImage.keyPoints = keypoints1;
        myImage.imageMat = img1Mat;

        if (descriptors1.rows() < 10)
        {
            Debug.Log("ARRRRRRGH der er ikke mange descripters i mit original-billede");

            //No winner as there is to few descriptors. 

            return returnImageList;
        }

        //Run through each image in list
        //-------------------------------------------------------------
        for (int i = 0; i < imgTextures.Count; i++)
        {
            Texture2D imgTexture2 = imgTextures[i];

            //Create Mat from texture
            Mat img2Mat = new Mat(imgTexture2.height, imgTexture2.width, CvType.CV_8UC3);
            Utils.texture2DToMat(imgTexture2, img2Mat);

            //Find keypoints and descriptors from image in list
            MatOfKeyPoint keypoints2 = new MatOfKeyPoint();
            Mat descriptors2 = new Mat();

            detector.detect(img2Mat, keypoints2);
            extractor.compute(img2Mat, keypoints2, descriptors2);

            //Match photo with image from list
            DescriptorMatcher matcher = DescriptorMatcher.create(DescriptorMatcher.BRUTEFORCE_HAMMINGLUT);
            //Debug.Log("Billede2 features: " + descriptors2.rows());
            if (descriptors2.rows() < 10)
            {
                Debug.Log("ARRRRRRGH der er ikke mange descripters i mit test billede: " + i);
                continue;
            }

            List<MatOfDMatch> matchList = new List<MatOfDMatch>();
            matcher.knnMatch(descriptors1, descriptors2, matchList, 2);

            //Find the good matches and put them ind a list
            List<MatOfDMatch> good = new List<MatOfDMatch>();

            foreach (MatOfDMatch match in matchList)
            {
                DMatch[] arrayDmatch = match.toArray();
                if (arrayDmatch[0].distance < 0.7f * arrayDmatch[1].distance)
                {
                    good.Add(match);
                }
            }

            //Find the best match image based on the good lists
            if (good.Count > winnerThreshold && good.Count > winnerImage.value)
            {
                winnerImage.index = i;
                winnerImage.imageMat = img2Mat;
                winnerImage.keyPoints = keypoints2;
                winnerImage.value = good.Count;
                winnerImage.matches = good; 
            }
        }
        // Run through done
        //-------------------------------------------------------------

        Debug.Log("The winner is image: " + winnerImage.index + " with a value of: " + winnerImage.value);

        //If no winner just return the original image
        if(winnerImage.index == -1)
        {
            Debug.Log("No winner");

            return returnImageList;
        }

        Texture2D imageTexture = new Texture2D(winnerImage.imageMat.cols(), winnerImage.imageMat.rows(), TextureFormat.RGBA32, false);
        winnerImage.image = Convert.ToBase64String(imageTexture.EncodeToPNG());

        returnImageList.Add(myImage);
        returnImageList.Add(winnerImage);

        return returnImageList;

    }

    
    public ImageObject warpImage(List<ImageObject> imageList)
    {
        Texture2D imgTexture = base64ImageToTexture(imageList[0].image);

        //Find the matching keypoints from the winner list.  
        MatOfPoint2f queryPoints = new MatOfPoint2f();
        MatOfPoint2f matchPoints = new MatOfPoint2f();

        List<Point> queryPointsList = new List<Point>();
        List<Point> matchPointsList = new List<Point>();


        foreach (MatOfDMatch match in imageList[1].matches)
        {
            DMatch[] arrayDmatch = match.toArray();
            queryPointsList.Add(imageList[0].keyPoints.toList()[arrayDmatch[0].queryIdx].pt);
            matchPointsList.Add(imageList[1].keyPoints.toList()[arrayDmatch[0].trainIdx].pt);
        }
        queryPoints.fromList(queryPointsList);
        matchPoints.fromList(matchPointsList);

        //Calculate the homography of the best matching image
        //Mat homography = Calib3d.findHomography(queryPoints, matchPoints, Calib3d.RANSAC, 5.0);
        Mat homography = Calib3d.findHomography(queryPoints, matchPoints, Calib3d.RANSAC, 3.0);
        Mat resultImg = new Mat();
        Imgproc.warpPerspective(imageList[0].imageMat, resultImg, homography, imageList[1].imageMat.size());

        //Show image
        Texture2D texture = new Texture2D(imageList[1].imageMat.cols(), imageList[1].imageMat.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(resultImg, texture);

        return new ImageObject(Convert.ToBase64String(texture.EncodeToPNG()), imageList[1].index);
    }
    
    public int FindBestMatchIndex(string base64image, List<string> base64imageList)
    {
        
        List<ImageObject> imageList = MatchFeatures(base64image, base64imageList);
        if(imageList.Count == 0)
        {
            return -1;
        }
        return imageList[1].index;
    }

    public ImageObject MatchAndWarp(string base64image, List<string> base64imageList)
    {
        List<ImageObject> imageList = MatchFeatures(base64image, base64imageList);

        if (imageList.Count == 0) //No winner. Return original image and index = -1
        {
            return new ImageObject(base64image, -1);

        } else //yesh, there is a winner. 
        {
            return warpImage(imageList);
        }
    }

    public ImageObject DownloadImage(string base64image, List<string> base64imageList)
    {
        List<ImageObject> imageList = MatchFeatures(base64image, base64imageList);

        if (imageList.Count == 0)
        {
            return new ImageObject(null, -1);
        }

        Debug.Log("Image: " +imageList[1].image);

        return imageList[1];
    }

    private Texture2D base64ImageToTexture(string image)
    {
        byte[] bytes = Convert.FromBase64String(image);
        Texture2D resultTexture = new Texture2D(1, 1);
        resultTexture.LoadImage(bytes);

        return resultTexture;
    }
}
