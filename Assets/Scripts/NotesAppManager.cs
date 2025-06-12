using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotesAppManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField notesInput;
    public Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(CloseNotes);
    }

    public void OpenNotes()
    {
        gameObject.SetActive(true);
        notesInput.ActivateInputField();
    }

    public void CloseNotes()
    {
        notesInput.DeactivateInputField();
        gameObject.SetActive(false);
    }
}
