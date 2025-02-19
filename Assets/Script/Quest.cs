using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Game", order = 0)]
public class Quest : ScriptableObject
{
    public string questDescription;
    public List<Solution> solutions;
    public BadSolution badSolution;
}

[System.Serializable]
public class Solution
{
    public ItemsType item;
    public int score;
    public string scoreReply;
    public string dialogReply;
}

[System.Serializable]
public class BadSolution
{
    public string scoreReply;
    public string dialogReply;
}

[System.Serializable]
public enum ItemsType{
    Bow,
    BrokenWand,
    Fruit,
    Gem,
    GoatCheese,
    GoldenGoblet,
    MagicChicken,
    MagicRune,
    MysticScroll,
    OldBoot,
    Pan,
    PumpkinPie,
    RustyBucket,
    RustySword
}