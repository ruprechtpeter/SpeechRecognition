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
using Android.Speech;
using System.Threading.Tasks;
using System.Threading;
using static Java.Interop.JniEnvironment;
using System.Resources;

namespace SpeechRecognition
{
    public class SpeechRecognition
    {
        private bool isRecording = false;
        public string recognizedText = "";
        private Activity activity;

        public SpeechRecognition(Activity activity)
        {
            this.activity = activity;
        }

        public void StartSpeechRecognition()
        {
            isRecording = !isRecording;
            if (isRecording)
            {
                var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                try
                {
                    activity.StartActivityForResult(voiceIntent, Consts.VOICE_REQUEST);
                }
                catch (ActivityNotFoundException e)
                {
                    Toast.MakeText(activity, Resource.String.noSpeechRecognition, ToastLength.Long).Show();
                }
            }
        }

        public void ActivityResult(int requestCode, Result resultVal, Intent data)
        {
            if (resultVal == Result.Ok)
            {
                var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                if (matches.Count != 0)
                {
                    recognizedText = matches[0];

                    if (recognizedText.Length > 500)
                        recognizedText = recognizedText.Substring(0, 500);
                }
                else
                {
                    recognizedText = activity.Resources.GetString(Resource.String.no_speech_was_recognised);
                }

                isRecording = !isRecording;
            }
        }
    }
}