using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.Provider;
using Android.Util;

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
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            CapturedImage._file = new Java.IO.File(CapturedImage._dir, String.Format("Pic_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(CapturedImage._file));
            try
            {
                activity.StartActivityForResult(intent, RequestCodeConsts.PICTURE_REQUEST);
            }
            catch (ActivityNotFoundException e)
            {
                Toast.MakeText(activity, Resource.String.no_camera, ToastLength.Long).Show();
                Log.Error(ParamConsts.TAG, e.Message);
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

    }
}