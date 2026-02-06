using UnityEngine;

namespace BeOnTrack.Core
{
    /// <summary>
    /// Attach this to an empty GameObject in your scene.
    /// It automatically creates and wires all BeOnTrack systems.
    /// Just press Play!
    /// </summary>
    public class SceneSetup : MonoBehaviour
    {
        [Header("Auto-setup on Start")]
        public bool autoSetup = true;

        private void Start()
        {
            if (autoSetup)
                SetupScene();
        }

        public void SetupScene()
        {
            // 1. Camera setup
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 7f;
            Camera.main.backgroundColor = new Color(0.08f, 0.1f, 0.16f);
            Camera.main.transform.position = new Vector3(0, 2, -10);

            // 2. GameManager
            var gmObj = new GameObject("GameManager");
            var gm = gmObj.AddComponent<GameManager>();

            // 3. Path System
            var pathObj = new GameObject("PathSystem");
            var pathSystem = pathObj.AddComponent<PathSystem>();
            pathSystem.pathColorStart = new Color(0.3f, 0.9f, 0.5f);  // Green start
            pathSystem.pathColorEnd = new Color(0f, 0.83f, 1f);       // Cyan end
            pathSystem.curveAmplitude = 2.5f;
            pathSystem.pathWidth = 0.5f;
            gm.pathSystem = pathSystem;

            // 4. Avatar
            var avatarObj = new GameObject("Avatar");
            var avatar = avatarObj.AddComponent<AvatarController>();
            avatar.pathSystem = pathSystem;
            gm.avatarController = avatar;

            // 5. Milestone Manager
            var milestoneObj = new GameObject("MilestoneManager");
            var milestones = milestoneObj.AddComponent<MilestoneManager>();
            gm.milestoneManager = milestones;

            // 6. Journey Timeline (Camera Controller)
            var timelineObj = new GameObject("JourneyTimeline");
            var timeline = timelineObj.AddComponent<JourneyTimeline>();
            timeline.mainCamera = Camera.main;
            timeline.pathSystem = pathSystem;
            timeline.avatarController = avatar;
            gm.journeyTimeline = timeline;

            // 7. Workout Service
            var workoutObj = new GameObject("WorkoutService");
            var workoutService = workoutObj.AddComponent<WorkoutService>();
            gm.workoutService = workoutService;

            // 8. UI
            var uiObj = new GameObject("GameUI");
            var gameUI = uiObj.AddComponent<GameUI>();
            gm.gameUI = gameUI;

            // 9. Screenshot Manager
            var screenshotObj = new GameObject("ScreenshotManager");
            var screenshotMgr = screenshotObj.AddComponent<ScreenshotManager>();
            gameUI.screenshotManager = screenshotMgr;

            // 10. Visual Effects
            var bgObj = new GameObject("Background");
            bgObj.AddComponent<BackgroundParallax>();

            var particlesObj = new GameObject("FloatingParticles");
            particlesObj.AddComponent<FloatingParticles>();

            Debug.Log("=== BeOnTrack Scene Setup Complete! ===");
            Debug.Log("Press the '+ WORKOUT' button to simulate workouts.");
            Debug.Log("Scroll/drag to explore your journey.");
        }
    }
}
