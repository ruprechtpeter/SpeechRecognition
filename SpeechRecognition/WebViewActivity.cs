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
using Android.Webkit;
using Java.IO;
using Java.Interop;
using Android.Graphics;
using Android.Content.Res;

namespace SpeechRecognition
{
    [Activity(Label = "WebViewActivity")]
    public class WebViewActivity : Activity
    {
        private WebView wv_webview;
        private string speech;
        private String image;
        private int imageWidth;

        //TODO: Webview image size
        private String html = @"
            <html>
                <body>
                    <div><p>Recognized speech: [SPEECH]</p><p>Captured image:</p></div>
                    <div><img width=""[WIDTH]"" style=""transform: rotate(90deg); -moz-transform: rotate(90deg); -webkit-transform: rotate(90deg);"" alt=""Not available"" class=""center_top_img"" src=""file://[IMAGE]""></div>
                    <div><button type=""button"" onClick=""JSRestart.Restart()"">Restart</button></div>";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.WebView);

            Init();

            wv_webview.LoadDataWithBaseURL("", replaceStringInHtml(), "text/html", "utf-8", "");
        }

        private void Init()
        {
            wv_webview = FindViewById<WebView>(Resource.Id.wv_webview);
            wv_webview.Settings.JavaScriptEnabled = true;
            wv_webview.AddJavascriptInterface(new JSRestartInterface(this), "JSRestart");

            CollectExtras();

            CalculateImageSize();
        }

        private void CollectExtras()
        {
            speech = Intent.GetStringExtra(Consts.BUNDLE_SPEECH) ?? "";
            image = Intent.GetStringExtra(Consts.BUNDLE_IMAGE) ?? "";

        }

        private void CalculateImageSize()
        {
            imageWidth = Resources.DisplayMetrics.WidthPixels / 4;

            //TODO: valahogy az orientation alapján kellene egy max szélességet megadni, és ehhez igazítani a magasságot
        }

        private string replaceStringInHtml()
        {
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