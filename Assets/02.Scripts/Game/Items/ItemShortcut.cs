using UnityEngine;

public class ItemShortcut
{
    public ItemList CurrentItem {  get; private set; }
    public enum ItemList
    {
        HPPostion,
        Bomb
    }
    
}
