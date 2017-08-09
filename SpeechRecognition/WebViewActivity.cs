using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;
using Java.Interop;
using Android.Content.Res;

namespace SpeechRecognition
{
    [Activity(Label = "Speech recognition - WebView")]
    public class WebViewActivity : Activity
    {
        private WebView wv_webview;
        private string speech;
        private String image;
        private String html = @"
            <html>
                <body>
                    <div><button type=""button"" onClick=""JSRestart.Restart()"">Restart</button></div>
                    <div><p>Recognized speech: [SPEECH]</p></div><div><p>Captured image:</p></div>
                    <div><img width = ""[WIDTH]"" alt=""Not available"" class=""center_top_img"" src=""file://[IMAGE]""></div>
                </body>
            </html>";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.WebView);

            Init();

            LoadPage();
        }

        private void Init()
        {
            wv_webview = FindViewById<WebView>(Resource.Id.wv_webview);
            wv_webview.Settings.JavaScriptEnabled = true;
            wv_webview.AddJavascriptInterface(new JSRestartInterface(this), "JSRestart");

            CollectExtras();
        }

        private void CollectExtras()
        {
            speech = Intent.GetStringExtra(Consts.BUNDLE_SPEECH) ?? "";
            image = Intent.GetStringExtra(Consts.BUNDLE_IMAGE) ?? "";
        }

        private void LoadPage()
        {
            string replacedHtml = ReplaceStringInHtml();
            wv_webview.LoadDataWithBaseURL("", replacedHtml, "text/html", "utf-8", "");
        }

        private string ReplaceStringInHtml()
        {
            int imageWidth = Resources.DisplayMetrics.WidthPixels / 4;
            StringBuilder builder = new StringBuilder(html);
            builder.Replace("[SPEECH]", speech);
            builder.Replace("[IMAGE]", image);
            builder.Replace("[WIDTH]", imageWidth.ToString());
            return builder.ToString();
        }
    }

    class JSRestartInterface : Java.Lang.Object
    {
        Context context;

        public JSRestartInterface(Context context)
        {
            this.context = context;
        }

        [Export]
        [JavascriptInterface]
        public void Restart()
        {
            if (context is Activity)
            {
                Activity activity = (Activity)context;
                activity.Finish();
            }
        }
    }
}