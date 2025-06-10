using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PalmUIController : MonoBehaviour
{
    [Header("Hand Tracking")]
    [SerializeField] private OVRHand rightHand;
    [SerializeField] private OVRSkeleton handSkeleton;

    [Header("UI Components")]
    [SerializeField] private Canvas palmCanvas;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Button[] actionButtons;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Positioning")]
    [SerializeField] private float palmOffset = 0.08f;
    [SerializeField] private float showThreshold = 0.7f;
    [SerializeField] private float hideDelay = 1.0f;

    private bool isUIVisible = false;
    private float hideTimer = 0f;
    private Vector3 palmPosition;
    private Vector3 palmNormal;

    void Start()
    {
        SetupUI();
        palmCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (rightHand.IsTracked && handSkeleton.IsInitialized)
        {
            CheckPalmGesture();
            UpdateUIPosition();
        }
    }

    void CheckPalmGesture()
    {
        // Get palm center from hand bones
        Transform palmBone = handSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_WristRoot].Transform;
        palmPosition = palmBone.position;
        palmNormal = palmBone.up;

        // Check if palm is facing towards user
        Vector3 toCamera = Camera.main.transform.position - palmPosition;
        float palmDot = Vector3.Dot(palmNormal, toCamera.normalized);

        // Check if hand is in proper orientation (palm up/forward)
        bool isPalmVisible = palmDot > showThreshold;
        bool isHandSteady = ((float)rightHand.GetFingerConfidence(OVRHand.HandFinger.Index)) > 0.8f;

        if (isPalmVisible && isHandSteady)
        {
            ShowPalmUI();
            hideTimer = 0f;
        }
        else
        {
            hideTimer += Time.deltaTime;
            if (hideTimer > hideDelay)
            {
                HidePalmUI();
            }
        }
    }

    void ShowPalmUI()
    {
        if (!isUIVisible)
        {
            palmCanvas.gameObject.SetActive(true);
            isUIVisible = true;
            statusText.text = "Palm Interface Active";
        }
    }

    void HidePalmUI()
    {
        if (isUIVisible)
        {
            palmCanvas.gameObject.SetActive(false);
            isUIVisible = false;
        }
    }

    void UpdateUIPosition()
    {
        if (isUIVisible)
        {
            // Position UI above palm
            Vector3 uiPosition = palmPosition + palmNormal * palmOffset;
            palmCanvas.transform.position = uiPosition;

            // Make UI face the camera
            Vector3 lookDirection = Camera.main.transform.position - uiPosition;
            palmCanvas.transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    void SetupUI()
    {
        // Configure canvas for world space
        palmCanvas.renderMode = RenderMode.WorldSpace;
        palmCanvas.worldCamera = Camera.main;
        palmCanvas.transform.localScale = Vector3.one * 0.001f; // Small scale for close viewing

        // Setup buttons
        for (int i = 0; i < actionButtons.Length; i++)
        {
            int buttonIndex = i;
            actionButtons[i].onClick.AddListener(() => OnButtonPressed(buttonIndex));
        }
    }

    void OnButtonPressed(int buttonIndex)
    {
        string[] buttonNames = { "Messages", "Camera", "Settings", "Apps" };
        Debug.Log($"Button pressed: {buttonNames[buttonIndex]}");
        statusText.text = $"Launched: {buttonNames[buttonIndex]}";

        // Add haptic feedback
        OVRInput.SetControllerVibration(0.3f, 0.3f, OVRInput.Controller.RHand);

        // Route to specific functions
        switch (buttonIndex)
        {
            case 0: // Messages
                LaunchMessaging();
                break;
            case 1: // Camera
                LaunchCamera();
                break;
            case 2: // Settings
                LaunchSettings();
                break;
            case 3: // Apps
                LaunchApps();
                break;
        }
    }

    void LaunchMessaging()
    {
        // Implementation for messaging app
        Debug.Log("Opening messaging interface...");
    }

    void LaunchCamera()
    {
        // Implementation for camera functionality
        Debug.Log("Activating camera mode...");
    }

    void LaunchSettings()
    {
        // Implementation for settings panel
        Debug.Log("Opening settings...");
    }

    void LaunchApps()
    {
        // Implementation for app launcher
        Debug.Log("Opening app grid...");
    }
}

// Additional helper class for UI animations
public class PalmUIAnimator : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform uiPanel;
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float scaleSpeed = 8f;

    private bool targetVisible = false;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = uiPanel.localScale;
        canvasGroup.alpha = 0f;
        uiPanel.localScale = Vector3.zero;
    }

    void Update()
    {
        AnimateVisibility();
    }

    public void SetVisible(bool visible)
    {
        targetVisible = visible;
    }

    void AnimateVisibility()
    {
        float targetAlpha = targetVisible ? 1f : 0f;
        Vector3 targetScale = targetVisible ? originalScale : Vector3.zero;

        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
        uiPanel.localScale = Vector3.MoveTowards(uiPanel.localScale, targetScale, scaleSpeed * Time.deltaTime);
    }
}