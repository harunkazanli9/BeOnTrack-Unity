using UnityEngine;

namespace BeOnTrack.Core
{
    public class WorkoutService : MonoBehaviour
    {
        private const string SAVE_KEY = "BeOnTrack_WorkoutData";
        private WorkoutData workoutData;

        public void LoadWorkoutData()
        {
            string json = PlayerPrefs.GetString(SAVE_KEY, "");
            if (!string.IsNullOrEmpty(json))
            {
                workoutData = JsonUtility.FromJson<WorkoutData>(json);
            }
            else
            {
                workoutData = new WorkoutData();
                // Add sample data for demo
                AddSampleData();
            }
            workoutData.RecalculateStreak();
        }

        private void AddSampleData()
        {
            string[] types = { "Krafttraining", "Cardio", "HIIT", "Yoga", "Laufen" };
            for (int i = 20; i >= 0; i--)
            {
                if (Random.value > 0.4f) // ~60% chance per day
                {
                    workoutData.workouts.Add(new WorkoutEntry
                    {
                        date = System.DateTime.Today.AddDays(-i),
                        type = types[Random.Range(0, types.Length)],
                        durationMinutes = Random.Range(20, 90),
                        notes = ""
                    });
                }
            }
        }

        public void SaveWorkoutData()
        {
            string json = JsonUtility.ToJson(workoutData);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        public void AddWorkout(WorkoutEntry workout)
        {
            workoutData.workouts.Add(workout);
            workoutData.RecalculateStreak();
            SaveWorkoutData();
        }

        public int GetTotalWorkouts() => workoutData?.workouts?.Count ?? 0;
        public WorkoutData GetWorkoutData() => workoutData;

        public void ClearAllData()
        {
            workoutData = new WorkoutData();
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
        }
    }
}
