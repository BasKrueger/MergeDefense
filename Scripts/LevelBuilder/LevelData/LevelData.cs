using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif
using UnityEngine;

public class LevelData : ScriptableObject
{
    [HideInInspector]
    public List<WaveData> startingWaves;
    [HideInInspector]
    public List<WaveData> repeatableWaves;
    [HideInInspector]
    public List<TileBluePrint> placedBluePrints;

    [HideInInspector]
    public int width;
    [HideInInspector]
    public int height;

    public void SetUp(List<WaveData> startingWaves, List<WaveData> repeatableWaves, List<TileBluePrint> placedBluePrints, int width, int height)
    {
        this.startingWaves = startingWaves;
        this.repeatableWaves = repeatableWaves;
        this.placedBluePrints = placedBluePrints;
        this.width = width;
        this.height = height;
    }

#if UNITY_EDITOR
    [Button(ButtonSizes.Gigantic, Name = "Open in Editor (or double click the asset)")]
    public void OpenInLevelEditor()
    {
        BuildWindow window = BuildWindow.GetWindow();
        window.LoadLevelData(AssetDatabase.GetAssetPath(this));
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        LevelData data = EditorUtility.InstanceIDToObject(instanceID) as LevelData;
        if(data == null)
        {
            return false;
        }
        BuildWindow window = BuildWindow.GetWindow();
        window.LoadLevelData(AssetDatabase.GetAssetPath(data));
        return false;
    }
#endif
}
