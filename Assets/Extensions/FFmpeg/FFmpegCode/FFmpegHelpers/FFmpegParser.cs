using System;
using UnityEngine;

namespace FFmpeg
{
    public static class FFmpegParser
    {
        internal static IFFmpegHandler Handler { get; set; }
        //Data
        const string COMMAND_CODE = "FFmpeg COMMAND: ";
        const string START_CODE = "onStart";
        const string PROGRESS_CODE = "onProgress: ";
        const string FAILURE_CODE = "onFailure: ";
        const string SUCCESS_CODE = "onSuccess: ";
        const string FINISH_CODE = "onFinish";

        //------------------------------

        internal static void Handle(string message)
        {
            //Print
            if (message.Contains("FFmpeg EXCEPTION: "))
                Debug.LogException(new System.Exception(message));
            else
                Debug.Log(message);

            //Handle
            if (message.Contains(COMMAND_CODE))
            {

                message = message.Remove(0, COMMAND_CODE.Length);

                if (message.Contains(START_CODE))
                {
                    Handler.OnStart();
                }
                else if (message.Contains(PROGRESS_CODE))
                {
                    Handler.OnProgress(message.Remove(0, PROGRESS_CODE.Length));
                }
                else if (message.Contains(FAILURE_CODE))
                {
                    Handler.OnFailure(message.Remove(0, FAILURE_CODE.Length));
                }
                else if (message.Contains(SUCCESS_CODE))
                {
                    Handler.OnSuccess(message.Remove(0, SUCCESS_CODE.Length));
                }
                else if (message.Contains(FINISH_CODE))
                {
                    Handler.OnFinish();
                }
            }
        }

    }
}