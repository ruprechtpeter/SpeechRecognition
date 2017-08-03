using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SpeechRecognition
{
    class Consts
    {
        public static readonly string TAG = "SpeechRecognition";
        public static readonly string[] patterns = { "camera", "kamera", "photo", "foto", "fotó", "fénykép", "picture" };

        //BUNDLE_CODES
        public static readonly string BUNDLE_SPEECH = "Speech";
        public static readonly string BUNDLE_IMAGE = "Image";

        //REQUEST_CODES
        public static readonly int VOICE_REQUEST = 10;
        public static readonly int PICTURE_REQUEST = 20;
        public static readonly int WEBVIEW_REQUEST = 30;

    }
}