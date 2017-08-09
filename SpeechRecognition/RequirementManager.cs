using System.Collections.Generic;
using Android.Content;
using Android.Provider;
using Android.Content.PM;

namespace SpeechRecognition
{
    public static class RequirementManager
    {
        public static bool HasAllRequirements()
        {
            return (HasMicrophone() && IsThereAnAppToTakePictures());
        }

        private static bool HasMicrophone()
        {
            return Android.Content.PM.PackageManager.FeatureMicrophone != "android.hardware.microphone";
        }

        private static bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = Android.App.Application.Context.PackageManager.QueryIntentActivities(intent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
    }
}