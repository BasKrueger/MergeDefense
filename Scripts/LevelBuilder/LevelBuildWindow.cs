#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class BuildWindow : OdinEditorWindow
{
    [MenuItem("Tools/Levelbuilder")]
    public static BuildWindow GetWindow()
    {
        BuildWindow window = GetWindow<BuildWindow>();
        window.titleContent = new GUIContent("Levelbuilder");
        window.builder.ReFillTable();
        return window;
    }

    [HorizontalGroup("Window", 300)]
    [BoxGroup("Window/Layout")]
    [SerializeField]
    [InfoBox("Error: Path is not valid", InfoMessageType.Error, "IsPathInvalid")]
    [InfoBox("Path is Valid", InfoMessageType.Info, "IsPathValid")]
    private PathBuilder builder = new PathBuilder();

    [HorizontalGroup("Window")]
    [BoxGroup("Window/Other")]
    [BoxGroup("Window/Other/Waves")]
    [SerializeField]
    private List<WaveData> startingWaves;

    [BoxGroup("Window/Other/Waves")]
    [SerializeField]
    private List<WaveData> repeatableWaves;

    [BoxGroup("Window/Other/FileManagement")]
    [SerializeField]
    private LevelData currentlyEditing
    {
        get
        {
            return openLevel;
        }
        set
        {
            if(value == null)
            {
                ResetEditor();
                return;
            }
            LoadLevelData(AssetDatabase.GetAssetPath(value));
        }
    }

    private LevelData openLevel = null;

    [BoxGroup("Window/Other/FileManagement")]
    [HorizontalGroup("Window/Other/FileManagement/Horizontal")]
    [Button(SdfIconType.Download, ButtonHeight = 50)]
    private void SaveLevel()
    {
        if (IsPathInvalid())
        {
            EditorUtility.DisplayDialog("Levelbuilder: Error", "Levelayout is invalid", "OK");
            return;
        }

        if (!DataLossConfirmed())
        {
            return;
        }
        LevelData data = openLevel;

        EditorUtility.SetDirty(data);
        data.SetUp(
            startingWaves.Clone(),
            repeatableWaves.Clone(),
            builder.placedBluePrints.Clone(),
            PathBuilder.width,
            PathBuilder.height
            );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if(FindObjectOfType<DefenseBoard>().Level == data)
        {
            FindObjectOfType<Path>().SpawnLevelLayout(data);
        }

        EditorUtility.DisplayDialog("Levelbuilder", "Level " + data.name + " saved succesfully", "OK");

        bool DataLossConfirmed()
        {
            if (openLevel != null)
            {
                int result = EditorUtility.DisplayDialogComplex("Levelbuilder: Warning", "Are you sure that you want to save your progress?" +
                    " This will overwrite the existing data for " + openLevel.name, "Save", "Save as Copy", "Cancel");

                switch (result)
                {
                    case 0:
                        return true;
                    case 1:
                        openLevel = CreateLevelData();
                        return openLevel != null;
                    case 2:
                        return false;
                    default:
                        return false;
                }
            }
            else
            {
                openLevel = CreateLevelData();
                return openLevel != null;
            }
        }
        LevelData CreateLevelData()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Level as .asset", "Levelname.asset", "asset", "Please enter a file name to save the texture to");

            if (path == "")
            {
                return null;
            }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance("LevelData"), path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return (LevelData)AssetDatabase.LoadAssetAtPath(path, typeof(LevelData));
        }
    }

    [BoxGroup("Window/Other/FileManagement")]
    [HorizontalGroup("Window/Other/FileManagement/Horizontal")]
    [Button(SdfIconType.Upload, ButtonHeight = 50)]
    private void LoadLevel()
    {
        LoadLevelData();
    }

    [BoxGroup("Window/Other/FileManagement")]
    [HorizontalGroup("Window/Other/FileManagement/Horizontal")]
    [Button(SdfIconType.ArrowClockwise, ButtonHeight = 50)]
    private void ResetEditor()
    {
        if (!DataLossConfirmed())
        {
            return;
        }

        builder.Clear(true);
        startingWaves = new List<WaveData>();
        repeatableWaves = new List<WaveData>();
        openLevel = null;

        bool DataLossConfirmed()
        {
            bool confirmationNeeded = false;
            if (builder.placedBluePrints != null && builder.placedBluePrints.Count > 0)
            {
                confirmationNeeded = true;
            }
            else if (startingWaves != null && startingWaves.Count > 0)
            {
                confirmationNeeded = true;
            }
            else if (repeatableWaves != null && repeatableWaves.Count > 0)
            {
                confirmationNeeded = true;
            }

            if (confirmationNeeded)
            {
                return EditorUtility.DisplayDialog("Levelbuilder: Warning", "Are you sure that you want to reset the Editor? Unsaved progress will get lost.", "Continue", "Cancel");
            }
            else
            {
                return true;
            }
        }
    }

    public void LoadLevelData(string path = "")
    {
        if(!DataLossConfirmed())
        {
            return;
        }

        if(path == "")
        {
            path = EditorUtility.OpenFilePanel("Load level asset", Application.dataPath, "asset");
            path = path.Substring(path.IndexOf("/Assets/") + 1);
        }

        openLevel = (LevelData)AssetDatabase.LoadAssetAtPath(path, typeof(LevelData));

        if (openLevel != null)
        {
            builder.LoadPath(openLevel);

            startingWaves = openLevel.startingWaves.Clone();
            repeatableWaves = openLevel.repeatableWaves.Clone();
        }

        bool DataLossConfirmed()
        {
            bool confirmationNeeded = false;
            if (builder.placedBluePrints != null && builder.placedBluePrints.Count > 0)
            {
                confirmationNeeded = true;
            }
            else if (startingWaves != null && startingWaves.Count > 0)
            {
                confirmationNeeded = true;
            }
            else if (repeatableWaves != null && repeatableWaves.Count > 0)
            {
                confirmationNeeded = true;
            }

            if (confirmationNeeded)
            {
                return EditorUtility.DisplayDialog("Levelbuilder: Warning","Are you sure that you want to load an existing level? Unsaved progress will get lost.", "Continue", "Cancel");
            }
            else
            {
                return true;
            }
        }
    }

    private bool IsPathValid()
    {
        return builder != null && builder.IsPathValid();
    }
    private bool IsPathInvalid()
    {
        return builder != null && !builder.IsPathValid();
    }

    
}
#endif