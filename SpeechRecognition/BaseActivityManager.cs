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
    public abstract class BaseActivityManager
    {

        public abstract void StartActivity();

        public abstract void ActivityResult(Result resultVal, Intent data);
    }
}