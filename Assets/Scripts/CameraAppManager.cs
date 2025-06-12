using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class CameraAppManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera backCamera; // Assign a secondary camera in the scene
    public Camera frontCamera; // Assign a secondary camera in the scene
    private Camera activeCamera;
    public RenderTexture renderTexture; // Linked to RawImage texture
    public RawImage cameraViewUI; // UI Display Panel

    [Header("Capture Button")]
    public Button captureButton;
    public Button switchCameraButton;
    public AudioSource clickSound;

    private void Start()
    {
        activeCamera = backCamera;
        SetActiveCamera(activeCamera);
        captureButton.onClick.AddListener(CapturePhoto);
        switchCameraButton.onClick.AddListener(SwitchCamera);
    }

    public void OpenCameraView()
    {
        cameraViewUI.gameObject.SetActive(true);
    }

    public void CloseCameraView()
    {
        cameraViewUI.gameObject.SetActive(false);
    }

    private void SetActiveCamera(Camera camToActivate)
    {
        backCamera.enabled = false;
        frontCamera.enabled = false;

        activeCamera = camToActivate;
        activeCamera.enabled = true;
        activeCamera.targetTexture = renderTexture;
    }

    public void SwitchCamera()
    {
        if (activeCamera == backCamera)
            SetActiveCamera(frontCamera);
        else
            SetActiveCamera(backCamera);
    }

    public void CapturePhoto()
    {
        StartCoroutine(CaptureFromRenderTexture());
    }

    [ContextMenu("Capture Photo")]
    public void SimulateCapture() => CapturePhoto();

    [ContextMenu("Switch Camera")]
    public void SimulateSwitch() => SwitchCamera();

    IEnumerator CaptureFromRenderTexture()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        image.Apply();

        RenderTexture.active = currentRT;

        if (clickSound != null)
            clickSound.Play();

        // Optional: Save to persistent path
        byte[] bytes = image.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, "Captured_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");
        File.WriteAllBytes(path, bytes);

        Debug.Log("Photo saved to: " + path);
    }
}
