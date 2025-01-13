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
        public int Count;
    }
}
