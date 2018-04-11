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

    public ImageString MatchFeatures(string base64image, List<string> base64imageList)
    {
        List<MatOfDMatch> winnerMatches = new List<MatOfDMatch>();
        MatOfKeyPoint winnerKeyPoints = new MatOfKeyPoint();
        Mat winnerImage = new Mat();
        int winnerIndex = -1;
        int winnerValue = 0;

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

        Debug.Log("Billede features: " + descriptors1.rows());

        if (descriptors1.rows() < 10)
        {
            Debug.Log("ARRRRRRGH der er ikke mange descripters i mit original-billede");
            return new ImageString(base64image, winnerIndex);
        }

            //Run through each image in list
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
            Debug.Log("Billede2 features: " + descriptors2.rows());
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
            if (good.Count > winnerThreshold && good.Count > winnerValue)
            {
                winnerImage = img2Mat;
                winnerMatches = good;
                winnerKeyPoints = keypoints2;
                winnerIndex = i;
                winnerValue = good.Count;
               
            }
        }

        Debug.Log("The winner is image: " + winnerIndex + " with a value of: " + winnerValue);

        //If no winner just return the original image
        if(winnerIndex == -1)
        {
            Debug.Log("No winner");
            return new ImageString(base64image, winnerIndex);
        }

        Debug.Log("No winner");
        //Find the matching keypoints from the winner list.  
        MatOfPoint2f queryPoints = new MatOfPoint2f();
        MatOfPoint2f matchPoints = new MatOfPoint2f();

        List<Point> queryPointsList = new List<Point>();
        List<Point> matchPointsList = new List<Point>();


        foreach (MatOfDMatch match in winnerMatches)
        {
            DMatch[] arrayDmatch = match.toArray();
            queryPointsList.Add(keypoints1.toList()[arrayDmatch[0].queryIdx].pt);
            matchPointsList.Add(winnerKeyPoints.toList()[arrayDmatch[0].trainIdx].pt);
        }
        queryPoints.fromList(queryPointsList);
        matchPoints.fromList(matchPointsList);

        //Calculate the homography of the best matching image
        Mat homography = Calib3d.findHomography(queryPoints, matchPoints, Calib3d.RANSAC, 5.0);
        Mat resultImg = new Mat();
        Imgproc.warpPerspective(img1Mat, resultImg, homography, winnerImage.size());

        //Show image
        Texture2D texture = new Texture2D(winnerImage.cols(), winnerImage.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(resultImg, texture);

        return new ImageString(Convert.ToBase64String(texture.EncodeToPNG()), winnerIndex);
    }

    private Texture2D base64ImageToTexture(string image)
    {
        byte[] bytes = Convert.FromBase64String(image);
        Texture2D resultTexture = new Texture2D(1, 1);
        resultTexture.LoadImage(bytes);

        return resultTexture;
    }
}
