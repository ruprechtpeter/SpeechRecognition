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
using Android.Content.Res;
using Android.Media;
using System.IO;

namespace SpeechRecognition
{
    public static class App
    {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Bitmap bitmap;
    }

    [Activity(Label = "Speech Recognition", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private static readonly string STATE_SPEECH = "StateSpeech";

        private bool isRecording = false;
        private TextView tv_text;
        private string recognizedText = "";
        private SpeechRecognition speechRecognition;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            Init();

            speechRecognition.StartSpeechRecognition();
            //StartSpeechrecognition();
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

            speechRecognition = new SpeechRecognition(this);
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
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                try
                {
                    StartActivityForResult(voiceIntent, Consts.VOICE_REQUEST);
                } catch (ActivityNotFoundException e)
                {
                    Toast.MakeText(this, Resource.String.noSpeechRecognition, ToastLength.Long).Show();
                    tv_text.Text = GetString(Resource.String.noSpeechRecognition);
                }
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            base.OnActivityResult(requestCode, resultVal, data);

            if (requestCode == Consts.VOICE_REQUEST)
            {
                VoiceActivityResult(requestCode, resultVal, data);
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

        private void VoiceActivityResult(int requestCode, Result resultVal, Intent data)
        {
            speechRecognition.ActivityResult(requestCode, resultVal, data);
            tv_text.Text = speechRecognition.recognizedText;

            SpeechAnalysis analysis = new SpeechAnalysis();
            if (analysis.SpeechIsMatchPattern(speechRecognition.recognizedText.ToLower()))
            {
                CameraAdapter camera = new CameraAdapter(this);
                camera.StartCamera();
            }
        }

        private void StartCamera()
        {
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Intent intent = new Intent(MediaStore.ActionImageCapture);
                App._file = new Java.IO.File(App._dir, String.Format("Pic_{0}.jpg", Guid.NewGuid()));
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

            RotateImage();

            GC.Collect();

            StartWebViewActivity();
        }

        private void RotateImage()
        {
            ExifInterface exif = new ExifInterface(App._file.AbsolutePath.ToString());
            int orientation = exif.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);
            int rotate = GetRotateFromOrientation(orientation);

            if (rotate != 0)
            {
                App.bitmap = BitmapFactory.DecodeFile(App._file.AbsolutePath.ToString());

                Matrix mtx = new Matrix();
                mtx.PreRotate(rotate);
                App.bitmap = Bitmap.CreateBitmap(App.bitmap, 0, 0, App.bitmap.Width, App.bitmap.Height, mtx, false);
                App.bitmap = App.bitmap.Copy(Bitmap.Config.Argb8888, true);

                FileStream stream = new FileStream(App._file.AbsolutePath.ToString(), FileMode.Create);
                App.bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                stream.Close();
                App.bitmap.Dispose();
                App.bitmap = null;
            }
        }

        private int GetRotateFromOrientation(int orientation)
        {
            int rotate = 0;
            switch (orientation)
            {
                case (int)Android.Media.Orientation.Rotate90: rotate = 90; break;
                case (int)Android.Media.Orientation.Rotate180: rotate = 180; break;
                case (int)Android.Media.Orientation.Rotate270: rotate = 270; break;
            }

            return rotate;
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
            App._dir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "SpeechRecognitionPics");
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

