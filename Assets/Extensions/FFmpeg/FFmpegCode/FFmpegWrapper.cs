using System.Collections;
using UnityEngine;

namespace FFmpeg
{
    public class FFmpegWrapper : MonoBehaviour
    {
        AndroidJavaClass unityClass;
        AndroidJavaObject pluginClass;

        //------------------------------

        class AndroidCallback : AndroidJavaProxy
        {
            public AndroidCallback() : base("com.botvinev.max.unityplugin.CallbackNotifier") { }
            void onEnd(string message)
            {
                FFmpegParser.Handle(message);
            }
        }

        //------------------------------

        void Start()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            pluginClass = new AndroidJavaObject("com.botvinev.max.unityplugin.VideoProcessing");
            pluginClass.Call(
                "Begin",
                unityClass.GetStatic<AndroidJavaObject>("currentActivity"), //Context
                new AndroidCallback());
#else
            Debug.LogWarning("FFmpeg is not implemented for " + Application.platform);
#endif
        }

        internal void Execute(string[] cmd)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            pluginClass.Call(
                "Process",
                unityClass.GetStatic<AndroidJavaObject>("currentActivity"),  //Context
                cmd,
                new AndroidCallback());
#else
            Debug.LogWarning("FFmpeg is not implemented for " + Application.platform);
#endif
        }
    }
}