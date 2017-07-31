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
    }
}