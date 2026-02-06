using UnityEngine;
using System.Collections;

namespace BeOnTrack.Core
{
    public class AvatarController : MonoBehaviour
    {
        [Header("References")]
        public PathSystem pathSystem;
        public SpriteRenderer avatarSprite;
        public ParticleSystem walkTrail;
        public ParticleSystem celebrationBurst;

        [Header("Movement")]
        public float smoothSpeed = 5f;
        public float bounceAmplitude = 0.15f;
        public float bounceSpeed = 8f;

        [Header("Visual")]
        public Color avatarTint = Color.white;
        public float idleBreathScale = 0.03f;
        public float idleBreathSpeed = 2f;

        private float currentDistance;
        private float targetDistance;
        private bool isMoving;
        private Vector3 baseScale;
        private float bounceTimer;

        private void Start()
        {
            if (avatarSprite == null)
                CreateDefaultAvatar();

            baseScale = transform.localScale;

            if (walkTrail != null)
                walkTrail.Stop();
        }

        private void CreateDefaultAvatar()
        {
            // Create a simple colorful circle avatar
            var go = new GameObject("AvatarVisual");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;

            avatarSprite = go.AddComponent<SpriteRenderer>();
            avatarSprite.sprite = CreateCircleSprite(32);
            avatarSprite.color = new Color(0.2f, 0.7f, 1f, 1f); // Bright blue
            avatarSprite.sortingOrder = 10;
            go.transform.localScale = Vector3.one * 0.8f;

            // Add a glow behind the avatar
            var glowObj = new GameObject("AvatarGlow");
            glowObj.transform.SetParent(transform);
            glowObj.transform.localPosition = Vector3.zero;
            var glowSprite = glowObj.AddComponent<SpriteRenderer>();
            glowSprite.sprite = CreateCircleSprite(32);
            glowSprite.color = new Color(0.2f, 0.7f, 1f, 0.2f);
            glowSprite.sortingOrder = 9;
            glowObj.transform.localScale = Vector3.one * 1.5f;

            // Face on avatar
            var faceObj = new GameObject("AvatarFace");
            faceObj.transform.SetParent(go.transform);
            faceObj.transform.localPosition = Vector3.zero;
            var faceSprite = faceObj.AddComponent<SpriteRenderer>();
            faceSprite.sprite = CreateCircleSprite(8);
            faceSprite.color = Color.white;
            faceSprite.sortingOrder = 11;
            faceObj.transform.localScale = Vector3.one * 0.15f;
            faceObj.transform.localPosition = new Vector3(0.1f, 0.1f, 0f);

            // Second eye
            var eye2Obj = new GameObject("AvatarEye2");
            eye2Obj.transform.SetParent(go.transform);
            var eye2Sprite = eye2Obj.AddComponent<SpriteRenderer>();
            eye2Sprite.sprite = CreateCircleSprite(8);
            eye2Sprite.color = Color.white;
            eye2Sprite.sortingOrder = 11;
            eye2Obj.transform.localScale = Vector3.one * 0.15f;
            eye2Obj.transform.localPosition = new Vector3(-0.1f, 0.1f, 0f);
        }

        private Sprite CreateCircleSprite(int resolution)
        {
            Texture2D tex = new Texture2D(resolution, resolution);
            float center = resolution / 2f;
            float radius = center - 1;

            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    tex.SetPixel(x, y, dist <= radius ? Color.white : Color.clear);
                }
            }
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0, 0, resolution, resolution), Vector2.one * 0.5f, resolution);
        }

        private void Update()
        {
            if (isMoving)
                UpdateMovement();
            else
                UpdateIdle();
        }

        private void UpdateMovement()
        {
            currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, smoothSpeed * Time.deltaTime);
            Vector3 targetPos = pathSystem.GetPointAtDistance(currentDistance);

            // Bounce effect while walking
            bounceTimer += Time.deltaTime * bounceSpeed;
            float bounce = Mathf.Abs(Mathf.Sin(bounceTimer)) * bounceAmplitude;
            targetPos.y += bounce;

            transform.position = Vector3.Lerp(transform.position, targetPos, 10f * Time.deltaTime);

            // Squash and stretch
            float squash = 1f + Mathf.Sin(bounceTimer * 2f) * 0.08f;
            transform.localScale = new Vector3(baseScale.x / squash, baseScale.y * squash, baseScale.z);

            // Face direction of movement
            Vector3 dir = pathSystem.GetDirectionAtDistance(currentDistance);
            if (dir.x < 0 && avatarSprite != null)
                avatarSprite.flipX = true;
            else if (avatarSprite != null)
                avatarSprite.flipX = false;

            if (Mathf.Abs(currentDistance - targetDistance) < 0.1f)
            {
                isMoving = false;
                transform.localScale = baseScale;
                if (walkTrail != null) walkTrail.Stop();
                PlayArrivalEffect();
            }
        }

        private void UpdateIdle()
        {
            // Gentle breathing animation
            float breath = 1f + Mathf.Sin(Time.time * idleBreathSpeed) * idleBreathScale;
            transform.localScale = baseScale * breath;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
            currentDistance = 0f;
        }

        public void MoveToDistance(float distance, float speed)
        {
            targetDistance = distance;
            smoothSpeed = speed;
            isMoving = true;
            bounceTimer = 0f;

            if (walkTrail != null) walkTrail.Play();
        }

        private void PlayArrivalEffect()
        {
            if (celebrationBurst != null)
            {
                celebrationBurst.transform.position = transform.position;
                celebrationBurst.Play();
            }
        }

        public float GetCurrentDistance() => currentDistance;
        public bool IsMoving() => isMoving;
    }
}
