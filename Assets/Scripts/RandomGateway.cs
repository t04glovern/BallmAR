using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


// LOTS OF BADLY WRITTEN CODE :D!

public class VideoBreakdownAsyncOperation
{
    [Serializable]
    public struct TimedData
    {
        public string name;
    }

    [Serializable]
    public struct SummarizedInsightsData
    {
        public TimedData[] topics;
        public TimedData[] annotations;
    }

    [Serializable]
    public class AnalysisData
    {
        public SummarizedInsightsData summarizedInsights;
    }

    private UnityWebRequestAsyncOperation operation;
    private AnalysisData result;

    public VideoBreakdownAsyncOperation(UnityWebRequestAsyncOperation operation)
    {
        this.operation = operation;
    }

    public bool IsDone
    {
        get
        {
            if (!this.operation.isDone)
                return false;

            if (this.result == null)
                this.result = JsonUtility.FromJson<AnalysisData>(this.operation.webRequest.downloadHandler.text);

            return true;
        }
    }

    public float Progress
    {
        get
        {
            return this.operation.progress;
        }
    }

    public AnalysisData Data
    {
        get
        {
            if (!this.IsDone)
                return null;

            return this.result;
        }
    }
}

public class VideoAnalysisAsyncOperation
{
    private UnityWebRequestAsyncOperation operation;
    private string breakdownId;

    public VideoAnalysisAsyncOperation(UnityWebRequestAsyncOperation operation)
    {
        this.operation = operation;
    }

    public bool IsDone
    {
        get
        {
            if (!this.operation.isDone)
                return false;

            if (string.IsNullOrEmpty(this.breakdownId))
                this.breakdownId = this.operation.webRequest.downloadHandler.text;

            return true;
        }
    }

    public float Progress
    {
        get
        {
            return this.operation.progress;
        }
    }

    public string BreakdownId
    {
        get
        {
            if (!this.IsDone)
                return "";

            return this.breakdownId;
        }
    }
}

public class ImageAnalysisDataAsyncOperation
{
    [Serializable]
    public struct AnalysisTagData
    {
        public string name;
        public float confidence;
    }

    [Serializable]
    public class AnalysisData
    {
        public AnalysisTagData[] tags;
    }

    private UnityWebRequestAsyncOperation operation;
    private AnalysisData result;

    public ImageAnalysisDataAsyncOperation(UnityWebRequestAsyncOperation operation)
    {
        this.operation = operation;
    }

    public bool IsDone
    {
        get
        {
            if (!this.operation.isDone)
                return false;

            if (this.result == null)
                this.result = JsonUtility.FromJson<AnalysisData>(this.operation.webRequest.downloadHandler.text);

            return true;
        }
    }

    public float Progress
    {
        get
        {
            return this.operation.progress;
        }
    }

    public float UploadProgress
    {
        get
        {
            return this.operation.webRequest.uploadHandler.progress;
        }
    }

    public AnalysisData Data
    {
        get
        {
            if (!this.IsDone)
                return null;

            return this.result;
        }
    }
}

public class RandomGateway : MonoBehaviour
{
    private const string COMPUTER_VISION_ENDPOINT = "https://southeastasia.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Categories,Tags,Description,Faces,ImageType,Color,Adult&details=Landmarks,Celebrities&language=en";
    private const string COMPUTER_VISION_KEY = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";

    private const string VIDEO_INDEX_ENDPOINT = "https://videobreakdown.azure-api.net/Breakdowns/Api/Partner/Breakdowns";
    private const string VIDEO_INDEX_KEY = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";

    private const string VIDEO_TEST = "";

    public ScreenshotMovie screenShotMovie;

    public VideoAnalysisAsyncOperation latestVideoAnalysisAsyncOperation;
    public void UploadMostRecentVideo()
    {
        byte[] video = this.screenShotMovie.GetLatestVideo();
        this.latestVideoAnalysisAsyncOperation = this.AnalyzeVideo(video);
    }

    public ImageAnalysisDataAsyncOperation AnalyzeImage(byte[] imageData)
    {
        UnityWebRequest request = new UnityWebRequest(COMPUTER_VISION_ENDPOINT, UnityWebRequest.kHttpVerbPOST);

        UploadHandlerRaw handler = new UploadHandlerRaw(imageData);
        handler.contentType = "application/octet-stream";

        request.SetRequestHeader("Ocp-Apim-Subscription-Key", COMPUTER_VISION_KEY);
        request.uploadHandler = handler;
        request.downloadHandler = new DownloadHandlerBuffer();

        return new ImageAnalysisDataAsyncOperation(request.SendWebRequest());
    }

    public VideoAnalysisAsyncOperation AnalyzeVideo(byte[] videoData)
    {
        string uri = VIDEO_INDEX_ENDPOINT
            + "?name=" + DateTime.Now.ToFileTimeUtc()
            + "&privacy=private"
            + "&language=en-us";

        WWWForm formData = new WWWForm();
        formData.AddBinaryData("File", videoData);

        UnityWebRequest request = UnityWebRequest.Post(uri, formData);

        request.SetRequestHeader("Ocp-Apim-Subscription-Key", VIDEO_INDEX_KEY);
        request.downloadHandler = new DownloadHandlerBuffer();

        return new VideoAnalysisAsyncOperation(request.SendWebRequest());
    }

    public VideoBreakdownAsyncOperation GetVideoBreakdown(string breakdownId)
    {
        string uri = VIDEO_INDEX_ENDPOINT
            + "/" + breakdownId
            + "?language=en-us";

        UnityWebRequest request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);

        request.SetRequestHeader("Ocp-Apim-Subscription-Key", VIDEO_INDEX_KEY);
        request.downloadHandler = new DownloadHandlerBuffer();

        return new VideoBreakdownAsyncOperation(request.SendWebRequest());
    }

    private bool isAnalyticsCurrentlyRunning = false;

    private void Update()
    {
        if (this.latestVideoAnalysisAsyncOperation != null)
        {
            this.StartCoroutine("DoVideoAnalyzeCheck");
        }
    }

    IEnumerator DoVideoAnalyzeCheck()
    {
        if (!this.latestVideoAnalysisAsyncOperation.IsDone && !this.isAnalyticsCurrentlyRunning)
        {
            this.isAnalyticsCurrentlyRunning = true;
            Debug.Log("LOADING: " + this.latestVideoAnalysisAsyncOperation.Progress);
        }
        else
        {
            this.isAnalyticsCurrentlyRunning = false;
            Debug.Log("BREAKDOWN ID: " + this.latestVideoAnalysisAsyncOperation.BreakdownId);
            this.StopCoroutine("DoVideoAnalyzeCheck");
            yield return new WaitForSeconds(15.0f);
        }
    }
}
