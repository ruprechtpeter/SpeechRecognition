using Android.App;
using Android.Content;

namespace SpeechRecognition
{
    public interface BaseActivityManager
    {
        void StartActivity();

        void ActivityResult(Result resultVal, Intent data);
    }
}