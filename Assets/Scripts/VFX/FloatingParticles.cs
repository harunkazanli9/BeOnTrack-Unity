using UnityEngine;

namespace BeOnTrack.Core
{
    public class FloatingParticles : MonoBehaviour
    {
        [Header("Settings")]
        public int particleCount = 40;
        public float spawnRadius = 20f;
        public float minSize = 0.05f;
        public float maxSize = 0.2f;
        public float minSpeed = 0.2f;
        public float maxSpeed = 1f;
        public Color[] colors = new Color[]
        {
            new Color(1f, 0.84f, 0f, 0.3f),     // Gold
            new Color(0f, 0.83f, 1f, 0.2f),      // Cyan
            new Color(0.2f, 0.8f, 0.4f, 0.25f),  // Green
            new Color(1f, 0.4f, 0.6f, 0.2f),     // Pink
            new Color(0.6f, 0.4f, 1f, 0.2f)      // Purple
        };

        private struct Particle
        {
            public Transform transform;
            public SpriteRenderer renderer;
            public float speed;
            public float phase;
            public float amplitude;
        }

        private Particle[] particles;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            particles = new Particle[particleCount];

            Sprite circleSprite = CreateGlowSprite();

            for (int i = 0; i < particleCount; i++)
            {
                var obj = new GameObject($"FloatingParticle_{i}");
                obj.transform.SetParent(transform);

                var sr = obj.AddComponent<SpriteRenderer>();
                sr.sprite = circleSprite;
                sr.color = colors[Random.Range(0, colors.Length)];
                sr.sortingOrder = 3;

                float size = Random.Range(minSize, maxSize);
                obj.transform.localScale = Vector3.one * size;

                Vector3 pos = mainCamera.transform.position + new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    Random.Range(-spawnRadius * 0.5f, spawnRadius * 0.5f),
                    0
                );
                obj.transform.position = pos;

                particles[i] = new Particle
                {
                    transform = obj.transform,
                    renderer = sr,
                    speed = Random.Range(minSpeed, maxSpeed),
                    phase = Random.Range(0f, Mathf.PI * 2f),
                    amplitude = Random.Range(0.5f, 2f)
                };
            }
        }

        private Sprite CreateGlowSprite()
        {
            int res = 16;
            Texture2D tex = new Texture2D(res, res);
            float center = res / 2f;
            for (int x = 0; x < res; x++)
            {
                for (int y = 0; y < res; y++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), Vector2.one * center) / center;
                    float alpha = Mathf.Clamp01(1f - dist);
                    alpha = alpha * alpha; // Soft falloff
                    tex.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
            }
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0, 0, res, res), Vector2.one * 0.5f, res);
        }

        private void Update()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                var p = particles[i];
                if (p.transform == null) continue;

                // Gentle floating motion
                float yOffset = Mathf.Sin(Time.time * p.speed + p.phase) * p.amplitude * Time.deltaTime;
                p.transform.position += new Vector3(0.1f * Time.deltaTime, yOffset, 0);

                // Fade based on distance to camera
                float dist = Vector3.Distance(p.transform.position, mainCamera.transform.position);
                if (dist > spawnRadius)
                {
                    // Respawn near camera
                    p.transform.position = mainCamera.transform.position + new Vector3(
                        -spawnRadius + Random.Range(-2f, 2f),
                        Random.Range(-spawnRadius * 0.5f, spawnRadius * 0.5f),
                        0
                    );
                }

                // Pulsing alpha
                Color c = p.renderer.color;
                c.a = Mathf.Abs(Mathf.Sin(Time.time * p.speed * 0.5f + p.phase)) * 0.3f;
                p.renderer.color = c;
            }
        }
    }
}
