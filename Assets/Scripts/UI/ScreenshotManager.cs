using UnityEngine;
using System.IO;

namespace BeOnTrack.Core
{
    public class ScreenshotManager : MonoBehaviour
    {
        [Header("Screenshot Settings")]
        public int screenshotWidth = 1080;
        public int screenshotHeight = 1920;
        public string screenshotFolder = "BeOnTrack_Screenshots";

        [Header("Share Overlay")]
        public bool addOverlay = true;
        public Color overlayColor = new Color(0.1f, 0.1f, 0.18f, 0.7f);

        public void TakeScreenshot()
        {
            StartCoroutine(CaptureScreenshot());
        }

        private System.Collections.IEnumerator CaptureScreenshot()
        {
            yield return new WaitForEndOfFrame();

            // Capture the screen
            RenderTexture rt = new RenderTexture(screenshotWidth, screenshotHeight, 24);
            Camera.main.targetTexture = rt;
            Camera.main.Render();

            RenderTexture.active = rt;
            Texture2D screenshot = new Texture2D(screenshotWidth, screenshotHeight, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, screenshotWidth, screenshotHeight), 0, 0);

            if (addOverlay)
                AddShareOverlay(screenshot);

            screenshot.Apply();

            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            // Save to file
            byte[] bytes = screenshot.EncodeToPNG();
            string folderPath = Path.Combine(Application.persistentDataPath, screenshotFolder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filePath = Path.Combine(folderPath, $"BeOnTrack_{timestamp}.png");
            File.WriteAllBytes(filePath, bytes);

            Debug.Log($"Screenshot saved: {filePath}");

            Destroy(screenshot);

            // Native share on mobile
            #if UNITY_ANDROID || UNITY_IOS
            ShareScreenshot(filePath);
            #endif
        }

        private void AddShareOverlay(Texture2D tex)
        {
            int barHeight = screenshotHeight / 8;

            // Bottom bar
            for (int x = 0; x < screenshotWidth; x++)
            {
                for (int y = 0; y < barHeight; y++)
                {
                    Color existing = tex.GetPixel(x, y);
                    Color blended = Color.Lerp(existing, overlayColor, overlayColor.a);
                    tex.SetPixel(x, y, blended);
                }
            }

            // Top bar
            for (int x = 0; x < screenshotWidth; x++)
            {
                for (int y = screenshotHeight - barHeight; y < screenshotHeight; y++)
                {
                    Color existing = tex.GetPixel(x, y);
                    Color blended = Color.Lerp(existing, overlayColor, overlayColor.a);
                    tex.SetPixel(x, y, blended);
                }
            }

            // Accent line at bottom of top bar
            Color accentColor = new Color(0f, 0.83f, 1f, 1f);
            for (int x = 0; x < screenshotWidth; x++)
            {
                for (int y = screenshotHeight - barHeight; y < screenshotHeight - barHeight + 4; y++)
                {
                    tex.SetPixel(x, y, accentColor);
                }
            }
        }

        private void ShareScreenshot(string filePath)
        {
            #if UNITY_ANDROID
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
            intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            intent.Call<AndroidJavaObject>("setType", "image/png");

            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + filePath);
            intent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uri);

            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("startActivity", intent);
            #endif
        }
    }
}
