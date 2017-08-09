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
using Android.Provider;
using Android.Content.PM;

namespace SpeechRecognition
{
    public class CameraAdapter: BaseActivityManager
    {
        private Activity activity;

        public CameraAdapter(Activity activity)
        {
            this.activity = activity;
        }

        public override void StartActivity()
        {
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Intent intent = new Intent(MediaStore.ActionImageCapture);
                CapturedImage._file = new Java.IO.File(CapturedImage._dir, String.Format("Pic_{0}.jpg", Guid.NewGuid()));
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(CapturedImage._file));
                try
                {
                    activity.StartActivityForResult(intent, Consts.PICTURE_REQUEST);
                }
                catch (ActivityNotFoundException e)
                {
                    //TODO: nincs kamera string
                    Toast.MakeText(activity, Resource.String.noSpeechRecognition, ToastLength.Long).Show();
                }
            }
        }

        public override void ActivityResult(Result resultVal, Intent data)
        {
            if (resultVal == Result.Ok) {
                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Android.Net.Uri contentUri = Android.Net.Uri.FromFile(CapturedImage._file);
                mediaScanIntent.SetData(contentUri);
                activity.SendBroadcast(mediaScanIntent);
            }
        }

        private void CreateDirectoryForPictures()
        {
            CapturedImage._dir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "SpeechRecognitionPics");
            if (!CapturedImage._dir.Exists())
            {
                CapturedImage._dir.Mkdir();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = activity.PackageManager.QueryIntentActivities(intent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

    }
}