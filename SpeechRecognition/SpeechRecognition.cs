using Android.App;
using Android.Content;
using Android.Widget;
using Android.Speech;
using Android.Util;

namespace SpeechRecognition
{
    public class SpeechRecognition: BaseActivityManager
    {
        private bool isRecording = false;
        public string recognizedText = "";
        private Activity activity;

        public SpeechRecognition(Activity activity)
        {
            this.activity = activity;
        }

        public override void StartActivity()
        {
            recognizedText = "";
            isRecording = !isRecording;
            if (isRecording)
            {
                var voiceIntent = getIntent();
                try
                {
                    activity.StartActivityForResult(voiceIntent, RequestCodeConsts.VOICE_REQUEST);
                }
                catch (ActivityNotFoundException e)
                {
                    Toast.MakeText(activity, Resource.String.noSpeechRecognition, ToastLength.Long).Show();
                    Log.Error(ParamConsts.TAG, e.Message);
                }
            }
        }

        public override void ActivityResult(Result resultVal, Intent data)
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

        private Intent getIntent()
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            intent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            intent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            intent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            intent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            return intent;
        }
    }
}