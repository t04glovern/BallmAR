using UnityEngine;
using UnityEngine.UI;

public class RandomVideoRecorder : MonoBehaviour
{
    public ScreenshotMovie screenshotMovie;

    public Button buttonStartCapture;
    public Button buttonStopCapture;

    public void StartRecording()
    {
        buttonStopCapture.interactable = true;
        buttonStartCapture.interactable = false;
        Debug.Log("Starting Capture");
        screenshotMovie.StartVideo();
    }

    public void StopRecording()
    {
        buttonStartCapture.interactable = true;
        buttonStopCapture.interactable = false;
        Debug.Log("Stopping Capture");
        screenshotMovie.StopVideo();
    }
}

