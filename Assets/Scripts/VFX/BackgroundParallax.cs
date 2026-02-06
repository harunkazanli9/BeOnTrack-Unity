using UnityEngine;

namespace BeOnTrack.Core
{
    public class BackgroundParallax : MonoBehaviour
    {
        [Header("Layers")]
        public ParallaxLayer[] layers;

        [Header("References")]
        public Camera mainCamera;

        private Vector3 lastCameraPosition;

        [System.Serializable]
        public class ParallaxLayer
        {
            public string name;
            public Color color;
            public float parallaxFactor; // 0 = static, 1 = moves with camera
            public float yOffset;
            public float scale = 1f;
            [HideInInspector] public Transform transform;
        }

        private void Start()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            lastCameraPosition = mainCamera.transform.position;

            if (layers == null || layers.Length == 0)
                CreateDefaultLayers();
        }

        private void CreateDefaultLayers()
        {
            layers = new ParallaxLayer[]
            {
                // Sky gradient
                new ParallaxLayer
                {
                    name = "Sky",
                    color = new Color(0.15f, 0.18f, 0.3f),
                    parallaxFactor = 0.05f,
                    yOffset = 8f,
                    scale = 30f
                },
                // Far mountains
                new ParallaxLayer
                {
                    name = "Mountains",
                    color = new Color(0.2f, 0.25f, 0.4f),
                    parallaxFactor = 0.15f,
                    yOffset = 3f,
                    scale = 15f
                },
                // Hills
                new ParallaxLayer
                {
                    name = "Hills",
                    color = new Color(0.15f, 0.35f, 0.25f),
                    parallaxFactor = 0.3f,
                    yOffset = -1f,
                    scale = 10f
                },
                // Ground
                new ParallaxLayer
                {
                    name = "Ground",
                    color = new Color(0.2f, 0.4f, 0.3f),
                    parallaxFactor = 0.5f,
                    yOffset = -5f,
                    scale = 8f
                }
            };

            foreach (var layer in layers)
            {
                var obj = new GameObject($"BG_{layer.name}");
                obj.transform.SetParent(transform);

                var sr = obj.AddComponent<SpriteRenderer>();
                sr.sprite = CreateGradientSprite(layer.color);
                sr.sortingOrder = -10;
                sr.drawMode = SpriteDrawMode.Tiled;
                sr.size = new Vector2(100f, layer.scale);

                obj.transform.position = new Vector3(0, layer.yOffset, 5f);
                layer.transform = obj.transform;
            }
        }

        private Sprite CreateGradientSprite(Color baseColor)
        {
            int w = 4, h = 32;
            Texture2D tex = new Texture2D(w, h);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float t = (float)y / h;
                    Color c = Color.Lerp(baseColor * 0.5f, baseColor, t);
                    tex.SetPixel(x, y, c);
                }
            }
            tex.Apply();
            tex.wrapMode = TextureWrapMode.Repeat;
            return Sprite.Create(tex, new Rect(0, 0, w, h), Vector2.one * 0.5f, h);
        }

        private void LateUpdate()
        {
            Vector3 deltaMovement = mainCamera.transform.position - lastCameraPosition;

            foreach (var layer in layers)
            {
                if (layer.transform == null) continue;
                float parallaxX = deltaMovement.x * layer.parallaxFactor;
                layer.transform.position += new Vector3(parallaxX, 0, 0);
            }

            lastCameraPosition = mainCamera.transform.position;
        }
    }
}
