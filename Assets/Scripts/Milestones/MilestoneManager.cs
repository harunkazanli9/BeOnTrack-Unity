using UnityEngine;
using System.Collections.Generic;

namespace BeOnTrack.Core
{
    [System.Serializable]
    public class MilestoneDefinition
    {
        public int workoutCount;
        public string title;
        public string emoji;
        public Color color;
    }

    public class MilestoneManager : MonoBehaviour
    {
        [Header("Milestone Definitions")]
        public List<MilestoneDefinition> milestones = new List<MilestoneDefinition>
        {
            new MilestoneDefinition { workoutCount = 1, title = "Erster Schritt!", emoji = "🎯", color = new Color(0.2f, 0.8f, 0.4f) },
            new MilestoneDefinition { workoutCount = 5, title = "Erste Woche geschafft!", emoji = "⭐", color = new Color(1f, 0.84f, 0f) },
            new MilestoneDefinition { workoutCount = 10, title = "Zweistellig!", emoji = "🔥", color = new Color(1f, 0.4f, 0.2f) },
            new MilestoneDefinition { workoutCount = 25, title = "Ein Monat dran!", emoji = "💪", color = new Color(0.6f, 0.2f, 1f) },
            new MilestoneDefinition { workoutCount = 50, title = "Halbzeit-Held!", emoji = "🏆", color = new Color(0f, 0.83f, 1f) },
            new MilestoneDefinition { workoutCount = 75, title = "Unaufhaltsam!", emoji = "⚡", color = new Color(1f, 0.6f, 0f) },
            new MilestoneDefinition { workoutCount = 100, title = "Legende!", emoji = "👑", color = new Color(1f, 0.84f, 0f) },
            new MilestoneDefinition { workoutCount = 150, title = "Unsterblich!", emoji = "🌟", color = new Color(1f, 0.2f, 0.6f) },
            new MilestoneDefinition { workoutCount = 200, title = "Zweihundert!", emoji = "💎", color = new Color(0f, 1f, 0.8f) },
            new MilestoneDefinition { workoutCount = 365, title = "Ein ganzes Jahr!", emoji = "🎉", color = Color.white }
        };

        [Header("Visual Settings")]
        public float flagHeight = 2f;
        public float flagWidth = 1.5f;
        public float poleWidth = 0.08f;

        [Header("Celebration")]
        public float celebrationDuration = 2.5f;
        public int confettiCount = 30;

        private Dictionary<int, GameObject> placedMilestones = new Dictionary<int, GameObject>();
        private HashSet<int> celebratedMilestones = new HashSet<int>();

        public void PlaceMilestones(PathSystem pathSystem)
        {
            foreach (var milestone in milestones)
            {
                float distance = milestone.workoutCount * GameManager.Instance.stepsPerWorkout;
                Vector3 position = pathSystem.GetPointAtDistance(distance);
                CreateMilestoneFlag(milestone, position);
            }
        }

        private void CreateMilestoneFlag(MilestoneDefinition def, Vector3 position)
        {
            var flagParent = new GameObject($"Milestone_{def.workoutCount}");
            flagParent.transform.SetParent(transform);
            flagParent.transform.position = position;

            // Flag pole
            var pole = new GameObject("Pole");
            pole.transform.SetParent(flagParent.transform);
            var poleSr = pole.AddComponent<SpriteRenderer>();
            poleSr.sprite = CreateRectSprite(4, 32);
            poleSr.color = new Color(0.8f, 0.8f, 0.8f, 0.6f);
            poleSr.sortingOrder = 5;
            pole.transform.localPosition = new Vector3(0, flagHeight * 0.5f, 0);
            pole.transform.localScale = new Vector3(poleWidth, flagHeight, 1f);

            // Flag banner
            var flag = new GameObject("Flag");
            flag.transform.SetParent(flagParent.transform);
            var flagSr = flag.AddComponent<SpriteRenderer>();
            flagSr.sprite = CreateRectSprite(16, 12);
            flagSr.color = def.color;
            flagSr.sortingOrder = 6;
            flag.transform.localPosition = new Vector3(flagWidth * 0.4f, flagHeight * 0.8f, 0);
            flag.transform.localScale = new Vector3(flagWidth, flagHeight * 0.3f, 1f);

            // Workout count text
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(flagParent.transform);
            var textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = $"{def.workoutCount}";
            textMesh.fontSize = 24;
            textMesh.characterSize = 0.15f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.color = Color.white;
            textMesh.fontStyle = FontStyle.Bold;
            var textRenderer = textObj.GetComponent<MeshRenderer>();
            textRenderer.sortingOrder = 7;
            textObj.transform.localPosition = new Vector3(flagWidth * 0.4f, flagHeight * 0.8f, -0.1f);

            // Circle marker on path
            var marker = new GameObject("Marker");
            marker.transform.SetParent(flagParent.transform);
            var markerSr = marker.AddComponent<SpriteRenderer>();
            markerSr.sprite = CreateCircleSprite(16);
            markerSr.color = def.color;
            markerSr.sortingOrder = 4;
            marker.transform.localScale = Vector3.one * 0.5f;

            // Pulsing animation component
            var pulse = marker.AddComponent<PulseAnimation>();
            pulse.pulseSpeed = 2f;
            pulse.pulseScale = 0.15f;

            placedMilestones[def.workoutCount] = flagParent;
        }

        public void CheckMilestones(int totalWorkouts)
        {
            foreach (var milestone in milestones)
            {
                if (totalWorkouts >= milestone.workoutCount && !celebratedMilestones.Contains(milestone.workoutCount))
                {
                    celebratedMilestones.Add(milestone.workoutCount);
                    CelebrateMilestone(milestone);
                }
            }
        }

        private void CelebrateMilestone(MilestoneDefinition def)
        {
            Debug.Log($"MEILENSTEIN ERREICHT: {def.title} ({def.workoutCount} Workouts)");

            if (placedMilestones.TryGetValue(def.workoutCount, out GameObject flagObj))
            {
                // Spawn confetti particles
                SpawnConfetti(flagObj.transform.position, def.color);

                // Scale up animation
                StartCoroutine(CelebrationAnimation(flagObj));
            }

            // Notify UI
            if (GameManager.Instance?.gameUI != null)
                GameManager.Instance.gameUI.ShowMilestonePopup(def.title, def.emoji, def.color);
        }

        private void SpawnConfetti(Vector3 position, Color baseColor)
        {
            var confettiObj = new GameObject("Confetti");
            confettiObj.transform.position = position + Vector3.up * flagHeight;

            var ps = confettiObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = celebrationDuration;
            main.loop = false;
            main.startLifetime = 2f;
            main.startSpeed = 5f;
            main.startSize = 0.15f;
            main.gravityModifier = 1f;
            main.maxParticles = confettiCount;
            main.startColor = new ParticleSystem.MinMaxGradient(
                baseColor,
                new Color(1f, 1f, 0.5f, 1f)
            );

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, confettiCount)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 45f;
            shape.radius = 0.5f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(baseColor, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            ps.Play();
            Destroy(confettiObj, celebrationDuration + 2f);
        }

        private System.Collections.IEnumerator CelebrationAnimation(GameObject obj)
        {
            Vector3 originalScale = obj.transform.localScale;
            float elapsed = 0f;

            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                float scale = 1f + Mathf.Sin(elapsed * Mathf.PI * 4f) * 0.3f * (1f - elapsed / 0.5f);
                obj.transform.localScale = originalScale * scale;
                yield return null;
            }

            obj.transform.localScale = originalScale;
        }

        private Sprite CreateCircleSprite(int res)
        {
            Texture2D tex = new Texture2D(res, res);
            float center = res / 2f;
            for (int x = 0; x < res; x++)
                for (int y = 0; y < res; y++)
                    tex.SetPixel(x, y, Vector2.Distance(new Vector2(x, y), Vector2.one * center) <= center - 1 ? Color.white : Color.clear);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, res, res), Vector2.one * 0.5f, res);
        }

        private Sprite CreateRectSprite(int w, int h)
        {
            Texture2D tex = new Texture2D(w, h);
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    tex.SetPixel(x, y, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, w, h), Vector2.one * 0.5f, Mathf.Max(w, h));
        }
    }

    public class PulseAnimation : MonoBehaviour
    {
        public float pulseSpeed = 2f;
        public float pulseScale = 0.1f;
        private Vector3 baseScale;

        private void Start() => baseScale = transform.localScale;
        private void Update()
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseScale;
            transform.localScale = baseScale * pulse;
        }
    }
}
