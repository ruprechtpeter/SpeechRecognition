using Android.App;
using Android.Content;

namespace SpeechRecognition
{
    public abstract class BaseActivityManager
    {
        public abstract void StartActivity();

        public abstract void ActivityResult(Result resultVal, Intent data);
    }
}