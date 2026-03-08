using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "UnitLevelData", menuName = "Game/Units/Level Data")]
public class UnitLevelData : ScriptableObject
{
    public LevelInfo[] levels;

    [System.Serializable]
    public class LevelInfo
    {
        public int requiredExp;
        public SpriteLibraryAsset spriteLibrary;
    }

    public int MaxLevel => levels.Length;
}