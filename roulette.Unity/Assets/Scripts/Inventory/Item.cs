using UnityEngine;

public class Item
{
    public  ItemData Data { get; private set;  }

    public Item(ItemData item)
    {
        this.Data = item;
    }
}
