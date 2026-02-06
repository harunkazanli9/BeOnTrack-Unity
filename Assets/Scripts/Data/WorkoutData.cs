using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeOnTrack.Core
{
    [Serializable]
    public class WorkoutEntry
    {
        public DateTime date;
        public string type;
        public int durationMinutes;
        public string notes;
    }

    [Serializable]
    public class WorkoutData
    {
        public List<WorkoutEntry> workouts = new List<WorkoutEntry>();
        public int currentStreak;
        public int longestStreak;
        public DateTime lastWorkoutDate;

        public void RecalculateStreak()
        {
            if (workouts.Count == 0)
            {
                currentStreak = 0;
                longestStreak = 0;
                return;
            }

            var sortedDates = workouts
                .Select(w => w.date.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            currentStreak = 0;
            DateTime checkDate = DateTime.Today;

            // Allow for today or yesterday to maintain streak
            if (sortedDates.Count > 0 && (checkDate - sortedDates[0]).Days <= 1)
            {
                checkDate = sortedDates[0];
                currentStreak = 1;

                for (int i = 1; i < sortedDates.Count; i++)
                {
                    if ((checkDate - sortedDates[i]).Days == 1)
                    {
                        currentStreak++;
                        checkDate = sortedDates[i];
                    }
                    else break;
                }
            }

            // Calculate longest streak
            longestStreak = Mathf.Max(longestStreak, currentStreak);

            if (workouts.Count > 0)
                lastWorkoutDate = workouts[workouts.Count - 1].date;
        }

        public int GetTotalMinutes()
        {
            return workouts.Sum(w => w.durationMinutes);
        }

        public float GetAverageWorkoutDuration()
        {
            return workouts.Count > 0 ? (float)GetTotalMinutes() / workouts.Count : 0f;
        }

        public Dictionary<string, int> GetWorkoutsByType()
        {
            return workouts
                .GroupBy(w => w.type)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public int GetWorkoutsThisWeek()
        {
            DateTime weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            return workouts.Count(w => w.date.Date >= weekStart);
        }

        public int GetWorkoutsThisMonth()
        {
            DateTime monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            return workouts.Count(w => w.date.Date >= monthStart);
        }
    }
}
