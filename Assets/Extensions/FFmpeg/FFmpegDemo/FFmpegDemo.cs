using System;
using UnityEngine;
using UnityEngine.UI;

namespace FFmpeg.Demo
{
    public class FFmpegDemo : MonoBehaviour, IFFmpegHandler
    {
        public EncodeView encodeView;
        public DecodeView decodeView;
        public TrimView trimView;
        public ConvertView convertView;
        public Text field;
        FFmpegHandler defaultHandler = new FFmpegHandler();

        //------------------------------

        private void Awake()
        {
            FFmpegParser.Handler = this;
        }

        //------------------------------

        public void OnVersion()
        {
            FFmpegCommands.GetVersion();
        }

		//------------------------------

		public void OnConvert()
		{
            convertView.Open();
		}

		//------------------------------

		public void OnTrim()
		{
			trimView.Open();
		}

		//------------------------------

		public void OnDecode()
		{
			decodeView.Open();
		}

		//------------------------------

		public void OnEncode()
		{
			encodeView.Open();
		}

		//------------------------------

		public void OnDirectInput(string commands)
        {
            FFmpegCommands.DirectInput(commands);
        }

        //------------------------------

        public void Print(string message)
        {
            field.text = message;
        }

        //FFmpeg processing callbacks
        //------------------------------

        //Begining of video processing
        public void OnStart()
        {
            defaultHandler.OnStart();
        }

		//You can make progress bar here (parse msg)
		public void OnProgress(string msg)
        {
            
            defaultHandler.OnProgress(msg);
            Print(msg);
        }

		//Notify user about failure here
		public void OnFailure(string msg)
        {
            defaultHandler.OnFailure(msg);
            Print(msg);
        }

		//Notify user about success here
		public void OnSuccess(string msg)
        {
			defaultHandler.OnSuccess(msg);
            Print(msg);
        }

		//Last callback - do whatever you need next
		public void OnFinish()
        {
            defaultHandler.OnFinish();
        }
    }
}