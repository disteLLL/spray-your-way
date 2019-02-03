using UnityEngine;
using System.Linq;

using Vuforia;
using OpenCvSharp;

public class ColorTracker : MonoBehaviour
{
    public Scalar lowerHSVColor = new Scalar(25, 150, 100);
    public Scalar upperHSVColor = new Scalar(35, 255, 255);

    private Image.PIXEL_FORMAT mPixelFormat = Image.PIXEL_FORMAT.UNKNOWN_FORMAT;

    private bool mAccessCameraImage = true;
    private bool mFormatRegistered = false;
    
    private Mat inputMat;
    private Mat smallMat = new Mat();
    private Mat blurredMat = new Mat();
    private Mat hsvMat = new Mat();
    private Mat thresholdMat = new Mat();
    private Mat hierarchy = new Mat();
    private Mat[] contours;
    
    #region MONOBEHAVIOUR_METHODS

    void Start() {

    #if UNITY_EDITOR
        mPixelFormat = Image.PIXEL_FORMAT.GRAYSCALE; // Need Grayscale for Editor
    #else
        mPixelFormat = Image.PIXEL_FORMAT.RGB888; // Use RGB888 for mobile
    #endif

        // Register Vuforia life-cycle callbacks:
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);

    }

    #endregion // MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS

    void OnVuforiaStarted() {

        // Try register camera image format
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true)) {
            Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());

            mFormatRegistered = true;
        }
        else {
            Debug.LogError(
                "\nFailed to register pixel format: " + mPixelFormat.ToString() +
                "\nThe format may be unsupported by your device." +
                "\nConsider using a different pixel format.\n");

            mFormatRegistered = false;
        }
    }

    /// <summary>
    /// Called each time the Vuforia state is updated
    /// Tracks the given color and sets the position and rotation of the spraycan
    /// </summary>
    void OnTrackablesUpdated() {
        if (mFormatRegistered) {
            if (mAccessCameraImage) {

                Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);   // get the current camera image in the given pixel format

                if (image != null) {

                #if UNITY_EDITOR
                    inputMat = new Mat(image.Height, image.Width, MatType.CV_8UC1, image.Pixels);
                #else
                    inputMat = new Mat(image.Height, image.Width, MatType.CV_8UC3, image.Pixels);   // store the image's pixels in an OpenCV mat                 
                #endif

                    Cv2.Resize(inputMat, smallMat, new Size(480, 270)); // resizing for performance reasons (keep aspect ratio!)
                    Cv2.GaussianBlur(smallMat, blurredMat, new Size(11, 11), 0);    // blur image to reduce noise
                    Cv2.CvtColor(blurredMat, hsvMat, ColorConversionCodes.RGB2HSV); // convert to HSV colors
                    Cv2.InRange(hsvMat, lowerHSVColor, upperHSVColor, thresholdMat);    // filter out all pixels matching the given HSV range

                    Cv2.Erode(thresholdMat, thresholdMat, Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(3, 3)), null, 2);  // shave off pixels from blobs to eliminate small blobs
                    Cv2.Dilate(thresholdMat, thresholdMat, Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(3, 3)), null, 2);    // strengthen the remaining blobs
                   
                    Cv2.FindContours(thresholdMat, out contours, hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);   // detect the blobs and save them as contours

                    if(contours.Length > 0) {

                        Mat contour = contours.Aggregate((i, j) => i.ContourArea() > j.ContourArea() ? i : j);  // find the blob with the biggest ContourArea/Size

                        Point2f point;
                        float radius;
                        Cv2.MinEnclosingCircle(contour, out point, out radius); // get the radius for passing a final threshold

                        if(radius > 5) {

                            Moments moments = Cv2.Moments(contour); // use moments to calculate the center point of the biggest blob
                            double area = moments.M00;
                            double m01 = moments.M01;
                            double m10 = moments.M10;

                            double posX = m10 / area;
                            double posY = m01 / area;

                            double rotX = MapValue(posX, 0, 480, -31.5, 31.5);  // map the values to match coordinates usable in Unity
                            double rotY = MapValue(posY, 0, 270, -19.75, 19.75);

                            posX = MapValue(posX, 0, 480, -6, 6);
                            posY = MapValue(posY, 0, 270, 3.5, -3.5);

                            this.transform.localPosition = new Vector3((float)posX, (float)posY, 10);   // apply the changes to position and rotation
                            this.transform.localEulerAngles = new Vector3((float)rotY, (float)rotX, 0);
                        }
                    }                                     
                }
            }
        }
    }

    /// <summary>
    /// Called when app is paused / resumed
    /// </summary>
    void OnPause(bool paused) {
        if (paused) {
            Debug.Log("App was paused");
            UnregisterFormat();
        }
        else {
            Debug.Log("App was resumed");
            RegisterFormat();
        }
    }

    /// <summary>
    /// Register the camera pixel format
    /// </summary>
    void RegisterFormat() {
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true)) {
            Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else {
            Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = false;
        }
    }

    /// <summary>
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// </summary>
    void UnregisterFormat() {
        Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }

    /// <summary>
    /// Map value a from range (a0-a1) to range (b0-b1)
    /// </summary>
    double MapValue(double a, double a0, double a1, double b0, double b1) {
        return b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
    }

    #endregion //PRIVATE_METHODS

    #region LEGACY_CODE

    // First kind of working color tracking attempt

    //private byte r2 = 255;
    //private byte g2 = 0;
    //private byte b2 = 0;
    //private byte threshold = 80;


    /*
                    byte[] pixels = image.Pixels;

                    float avgX = 0;
                    float avgY = 0;
                    int count = 0;
                    

                    for (int x = 0; x < image.Width; x++) {

                        for (int y = 0; y < image.Height; y++) {

                            int location = (x + y * image.Width) * 3;

                            byte r1 = pixels[location];
                            byte g1 = pixels[location + 1];
                            byte b1 = pixels[location + 2];

                            int d = distSq(r1, g1, b1, r2, g2, b2);

                            

                            if(d < threshold * threshold) {
                                avgX += x;
                                avgY += y;
                                count++;
                            }
                        }
                    }
            
                    if (count > 0) {

                        avgX = avgX / count;
                        avgY = avgY / count;

                        avgX = (avgX / 72) - 5;
                        avgY = (avgY * -0.013875f) + 3.33f;

                        this.transform.localPosition = new Vector3(avgX, avgY, 10);

                    }
                    */

    /*
    int distSq(byte x1, byte y1, byte z1, byte x2, byte y2, byte z2) {
        int d = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1);
        return d;
    }
    */

    #endregion // LEGACY_CODE
}
