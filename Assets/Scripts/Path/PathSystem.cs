using UnityEngine;
using System.Collections.Generic;

namespace BeOnTrack.Core
{
    public class PathSystem : MonoBehaviour
    {
        [Header("Path Settings")]
        public float segmentLength = 5f;
        public float pathWidth = 0.4f;
        public float curveAmplitude = 3f;
        public float curveFrequency = 0.15f;

        [Header("Visual Settings")]
        public Color pathColorStart = new Color(0.2f, 0.8f, 0.4f, 1f);
        public Color pathColorEnd = new Color(0f, 0.83f, 1f, 1f);
        public Color futurePathColor = new Color(1f, 1f, 1f, 0.15f);
        public Material pathMaterial;

        [Header("Environment")]
        public GameObject[] treePrefabs;
        public GameObject[] flowerPrefabs;
        public GameObject[] cloudPrefabs;

        private List<Vector3> pathPoints = new List<Vector3>();
        private LineRenderer pathRenderer;
        private LineRenderer futurePathRenderer;
        private float currentMaxDistance;

        private void Awake()
        {
            SetupRenderers();
        }

        private void SetupRenderers()
        {
            // Walked path
            var pathObj = new GameObject("WalkedPath");
            pathObj.transform.SetParent(transform);
            pathRenderer = pathObj.AddComponent<LineRenderer>();
            pathRenderer.startWidth = pathWidth;
            pathRenderer.endWidth = pathWidth;
            pathRenderer.material = pathMaterial ?? CreateDefaultMaterial();
            pathRenderer.startColor = pathColorStart;
            pathRenderer.endColor = pathColorEnd;
            pathRenderer.sortingOrder = 1;
            pathRenderer.numCapVertices = 5;
            pathRenderer.numCornerVertices = 5;

            // Future/unwalked path
            var futureObj = new GameObject("FuturePath");
            futureObj.transform.SetParent(transform);
            futurePathRenderer = futureObj.AddComponent<LineRenderer>();
            futurePathRenderer.startWidth = pathWidth * 0.6f;
            futurePathRenderer.endWidth = pathWidth * 0.6f;
            futurePathRenderer.material = CreateDefaultMaterial();
            futurePathRenderer.startColor = futurePathColor;
            futurePathRenderer.endColor = futurePathColor;
            futurePathRenderer.sortingOrder = 0;
            futurePathRenderer.numCapVertices = 3;
        }

        private Material CreateDefaultMaterial()
        {
            return new Material(Shader.Find("Sprites/Default"));
        }

        public void GeneratePath(float distance)
        {
            pathPoints.Clear();
            float totalLength = Mathf.Max(distance + 200f, 500f);
            int pointCount = Mathf.CeilToInt(totalLength / segmentLength);

            for (int i = 0; i <= pointCount; i++)
            {
                float t = i * segmentLength;
                float x = t * 0.3f;
                float y = Mathf.Sin(t * curveFrequency) * curveAmplitude
                        + Mathf.Sin(t * curveFrequency * 0.5f) * curveAmplitude * 0.5f;
                pathPoints.Add(new Vector3(x, y, 0f));
            }

            currentMaxDistance = distance;
            UpdatePathVisuals();
            SpawnEnvironment();
        }

        public void ExtendPath(float newDistance)
        {
            if (newDistance > currentMaxDistance)
            {
                float totalLength = newDistance + 200f;
                int currentCount = pathPoints.Count;
                int neededCount = Mathf.CeilToInt(totalLength / segmentLength);

                for (int i = currentCount; i <= neededCount; i++)
                {
                    float t = i * segmentLength;
                    float x = t * 0.3f;
                    float y = Mathf.Sin(t * curveFrequency) * curveAmplitude
                            + Mathf.Sin(t * curveFrequency * 0.5f) * curveAmplitude * 0.5f;
                    pathPoints.Add(new Vector3(x, y, 0f));
                }

                currentMaxDistance = newDistance;
                UpdatePathVisuals();
            }
        }

        private void UpdatePathVisuals()
        {
            int walkedIndex = GetIndexAtDistance(currentMaxDistance);

            // Walked path
            Vector3[] walkedPoints = new Vector3[walkedIndex + 1];
            for (int i = 0; i <= walkedIndex && i < pathPoints.Count; i++)
                walkedPoints[i] = pathPoints[i];

            pathRenderer.positionCount = walkedPoints.Length;
            pathRenderer.SetPositions(walkedPoints);

            // Gradient on walked path
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(pathColorStart, 0f),
                    new GradientColorKey(pathColorEnd, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                }
            );
            pathRenderer.colorGradient = gradient;

            // Future path (dashed look via alpha)
            int futureCount = pathPoints.Count - walkedIndex;
            if (futureCount > 1)
            {
                Vector3[] futurePoints = new Vector3[futureCount];
                for (int i = 0; i < futureCount; i++)
                    futurePoints[i] = pathPoints[walkedIndex + i];

                futurePathRenderer.positionCount = futurePoints.Length;
                futurePathRenderer.SetPositions(futurePoints);
            }
        }

        private void SpawnEnvironment()
        {
            if (treePrefabs == null || treePrefabs.Length == 0) return;

            for (int i = 0; i < pathPoints.Count; i += 3)
            {
                if (Random.value > 0.4f) continue;
                float side = Random.value > 0.5f ? 1f : -1f;
                Vector3 pos = pathPoints[i] + new Vector3(
                    Random.Range(-1f, 1f),
                    side * Random.Range(2f, 5f),
                    0f
                );

                GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                if (prefab != null)
                {
                    var tree = Instantiate(prefab, pos, Quaternion.identity, transform);
                    float scale = Random.Range(0.6f, 1.4f);
                    tree.transform.localScale = Vector3.one * scale;
                }
            }
        }

        public Vector3 GetPointAtDistance(float distance)
        {
            int index = GetIndexAtDistance(distance);
            if (index >= 0 && index < pathPoints.Count)
                return pathPoints[index];
            return pathPoints.Count > 0 ? pathPoints[pathPoints.Count - 1] : Vector3.zero;
        }

        private int GetIndexAtDistance(float distance)
        {
            int index = Mathf.FloorToInt(distance / segmentLength);
            return Mathf.Clamp(index, 0, pathPoints.Count - 1);
        }

        public List<Vector3> GetPathPoints() => pathPoints;
        public float GetCurrentMaxDistance() => currentMaxDistance;

        public Vector3 GetDirectionAtDistance(float distance)
        {
            int index = GetIndexAtDistance(distance);
            if (index < pathPoints.Count - 1)
                return (pathPoints[index + 1] - pathPoints[index]).normalized;
            return Vector3.right;
        }
    }
}
