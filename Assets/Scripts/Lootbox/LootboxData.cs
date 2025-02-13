using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootboxData", menuName = "LootboxData", order = 1)]
public class LootboxData : ScriptableObject
{
    public List<LootboxCatsData> CatsData;
    [Serializable]
    public class LootboxCatsData
    {
        public Rarity Rarity;
        public List<CatLootboxInfo> CatLootboxInfos;
    }
}

[Serializable]
public class CatLootboxInfo
{
    public string Id;
    public string Name;
    public Collection Collection;
}

public enum Collection
{
    Goth = 0,
    PurePink = 1,
    PureWhite = 2,
    Cute = 3,
    Popstar = 4,
    Cozy = 5,
    Elegant = 6,
    Smol = 7,
    Chierful = 8,
    Winter = 9,
    Spring = 10,
    Summer = 11,
    Automn = 12,
    Maid = 13
}
