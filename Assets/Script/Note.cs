using TMPro;
using UnityEngine;

public class Note : MonoBehaviour {

    public TextMeshProUGUI noteNameText, noteDesText;
    public void UpdateNote(string noteName, string noteDes)
    {
        noteNameText.text = noteName;
        noteDesText.text = noteDes;
    }
}