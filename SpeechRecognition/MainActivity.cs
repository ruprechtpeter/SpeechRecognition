using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;

namespace SpeechRecognition
{
    [Activity(Label = "Speech Recognition", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private static readonly string STATE_SPEECH = "StateSpeech";

        private TextView tv_text;
        private SpeechRecognition speechRecognition;
        private CameraAdapter cameraAdapter;
        private WebViewDisplay webViewDisplay;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            Init();

            if (RequirementManager.HasAllRequirements())
            {
                speechRecognition.StartActivity();
            }
            else
            {
                Toast.MakeText(this, Resource.String.no_all_requirement, ToastLength.Long).Show();
                tv_text.Text = GetString(Resource.String.no_all_requirement);
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString(STATE_SPEECH, tv_text.Text);
            base.OnSaveInstanceState(outState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);

            if (savedInstanceState != null)
            {
                tv_text.Text = savedInstanceState.GetString(STATE_SPEECH, GetString(Resource.String.please_speak));
            }
        }

        private void Init()
        {
            tv_text = FindViewById<TextView>(Resource.Id.tv_text);

            speechRecognition = new SpeechRecognition(this);
            cameraAdapter = new CameraAdapter(this);
            webViewDisplay = new WebViewDisplay(this);

            IOManager.CreateDirectoryForPictures();
        }

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            base.OnActivityResult(requestCode, resultVal, data);

            if (requestCode == RequestCodeConsts.VOICE_REQUEST)
            {
                VoiceActivityResult(resultVal, data);
            }

            if (requestCode == RequestCodeConsts.PICTURE_REQUEST)
            {
                CameraActivityResult(resultVal, data);
            }

            if (requestCode == RequestCodeConsts.WEBVIEW_REQUEST)
            {
                speechRecognition.StartActivity();
            }
        }

        private void VoiceActivityResult(Result resultVal, Intent data)
        {
            speechRecognition.ActivityResult(resultVal, data);
            tv_text.Text = speechRecognition.recognizedText;

            SpeechAnalysis analysis = new SpeechAnalysis();
            if (analysis.SpeechIsMatchPattern(speechRecognition.recognizedText.ToLower()))
            {
                cameraAdapter.StartActivity();
            }
        }

        private void CameraActivityResult(Result resultVal, Intent data)
        {
            cameraAdapter.ActivityResult(resultVal, data);

            if (resultVal == Result.Ok)
            {
                ImageManipulation.RotateImage();

                GC.Collect();

                webViewDisplay.StartActivity(speechRecognition.recognizedText);
            }
        }
    }

}

