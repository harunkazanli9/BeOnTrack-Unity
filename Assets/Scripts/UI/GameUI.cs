using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace BeOnTrack.Core
{
    public class GameUI : MonoBehaviour
    {
        [Header("Stats Panel")]
        public Text workoutCountText;
        public Text streakText;
        public Text totalDistanceText;
        public Text lastWorkoutText;

        [Header("Milestone Popup")]
        public GameObject milestonePopupPanel;
        public Text milestoneTitle;
        public Text milestoneEmoji;
        public Image milestoneBackground;

        [Header("Bottom Bar")]
        public Button addWorkoutButton;
        public Button scrollToStartButton;
        public Button scrollToEndButton;
        public Button focusAvatarButton;
        public Button screenshotButton;

        [Header("Screenshot")]
        public ScreenshotManager screenshotManager;

        private Canvas canvas;

        private void Start()
        {
            if (canvas == null)
                CreateUI();

            SetupButtons();
        }

        private void CreateUI()
        {
            // Create Canvas
            var canvasObj = new GameObject("BeOnTrackCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
            canvasObj.AddComponent<GraphicRaycaster>();

            // Top Stats Panel
            var topPanel = CreatePanel(canvasObj.transform, "TopPanel",
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -20), new Vector2(0, 140));
            topPanel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.18f, 0.85f);

            workoutCountText = CreateText(topPanel.transform, "WorkoutCount", "0 Workouts",
                new Vector2(0.02f, 0.5f), new Vector2(0.3f, 1f), 28, TextAnchor.MiddleLeft, Color.white);

            streakText = CreateText(topPanel.transform, "Streak", "🔥 0 Tage",
                new Vector2(0.35f, 0.5f), new Vector2(0.65f, 1f), 24, TextAnchor.MiddleCenter, new Color(1f, 0.6f, 0.2f));

            totalDistanceText = CreateText(topPanel.transform, "Distance", "0 Schritte",
                new Vector2(0.7f, 0.5f), new Vector2(0.98f, 1f), 20, TextAnchor.MiddleRight, new Color(0.7f, 0.7f, 0.8f));

            // Bottom Button Bar
            var bottomPanel = CreatePanel(canvasObj.transform, "BottomPanel",
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 20), new Vector2(0, 180));
            bottomPanel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.18f, 0.9f);

            // Add Workout Button (center, large)
            addWorkoutButton = CreateButton(bottomPanel.transform, "AddWorkout", "+ WORKOUT",
                new Vector2(0.25f, 0.2f), new Vector2(0.75f, 0.8f),
                new Color(0.2f, 0.8f, 0.4f), Color.white, 24);

            // Small navigation buttons
            scrollToStartButton = CreateButton(bottomPanel.transform, "ToStart", "◀ Start",
                new Vector2(0.02f, 0.3f), new Vector2(0.22f, 0.7f),
                new Color(0.2f, 0.2f, 0.3f), new Color(0.7f, 0.7f, 0.8f), 14);

            focusAvatarButton = CreateButton(bottomPanel.transform, "Focus", "📍",
                new Vector2(0.78f, 0.3f), new Vector2(0.88f, 0.7f),
                new Color(0.2f, 0.2f, 0.3f), Color.white, 18);

            screenshotButton = CreateButton(bottomPanel.transform, "Screenshot", "📷",
                new Vector2(0.89f, 0.3f), new Vector2(0.98f, 0.7f),
                new Color(0f, 0.6f, 0.9f), Color.white, 18);

            // Milestone Popup (hidden by default)
            milestonePopupPanel = CreatePanel(canvasObj.transform, "MilestonePopup",
                new Vector2(0.1f, 0.35f), new Vector2(0.9f, 0.65f), Vector2.zero, Vector2.zero);
            milestonePopupPanel.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.25f, 0.95f);

            milestoneEmoji = CreateText(milestonePopupPanel.transform, "Emoji", "🏆",
                new Vector2(0.1f, 0.5f), new Vector2(0.3f, 0.9f), 48, TextAnchor.MiddleCenter, Color.white);

            milestoneTitle = CreateText(milestonePopupPanel.transform, "Title", "Meilenstein!",
                new Vector2(0.3f, 0.3f), new Vector2(0.9f, 0.8f), 28, TextAnchor.MiddleCenter, Color.white);

            milestonePopupPanel.SetActive(false);
        }

        private GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var img = obj.AddComponent<Image>();
            var rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            return obj;
        }

        private Text CreateText(Transform parent, string name, string content, Vector2 anchorMin, Vector2 anchorMax, int fontSize, TextAnchor anchor, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var text = obj.AddComponent<Text>();
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.color = color;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontStyle = FontStyle.Bold;
            var rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return text;
        }

        private Button CreateButton(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax, Color bgColor, Color textColor, int fontSize)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var img = obj.AddComponent<Image>();
            img.color = bgColor;
            var btn = obj.AddComponent<Button>();
            var rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            // Round corners via outline
            var outline = obj.AddComponent<Outline>();
            outline.effectColor = new Color(1, 1, 1, 0.1f);
            outline.effectDistance = new Vector2(1, 1);

            // Label
            var textObj = new GameObject("Label");
            textObj.transform.SetParent(obj.transform, false);
            var text = textObj.AddComponent<Text>();
            text.text = label;
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = textColor;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontStyle = FontStyle.Bold;
            var textRt = textObj.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            return btn;
        }

        private void SetupButtons()
        {
            if (addWorkoutButton != null)
                addWorkoutButton.onClick.AddListener(() => GameManager.Instance?.SimulateWorkout());

            if (focusAvatarButton != null)
                focusAvatarButton.onClick.AddListener(() => GameManager.Instance?.journeyTimeline?.FocusOnAvatar());

            if (scrollToStartButton != null)
                scrollToStartButton.onClick.AddListener(() => GameManager.Instance?.journeyTimeline?.ScrollToStart());

            if (screenshotButton != null && screenshotManager != null)
                screenshotButton.onClick.AddListener(() => screenshotManager.TakeScreenshot());
        }

        public void UpdateStats(WorkoutData data)
        {
            if (workoutCountText != null)
                workoutCountText.text = $"{data.workouts.Count} Workouts";

            if (streakText != null)
                streakText.text = $"🔥 {data.currentStreak} Tage";

            if (totalDistanceText != null)
                totalDistanceText.text = $"{data.workouts.Count * 50} Schritte";
        }

        public void ShowMilestonePopup(string title, string emoji, Color color)
        {
            if (milestonePopupPanel == null) return;

            milestonePopupPanel.SetActive(true);
            milestoneTitle.text = title;
            milestoneEmoji.text = emoji;
            milestoneBackground.color = new Color(color.r * 0.3f, color.g * 0.3f, color.b * 0.3f, 0.95f);

            StartCoroutine(HideMilestonePopup());
        }

        private IEnumerator HideMilestonePopup()
        {
            yield return new WaitForSeconds(3f);
            milestonePopupPanel.SetActive(false);
        }
    }
}
