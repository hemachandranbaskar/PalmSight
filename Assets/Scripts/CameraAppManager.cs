using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class CameraAppManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera virtualCamera; // Assign a secondary camera in the scene
    public RenderTexture renderTexture; // Linked to RawImage texture
    public RawImage cameraViewUI; // UI Display Panel

    [Header("Capture Button")]
    public Button captureButton;
    public AudioSource clickSound;

    private void Start()
    {
        captureButton.onClick.AddListener(CapturePhoto);
    }

    public void OpenCameraView()
    {
        cameraViewUI.gameObject.SetActive(true);
    }

    public void CloseCameraView()
    {
        cameraViewUI.gameObject.SetActive(false);
    }

    public void CapturePhoto()
    {
        StartCoroutine(CaptureFromRenderTexture());
    }

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
