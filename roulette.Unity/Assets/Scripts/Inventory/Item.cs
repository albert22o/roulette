using UnityEngine;

public class Item
{
    public ItemData Data { get; private set;  }
    public string Id { get; private set;  }

    public Item(ItemData item, string id)
    {
        this.Data = item;
        Id = id;
    }
}
