namespace FFmpeg
{
	[System.Serializable]
	public class BaseData
	{
		public string inputPath;
		public string outputPath;
	}

    [System.Serializable]
    public class FilesData : BaseData
    {
        public string soundPath;
        public float fps;
    }

	[System.Serializable]
	public class TrimData : BaseData
	{
        public string fromTime; //"00:00:01.0" - after first second
		public int durationSec;
	}
}
