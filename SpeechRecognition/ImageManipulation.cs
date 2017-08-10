using Android.Media;
using Android.Graphics;
using System.IO;

namespace SpeechRecognition
{
    public static class ImageManipulation
    {
        public static void RotateImage()
        {
            int orientation = GetPictureOrientationFromExif();
            int rotate = GetRotateFromOrientation(orientation);

            if (rotate != 0)
            {
                CapturedImage.bitmap = BitmapFactory.DecodeFile(CapturedImage._file.AbsolutePath.ToString());

                Matrix mtx = new Matrix();
                mtx.PreRotate(rotate);
                CapturedImage.bitmap = Bitmap.CreateBitmap(CapturedImage.bitmap, 0, 0, CapturedImage.bitmap.Width, CapturedImage.bitmap.Height, mtx, false);
                CapturedImage.bitmap = CapturedImage.bitmap.Copy(Bitmap.Config.Argb8888, true);

                FileStream stream = new FileStream(CapturedImage._file.AbsolutePath.ToString(), FileMode.Create);
                CapturedImage.bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                stream.Close();
                CapturedImage.bitmap.Dispose();
                CapturedImage.bitmap = null;
            }
        }

        private static int GetPictureOrientationFromExif()
        {
            ExifInterface exif = new ExifInterface(CapturedImage._file.AbsolutePath.ToString());
            return exif.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);
        }

        private static int GetRotateFromOrientation(int orientation)
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
    }
}