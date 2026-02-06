using UnityEngine;

namespace BeOnTrack.Core
{
    public class JourneyTimeline : MonoBehaviour
    {
        [Header("References")]
        public Camera mainCamera;
        public PathSystem pathSystem;
        public AvatarController avatarController;

        [Header("Camera Settings")]
        public float followSpeed = 5f;
        public float defaultZoom = 7f;
        public float scrollZoomMin = 4f;
        public float scrollZoomMax = 15f;
        public float zoomSpeed = 2f;
        public Vector3 cameraOffset = new Vector3(0, 2f, -10f);

        [Header("Timeline Scroll")]
        public float scrollSpeed = 10f;
        public float scrollDamping = 5f;
        public float snapBackSpeed = 3f;
        public float freeScrollTimeout = 3f;

        private float scrollVelocity;
        private float currentScrollPosition;
        private bool isFreeScrolling;
        private float lastInputTime;
        private bool isDragging;
        private Vector3 lastTouchPosition;
        private float targetZoom;

        private void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            targetZoom = defaultZoom;
            mainCamera.orthographicSize = defaultZoom;
            mainCamera.backgroundColor = new Color(0.12f, 0.14f, 0.22f);
        }

        private void Update()
        {
            HandleInput();
            UpdateCameraPosition();
            UpdateZoom();
        }

        private void HandleInput()
        {
            // Touch input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    isDragging = true;
                    lastTouchPosition = touch.position;
                    scrollVelocity = 0f;
                }
                else if (touch.phase == TouchPhase.Moved && isDragging)
                {
                    Vector3 delta = mainCamera.ScreenToWorldPoint(touch.position)
                                  - mainCamera.ScreenToWorldPoint(lastTouchPosition);
                    currentScrollPosition -= delta.x;
                    lastTouchPosition = touch.position;
                    isFreeScrolling = true;
                    lastInputTime = Time.time;
                    scrollVelocity = -delta.x / Time.deltaTime;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    isDragging = false;
                }

                // Pinch to zoom
                if (Input.touchCount == 2)
                {
                    Touch t0 = Input.GetTouch(0);
                    Touch t1 = Input.GetTouch(1);

                    float prevDist = Vector2.Distance(
                        t0.position - t0.deltaPosition,
                        t1.position - t1.deltaPosition
                    );
                    float currDist = Vector2.Distance(t0.position, t1.position);

                    float diff = prevDist - currDist;
                    targetZoom += diff * 0.01f * zoomSpeed;
                    targetZoom = Mathf.Clamp(targetZoom, scrollZoomMin, scrollZoomMax);
                }
            }

            // Mouse input (for editor testing)
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                lastTouchPosition = Input.mousePosition;
                scrollVelocity = 0f;
            }
            else if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3 delta = mainCamera.ScreenToWorldPoint(Input.mousePosition)
                              - mainCamera.ScreenToWorldPoint(lastTouchPosition);
                currentScrollPosition -= delta.x;
                lastTouchPosition = Input.mousePosition;
                isFreeScrolling = true;
                lastInputTime = Time.time;
                scrollVelocity = -delta.x / Time.deltaTime;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            // Mouse scroll zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                targetZoom -= scroll * zoomSpeed * 5f;
                targetZoom = Mathf.Clamp(targetZoom, scrollZoomMin, scrollZoomMax);
            }
        }

        private void UpdateCameraPosition()
        {
            // Apply scroll velocity with damping
            if (!isDragging && isFreeScrolling)
            {
                currentScrollPosition += scrollVelocity * Time.deltaTime;
                scrollVelocity = Mathf.Lerp(scrollVelocity, 0f, scrollDamping * Time.deltaTime);

                // Snap back to avatar after timeout
                if (Time.time - lastInputTime > freeScrollTimeout)
                {
                    isFreeScrolling = false;
                    scrollVelocity = 0f;
                }
            }

            Vector3 targetPos;

            if (isFreeScrolling)
            {
                // Free scroll mode — camera follows scroll position
                float clampedScroll = Mathf.Max(0, currentScrollPosition);
                targetPos = new Vector3(clampedScroll * 0.3f, 0f, 0f) + cameraOffset;
            }
            else
            {
                // Follow avatar mode
                Vector3 avatarPos = avatarController != null
                    ? avatarController.transform.position
                    : Vector3.zero;
                targetPos = avatarPos + cameraOffset;
                currentScrollPosition = Mathf.Lerp(currentScrollPosition, avatarPos.x / 0.3f, snapBackSpeed * Time.deltaTime);
            }

            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPos,
                followSpeed * Time.deltaTime
            );
        }

        private void UpdateZoom()
        {
            mainCamera.orthographicSize = Mathf.Lerp(
                mainCamera.orthographicSize,
                targetZoom,
                5f * Time.deltaTime
            );
        }

        public void FocusOnAvatar()
        {
            isFreeScrolling = false;
            scrollVelocity = 0f;
        }

        public void ScrollToStart()
        {
            currentScrollPosition = 0f;
            isFreeScrolling = true;
            lastInputTime = Time.time;
        }

        public void ScrollToEnd()
        {
            float maxDist = pathSystem.GetCurrentMaxDistance();
            currentScrollPosition = maxDist;
            isFreeScrolling = true;
            lastInputTime = Time.time;
        }
    }
}
