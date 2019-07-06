using UnityEngine;
using FFmpeg;
using System;

/// <summary>
/// Entry point for FFmpeg
/// </summary>
public static class FFmpegCommands
{
    //Do not call this
    static FFmpegWrapper w;
    //Call this ------------|
    //                      |
    //                     \|/
    static FFmpegWrapper Wrapper
    {
        get
        {
            if (w == null)
            {
                w = MonoBehaviour.FindObjectOfType<FFmpegWrapper>();
                if (w == null)
                    Debug.LogException(new Exception("Place a FFmpeg.prefab in the scene"));
            }
            return w;
        }
    }
    //Data (instructions)
    public const string VERSION_INSTRUCTION = "-version";
    public const string REWRITE_INSTRUCTION = "-y";
    public const string INPUT_INSTRUCTION = "-i";
    public const string INDEX_PREFIX_INSTRUCTION = "%";
    public const string INDEX_SUFIX_INSTRUCTION = "d";
    public const string RESIZE_INSTRUCTION = "-r";
    public const string SS_INSTRUCTION = "-ss";
	public const string CODEC_INSTRUCTION = "-codec";
	public const string COPY_INSTRUCTION = "copy";
    public const string TIME_INSTRUCTION = "-t";

    //------------------------------

    public static void GetVersion()
    {
        Wrapper.Execute(new string[] { VERSION_INSTRUCTION });
    }

	//------------------------------

    public static void Convert(BaseData config)
	{
		//-y -i input.mp4 output.mp3
		string[] command =
		{
			REWRITE_INSTRUCTION,
			INPUT_INSTRUCTION,
			config.inputPath,
			config.outputPath
		};

		DebugCommand(command);

		Wrapper.Execute(command);
	}

    //------------------------------

    public static void Trim(TrimData config)
    {
		//-y -i input.mp4 -ss 00:00:50.0 -codec copy -t 20 output.mp4
		string[] command =
		{
			REWRITE_INSTRUCTION,
			INPUT_INSTRUCTION,
            config.inputPath,
            SS_INSTRUCTION,
            config.fromTime,
            CODEC_INSTRUCTION,
            COPY_INSTRUCTION,
            TIME_INSTRUCTION,
            config.durationSec.ToString(),
            config.outputPath
		};

		DebugCommand(command);

		Wrapper.Execute(command);
    }

	//------------------------------

	public static void Decode(FilesData config)
	{
		//-y -i video.mp4 -r 30 .../image%1d.jpg .../track.mp3 
		string[] command =
		{
			REWRITE_INSTRUCTION,
			INPUT_INSTRUCTION,
			config.inputPath,
			RESIZE_INSTRUCTION,
			config.fps.ToString(),
			config.outputPath,
			config.soundPath
		};

		DebugCommand(command);

		Wrapper.Execute(command);
	}

	//------------------------------

	public static void Encode(FilesData config)
	{
		//-y -i .../image%1d.jpg -r 30 -i .../track.mp3 video.mp4
		string[] command =
		{
			REWRITE_INSTRUCTION,
			INPUT_INSTRUCTION,
			config.inputPath,
			RESIZE_INSTRUCTION,
			config.fps.ToString(),
			INPUT_INSTRUCTION,
			config.soundPath,
			config.outputPath
		};

		DebugCommand(command);

		Wrapper.Execute(command);
	}

	//------------------------------

	public static void DirectInput(string input)
	{
		string[] command = input.Split(' ');

        DebugCommand(command);

		Wrapper.Execute(command);
	}

	//------------------------------

	static void DebugCommand(string[] command)
    {
        string debugCommand = "";
        foreach (string instruction in command)
            debugCommand += instruction + " ";
        Debug.Log(debugCommand);
    }
}