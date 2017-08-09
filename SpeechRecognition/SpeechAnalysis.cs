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
    public class SpeechAnalysis
    {

        public bool SpeechIsMatchPattern(string speech)
        {
            foreach (string pattern in Consts.patterns)
            {
                if (speech.Contains(pattern))
                {
                    return true;
                }
            }

            return false;
        }
    }
}