using UnityEngine;

public class ItemShortcut
{
    [SerializeField] Inventory inventory;

    private Inventory.ItemList[] items;
    private int currentIndex = 0;

    public void NextItem()
    {
        currentIndex++;
        if(currentIndex >= items.Length)
            currentIndex = 0;
    }

    public void PrevItem()
    {
        currentIndex--;
        if(currentIndex < 0)
            currentIndex = items.Length - 1;
    }
}
