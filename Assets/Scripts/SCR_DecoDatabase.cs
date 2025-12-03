using UnityEngine;

public enum DecoType
{
    Pumpkin1,
    Pumpkin2,
    Pumpkin3,
    CarvedPumpkin1,
    CarvedPumpkin2,
    CarvedPumpkin3,
    WitchHat,
    HangingBat,
    SpreadBat,
    StringedPumpkin,
    StringedBat,
    StringedSkull,
    LeafWreath,
    CatSit,
    CatLay,
    CatStand,
    CobwebFull,
    CobwebCorner,
    FrontSkull,
    SideSkull,
    Bowl,
    RedBowl,
    HangingSpider,
    PumpkinCandyBowl,
    StringedCobweb,
    StringLightGreen,
    StringLightBlue,
    StringLightPurple,
    StringLightPink,
    StringLightYellow,
    Null
}

[CreateAssetMenu(menuName = "Deco Database")]
public class SCR_DecoDatabase : ScriptableObject
{
    [System.Serializable]
    public class Deco
    {
        public DecoType type;
        public string DecoName;
        public GameObject decoPrefab;
        public Sprite decoSprite;
        public float decoPrice;
        public float shopSpawnChance;
    }

    public Deco[] decos;

    public Deco GetDeco(DecoType type)
    {
        foreach (var deco in decos)
        {
            if (deco.type == type)
                return deco;
        }
        Debug.LogWarning($"No deco defined for deco type {type}");
        return null;
    }
}

