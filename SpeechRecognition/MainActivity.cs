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
        private bool isRecording = false;
        private readonly int VOICE = 10;
        private readonly int PICTURE = 20;
        private TextView tv_text;
        private Button btn_speech;
        private ImageView iv_image;

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
            iv_image = FindViewById<ImageView>(Resource.Id.iv_image);

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
            base.OnActivityResult(requestCode, resultVal, data);

            if (requestCode == VOICE)
            {
                VoiceActivityResult(resultVal, data);
            }

            if (requestCode == PICTURE)
            {
                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);

                int height = Resources.DisplayMetrics.HeightPixels;
                int width = iv_image.Height;
                App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
                if (App.bitmap != null)
                {
                    iv_image.SetImageBitmap(App.bitmap);
                    App.bitmap = null;
                }

                GC.Collect();
            }
        }
        
        private void PictureActivityResult(Result resultVal, Intent data)
        {
            throw new NotImplementedException();
        }

        private void VoiceActivityResult(Result resultVal, Intent data)
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

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Intent intent = new Intent(MediaStore.ActionImageCapture);
                App._file = new File(App._dir, String.Format("Pic_Speech_{0}.jpg", Guid.NewGuid()));
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
                StartActivityForResult(intent, PICTURE);
            }
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "SpeechRecognitionPics");
            if (!App._dir.Exists())
            {
                App._dir.Mkdir();
                Log.Debug(Consts.TAG, "Directory created: " + App._dir.ToString());
            } else
            {
                Log.Debug(Consts.TAG, "Directory exists: " + App._dir.ToString());
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
    }

    public static class BitmapHelpers
    {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }
    }
}

