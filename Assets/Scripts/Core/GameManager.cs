using UnityEngine;

namespace BeOnTrack.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        public PathSystem pathSystem;
        public AvatarController avatarController;
        public MilestoneManager milestoneManager;
        public JourneyTimeline journeyTimeline;
        public GameUI gameUI;
        public WorkoutService workoutService;

        [Header("Settings")]
        public float stepsPerWorkout = 50f;
        public float avatarMoveSpeed = 3f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            workoutService.LoadWorkoutData();
            int totalWorkouts = workoutService.GetTotalWorkouts();
            float totalDistance = totalWorkouts * stepsPerWorkout;

            pathSystem.GeneratePath(totalDistance);
            milestoneManager.PlaceMilestones(pathSystem);
            avatarController.SetPosition(pathSystem.GetPointAtDistance(totalDistance));
            gameUI.UpdateStats(workoutService.GetWorkoutData());
        }

        public void OnWorkoutCompleted(WorkoutEntry workout)
        {
            workoutService.AddWorkout(workout);
            float newDistance = workoutService.GetTotalWorkouts() * stepsPerWorkout;

            pathSystem.ExtendPath(newDistance);
            avatarController.MoveToDistance(newDistance, avatarMoveSpeed);
            milestoneManager.CheckMilestones(workoutService.GetTotalWorkouts());
            gameUI.UpdateStats(workoutService.GetWorkoutData());
        }

        public void SimulateWorkout()
        {
            var workout = new WorkoutEntry
            {
                date = System.DateTime.Now,
                type = "Krafttraining",
                durationMinutes = Random.Range(30, 90),
                notes = "Simuliertes Workout"
            };
            OnWorkoutCompleted(workout);
        }
    }
}
