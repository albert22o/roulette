using System;

public class ItemFactory
{
    public Item Create(ItemData data)
    {
        string id = Guid.NewGuid().ToString();
        var item = new Item(data, id);

        return item;
    }
}
