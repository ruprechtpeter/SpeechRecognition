using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;
using Android.Speech;

namespace SpeechRecognition
{
    [Activity(Label = "SpeechRecognition", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private bool isRecording = false;
        private readonly int VOICE = 10;
        private TextView tv_text;
        private Button btn_speech;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            Init();
        }

        private void Init()
        {
            tv_text = FindViewById<TextView>(Resource.Id.tv_text);
            btn_speech = FindViewById<Button>(Resource.Id.btn_speech);

            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;

            if (rec != "android.hardware.microphone")
            {
                tv_text.Text = GetString(Resource.String.no_microphone);
                btn_speech.Enabled = false;
            }
            else
            {
                tv_text.Text = GetString(Resource.String.please_speak);
                btn_speech.Enabled = true;
            }

            btn_speech.Click += Btn_speech_Click;
        }

        private void Btn_speech_Click(object sender, System.EventArgs e)
        {
            btn_speech.Text = GetString(Resource.String.end_recording);
            isRecording = !isRecording;
            if (isRecording)
            {
                var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.messageSpeakNow));
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                StartActivityForResult(voiceIntent, VOICE);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            if (requestCode == VOICE)
            {
                if (resultVal == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        string textInput = matches[0];

                        if (textInput.Length > 500)
                            textInput = textInput.Substring(0, 500);

                        tv_text.Text = textInput;

                        if (CheckPatterns(textInput.ToLower()))
                        {
                            StartCamera();
                        }
                    }
                    else
                        tv_text.Text = GetString(Resource.String.no_speech_was_recognised);

                    btn_speech.Text = GetString(Resource.String.start_recording);
                    isRecording = !isRecording;
                }
            }

            base.OnActivityResult(requestCode, resultVal, data);
        }

        private bool CheckPatterns(string text)
        {
            foreach (string pattern in Consts.patterns)
            {
                if (text.Contains(pattern))
                {
                    return true;
                }
            }

            return false;
        }

        private void StartCamera()
        {
            Toast.MakeText(this, "Start Camera", ToastLength.Long).Show();
        }
    }
}

