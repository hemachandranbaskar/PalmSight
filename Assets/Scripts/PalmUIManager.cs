using UnityEngine;
using UnityEngine.UI;
using Oculus.Interaction;
using System.Collections;

public class PalmUIManager : MonoBehaviour
{
    [Header("Gesture Connection")]
    [SerializeField] private ActiveStateSelector stopSignGesture;

    [Header("UI Components")]
    [SerializeField] private Canvas palmCanvas;
    [SerializeField] private RectTransform uiPanel;
    //[SerializeField] private Button[] appButtons;
    [SerializeField] private GameObject mainUIPanel;
    [SerializeField] private GameObject cameraPanel;
    [SerializeField] private CameraAppManager cameraAppManager;

    [Header("App Buttons")]
    [SerializeField] private Button messageBtn;
    [SerializeField] private Button cameraBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button backBtn;

    [Header("Hand Tracking")]
    [SerializeField] private OVRHand targetHand;
    [SerializeField] private OVRSkeleton handSkeleton;

    [Header("Settings")]
    [SerializeField] private float palmOffset = 0.08f;
    [SerializeField] private float hideDelay = 1.5f;
    [SerializeField] private Transform uiAnchor;

    [Header("Voice Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] appClips;

    private bool uiActive = false;
    private float hideTimer = 0f;
    private bool gestureActive = false;

    void Start()
    {
        palmCanvas.gameObject.SetActive(false);
        cameraPanel.gameObject.SetActive(false);
        stopSignGesture.WhenSelected += ShowPalmUI;
        stopSignGesture.WhenUnselected += StartHideTimer;
        //SetupButtons();

        messageBtn.onClick.AddListener(() => LaunchApp("Messages"));
        cameraBtn.onClick.AddListener(() => LaunchApp("Camera"));
        settingsBtn.onClick.AddListener(() => LaunchApp("Settings"));
        backBtn.onClick.AddListener(BackToMain);
    }

    void Update()
    {
        if (uiActive)
        {
            UpdateUIPosition();
            HandleHideTimer();
        }
    }

    void ShowPalmUI()
    {
        //palmCanvas.gameObject.SetActive(true);
        StartCoroutine(FadeInUI());
        uiActive = true;
        gestureActive = true;
        hideTimer = 0f;
    }

    void StartHideTimer()
    {
        gestureActive = false;
        hideTimer = 0f;
    }

    void HandleHideTimer()
    {
        if (!gestureActive)
        {
            hideTimer += Time.deltaTime;
            if (hideTimer >= hideDelay)
            {
                HidePalmUI();
            }
        }
    }

    void HidePalmUI()
    {
        //palmCanvas.gameObject.SetActive(false);
        StartCoroutine(FadeOutUI());
        uiActive = false;
        gestureActive = false;
    }

    private IEnumerator FadeInUI()
    {
        CanvasGroup group = palmCanvas.GetComponent<CanvasGroup>();
        if (group == null) group = palmCanvas.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        palmCanvas.gameObject.SetActive(true);
        while (group.alpha < 1f)
        {
            group.alpha += Time.deltaTime * 3f;
            yield return null;
        }
    }

    private IEnumerator FadeOutUI()
    {
        CanvasGroup group = palmCanvas.GetComponent<CanvasGroup>();
        if (group == null) group = palmCanvas.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 1f;
        palmCanvas.gameObject.SetActive(false);
        while (group.alpha > 0f)
        {
            group.alpha -= Time.deltaTime * 3f;
            yield return null;
        }
    }

    void UpdateUIPosition()
    {
        if (targetHand.IsTracked && handSkeleton.IsInitialized)
        {
            Transform wrist = handSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_WristRoot].Transform;
            Vector3 palmPos = wrist.position + wrist.up * palmOffset;

            //palmCanvas.transform.position = palmPos;
            Vector3 lookDirection = Camera.main.transform.position - palmPos;
            if (targetHand.IsTracked && handSkeleton.IsInitialized && uiAnchor != null)
            {
                //palmCanvas.transform.position = uiAnchor.position;
                //palmCanvas.transform.rotation = uiAnchor.rotation;
                Transform palm = handSkeleton.Bones[(int)OVRSkeleton.BoneId.Body_LeftHandPalm].Transform;
                Vector3 uiPos = palm.position + palm.forward * palmOffset;
                palmCanvas.transform.position = uiPos;
                palmCanvas.transform.rotation = Quaternion.LookRotation(palm.forward);
            }
            palmCanvas.transform.rotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 180, 0);
        }
    }

    //void SetupButtons()
    //{
    //    string[] appNames = { "Messages", "Camera", "Settings", "Apps" };
    //    for (int i = 0; i < appButtons.Length; i++)
    //    {
    //        int index = i;
    //        appButtons[i].onClick.AddListener(() => LaunchApp(appNames[index]));
    //    }
    //}

    void LaunchApp(string appName)
    {
        Debug.Log($"Launching: {appName}");
        //int index = System.Array.IndexOf(appButtons, appName); //check the appButtons or appNames
        //if (index >= 0 && audioSource != null && appClips[index] != null)
        //    audioSource.PlayOneShot(appClips[index]);

        mainUIPanel.SetActive(false);

        switch (appName)
        {
            case "Camera":
                cameraPanel.SetActive(true);
                cameraAppManager.OpenCameraView(); // optional
                break;
            case "Messages":
            case "Settings":
                // Add logic for others later
                break;
        }
        HidePalmUI();
    }

    public void BackToMain()
    {
        cameraAppManager.CloseCameraView();
        cameraPanel.SetActive(false);
        mainUIPanel.SetActive(true);
    }

    void OnDestroy()
    {
        if (stopSignGesture != null)
        {
            stopSignGesture.WhenSelected -= ShowPalmUI;
            stopSignGesture.WhenUnselected -= StartHideTimer;
        }
    }
}