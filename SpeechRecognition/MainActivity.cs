using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;
using Android.Speech;
using Java.IO;
using Android.Graphics;
using Android.Provider;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Util;

namespace SpeechRecognition
{
    public static class App
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }

    [Activity(Label = "SpeechRecognition", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private static readonly string STATE_SPEECH = "StateSpeech";

        private bool isRecording = false;
        private TextView tv_text;
        private string recognizedText = "";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            Init();

            StartSpeechrecognition();
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

            CheckMicrophone();
        }

        private void CheckMicrophone()
        {
            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;

            if (rec != "android.hardware.microphone")
            {
                tv_text.Text = GetString(Resource.String.no_microphone);
            }
            else
            {
                tv_text.Text = GetString(Resource.String.please_speak);
            }
        }

        private void StartSpeechrecognition()
        {
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
                StartActivityForResult(voiceIntent, Consts.VOICE_REQUEST);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            base.OnActivityResult(requestCode, resultVal, data);

            if (requestCode == Consts.VOICE_REQUEST)
            {
                VoiceActivityResult(resultVal, data);
            }

            if (requestCode == Consts.PICTURE_REQUEST)
            {
                CameraActivityResult(resultVal, data);
            }

            if (requestCode == Consts.WEBVIEW_REQUEST)
            {
                StartSpeechrecognition();
            }
        }

        private void VoiceActivityResult(Result resultVal, Intent data)
        {
            if (resultVal == Result.Ok)
            {
                var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                if (matches.Count != 0)
                {
                    recognizedText = matches[0];

                    if (recognizedText.Length > 500)
                        recognizedText = recognizedText.Substring(0, 500);

                    tv_text.Text = recognizedText;

                    if (CheckPatterns(recognizedText.ToLower()))
                    {
                        StartCamera();
                    }
                }
                else
                    tv_text.Text = GetString(Resource.String.no_speech_was_recognised);

                isRecording = !isRecording;
            }
        }

        private void StartCamera()
        {
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Intent intent = new Intent(MediaStore.ActionImageCapture);
                App._file = new File(App._dir, String.Format("Pic_Speech_{0}.jpg", Guid.NewGuid()));
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
                StartActivityForResult(intent, Consts.PICTURE_REQUEST);
            }
        }

        private void CameraActivityResult(Result resultVal, Intent data)
        {
            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            GC.Collect();

            StartWebViewActivity();
        }

        private void StartWebViewActivity()
        {
            Intent intent = new Intent(this, typeof(WebViewActivity));
            intent.PutExtra(Consts.BUNDLE_SPEECH, tv_text.Text);
            intent.PutExtra(Consts.BUNDLE_IMAGE, App._file.AbsolutePath.ToString());

            StartActivityForResult(intent, Consts.WEBVIEW_REQUEST);
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

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "SpeechRecognitionPics");
            if (!App._dir.Exists())
            {
                App._dir.Mkdir();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
    }

}

