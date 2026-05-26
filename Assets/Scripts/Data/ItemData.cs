using System;
using System.Collections.Generic;

[Serializable]
public class ItemInfo
{
    public string id;
    public string itemName;
    public string itemType;
    public int healthRestore;
    public string description;
}

[Serializable]
public class ItemDataContainer
{
    public List<ItemInfo> items;
}