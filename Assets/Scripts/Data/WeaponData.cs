using System;
using System.Collections.Generic;

[Serializable]
public class WeaponInfo
{
    public string id;
    public string weaponName;
    public string typeWeapon;
    public int damage;
    public float attackDuration;
    public string prefabPath;
}

[Serializable]
public class WeaponDataContainer
{
    public List<WeaponInfo> weapons;
}