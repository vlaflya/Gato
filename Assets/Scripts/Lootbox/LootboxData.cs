using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootboxData", menuName = "LootboxData", order = 1)]
public class LootboxData : ScriptableObject
{
    public Dictionary<Rarity, List<CatLootboxInfo>> RarityData
    {
        get
        {
            if (_rarityData != null)
            {
                return _rarityData;
            }
            else
            {
                _rarityData = new Dictionary<Rarity, List<CatLootboxInfo>>();
                foreach (var data in Data)
                {
                    if (_rarityData.ContainsKey(data.Rarity))
                    {
                        if (_rarityData[data.Rarity] == null)
                        {
                            _rarityData[data.Rarity] = new List<CatLootboxInfo> { data };
                        }
                        else
                        {
                            _rarityData[data.Rarity].Add(data);
                        }
                    }
                    else
                    {
                        _rarityData.Add(data.Rarity, new List<CatLootboxInfo> { data });
                    }
                }
                return _rarityData;
            }
        }
    }
    public List<CatLootboxInfo> Data;

    private Dictionary<Rarity, List<CatLootboxInfo>> _rarityData;

}

[Serializable]
public class CatLootboxInfo
{
    public string Id;
    public string Name;
    public Rarity Rarity;
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
