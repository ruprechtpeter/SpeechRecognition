namespace SpeechRecognition
{
    public static class IOManager
    {
        public static void CreateDirectoryForPictures()
        {
            CapturedImage._dir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "SpeechRecognitionPics");
            if (!CapturedImage._dir.Exists())
            {
                CapturedImage._dir.Mkdir();
            }
        }

    }
}