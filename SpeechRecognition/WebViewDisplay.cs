using Android.App;
using Android.Content;

namespace SpeechRecognition
{
    public class WebViewDisplay: BaseActivityManager
    {
        private Activity activity;
        private string recognizedText;

        public WebViewDisplay(Activity activity)
        {
            this.activity = activity;
        }

        public void StartActivity(string recognizedText)
        {
            this.recognizedText = recognizedText;
            StartActivity();
        }
        public override void StartActivity()
        {
            Intent intent = new Intent(activity, typeof(WebViewActivity));
            intent.PutExtra(Consts.BUNDLE_SPEECH, recognizedText);
            intent.PutExtra(Consts.BUNDLE_IMAGE, CapturedImage._file.AbsolutePath.ToString());
            activity.StartActivityForResult(intent, Consts.WEBVIEW_REQUEST);
        }

        public override void ActivityResult(Result resultVal, Intent data)
        {
        }

    }
}