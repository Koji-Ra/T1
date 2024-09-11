using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton :MonoBehaviour{

    [SerializeField] Item[] itemDatas;
    Button b;
    public Note note;
    private void Start()
    {
        b = GetComponent<Button>();
        b.onClick.AddListener(UpdateNote);
    }
    void UpdateNote()
    {
        if (note != null)
        {
            note.gameObject.SetActive(true);
            note.UpdateNote(itemDatas[0].itemName, itemDatas[0].description);
        }
    }
}
[System.Serializable]
public class Item
{

    public string itemName;
    [TextArea(3, 5)]
    public string description;
}