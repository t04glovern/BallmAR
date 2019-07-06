using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using FFmpeg;

public class ScreenshotMovie : MonoBehaviour, IFFmpegHandler
{
    FFmpegHandler defaultHandler = new FFmpegHandler();
    FilesData config = new FilesData();
    SavWav savWav = new SavWav();

    public static string persistentDataPath;
    public static string streamingAssetsPath;
    public static string myDocumentsPath;

    // 60 seconds max recording time for audio
    private static int microphoneMaxRecordingTime = 60;

    public Image loadingBarFilled;

    AudioClip audio;
    bool capturing;
    bool encoding;

    /// <summary>
    /// The video folder, save recorded video.
    /// </summary>
    public static string folder;

	public int frameRate = 25;
    private int frameCount;
	public int sizeMultiplier = 1;

    float timer;

    public Button buttonUploadVideo;
    public MarklessAR marklessAR;

	private string realFolder = "";

    private void Awake()
    {
        FFmpegParser.Handler = this;
    }

	void Start()
	{
        persistentDataPath = Application.persistentDataPath;
        streamingAssetsPath = Application.streamingAssetsPath;
        myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        folder = persistentDataPath + "/BallmAR/Video/";
#else
        folder = myDocumentsPath + "/BallmAR/Video/";
#endif

        capturing = false;
        encoding = false;

        // Set the playback framerate!
        // (real time doesn't influence time anymore)
        Time.captureFramerate = frameRate;

        frameCount = 1;
        timer = 0.0f;
	}

	void Update()
	{
        timer += Time.deltaTime;

        if (capturing) {
            StartCoroutine(TakePhotoEnumerator());
        }
        if (encoding) {
            encoding = false;

            // Save audio clip to wav file
            Microphone.End("audio");
            audio = savWav.TrimSilence(audio, 0);
            savWav.Save(string.Format("{0}/audio", realFolder), audio);
            OnImagesPathInput();
            OnSoundPathInput();
            OnOutputPathInput();
            OnFPSInput();
            FFmpegCommands.Encode(config);
        }
	}

    public void OnImagesPathInput()
    {
        config.inputPath = string.Format("{0}/image-%4d.jpg", realFolder);
    }

    public void OnSoundPathInput()
    {
        config.soundPath = string.Format("{0}/audio.wav", realFolder);
    }

    public void OnOutputPathInput()
    {
        config.outputPath = string.Format("{0}/video.mp4", realFolder);
    }

    public void OnFPSInput()
    {
        config.fps = (float)frameRate;
    }

	public void StartVideo()
	{
		// Find a folder that doesn't exist yet by appending numbers!
		realFolder = folder;
		int count = 1;
		while (Directory.Exists(realFolder))
		{
			realFolder = folder + count;
			count++;
		}
		// Create the folder
		Directory.CreateDirectory(realFolder);

        if (marklessAR.cam != null) {
            audio = Microphone.Start("audio", false, microphoneMaxRecordingTime, 44100);
            // Start capturing
            capturing = true;
            timer = 0.0f;
        }
	}

    public void StopVideo() {
        
        // Stop capturing
        capturing = false;
        frameCount = 1;
        encoding = true;
    }

	IEnumerator TakePhotoEnumerator()
	{
        Texture2D texture = new Texture2D(marklessAR.cam.width, marklessAR.cam.height);
        texture.SetPixels(marklessAR.cam.GetPixels());
        texture.Apply();

        // filename is "/BallmAR/Video/imageX.jpg"
        var filepath = string.Format("{0}/image-{1:D04}.jpg", realFolder, frameCount++);

		// Encode to a PNG
        byte[] bytes = texture.EncodeToJPG();

		// Write out the PNG
		File.WriteAllBytes(filepath, bytes);

		FileInfo fileInfo = new FileInfo(filepath);
		while (fileInfo == null || fileInfo.Exists == false) {
			yield return null;
		} 
	}

    public void OnDirectInput(string commands)
    {
        FFmpegCommands.DirectInput(commands);
    }

    //FFmpeg processing callbacks
    //------------------------------
    public void OnStart()
    {
        defaultHandler.OnStart();
    }

    public void OnProgress(string msg)
    {
		defaultHandler.OnProgress(msg);
		Debug.Log(msg);
    }

    public void OnFailure(string msg)
    {
		defaultHandler.OnFailure(msg);
		Debug.Log(msg);
    }

    public void OnSuccess(string msg)
    {
		defaultHandler.OnSuccess(msg);
		Debug.Log(msg);
    }

    public void OnFinish()
    {
        defaultHandler.OnFinish();
        buttonUploadVideo.interactable = true;
    }

    public byte[] GetLatestVideo() {
        return File.ReadAllBytes(string.Format("{0}/video.mp4", realFolder));
    }
}
