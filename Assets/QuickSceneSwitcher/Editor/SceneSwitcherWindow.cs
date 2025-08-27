using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SceneSwitcherWindow : EditorWindow
{
    private Vector2 scrollPos;
    private bool showSettings = false;

    private string defaultPath;
    private string additionalPath;

    private string[] allScenePaths;

    private const string DefaultPathKey = "SceneSwitcher_DefaultPath";
    private const string AdditionalPathKey = "SceneSwitcher_AdditionalPath";

    [MenuItem("Tools/Quick Scene Switcher")]
    public static void ShowWindow()
    {
        GetWindow<SceneSwitcherWindow>("Scene Switcher");
    }

    private void OnEnable()
    {
        defaultPath = EditorPrefs.GetString(DefaultPathKey, "Assets/Scenes");
        additionalPath = EditorPrefs.GetString(AdditionalPathKey, "");
        RefreshSceneList();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Available Scenes", EditorStyles.boldLabel);

        if (GUILayout.Button(new GUIContent("⚙", "Settings"), GUILayout.Width(25)))
        {
            showSettings = !showSettings;
        }
        GUILayout.EndHorizontal();

        if (showSettings)
        {
            DrawSettingsPanel();
        }

        DrawSceneList();
    }

    private void DrawSettingsPanel()
    {
        GUILayout.Space(10);
        GUILayout.Label("Settings", EditorStyles.boldLabel);

        // Default Path
        GUILayout.BeginHorizontal();
        GUILayout.Label("Default Path:", GUILayout.Width(90));
        defaultPath = GUILayout.TextField(defaultPath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string selected = EditorUtility.OpenFolderPanel("Select Default Folder", "Assets", "");
            if (!string.IsNullOrEmpty(selected) && selected.StartsWith(Application.dataPath))
            {
                defaultPath = "Assets" + selected.Substring(Application.dataPath.Length);
                EditorPrefs.SetString(DefaultPathKey, defaultPath);
                RefreshSceneList();
            }
        }
        if (GUILayout.Button(new GUIContent("🗑", "Clear Default Path"), GUILayout.Width(25)))
        {
            defaultPath = "";
            EditorPrefs.SetString(DefaultPathKey, defaultPath);
            RefreshSceneList();
        }
        GUILayout.EndHorizontal();

        // Additional Path
        GUILayout.BeginHorizontal();
        GUILayout.Label("Additional Path:", GUILayout.Width(90));
        additionalPath = GUILayout.TextField(additionalPath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string selected = EditorUtility.OpenFolderPanel("Select Additional Folder", "Assets", "");
            if (!string.IsNullOrEmpty(selected) && selected.StartsWith(Application.dataPath))
            {
                additionalPath = "Assets" + selected.Substring(Application.dataPath.Length);
                EditorPrefs.SetString(AdditionalPathKey, additionalPath);
                RefreshSceneList();
            }
        }
        if (GUILayout.Button(new GUIContent("🗑", "Clear Additional Path"), GUILayout.Width(25)))
        {
            additionalPath = "";
            EditorPrefs.SetString(AdditionalPathKey, additionalPath);
            RefreshSceneList();
        }
        GUILayout.EndHorizontal();
    }

    private void DrawSceneList()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(position.height - 80));

        if (allScenePaths.Length == 0)
        {
            GUILayout.Label("No scenes found in the selected paths.");
        }
        else
        {
            foreach (var path in allScenePaths)
            {
                string sceneName = Path.GetFileNameWithoutExtension(path);
                if (GUILayout.Button(sceneName))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(path);
                    }
                }
            }
        }

        GUILayout.EndScrollView();
    }

    private void RefreshSceneList()
    {
        List<string> sceneList = new List<string>();

        if (!string.IsNullOrEmpty(defaultPath) && Directory.Exists(defaultPath))
        {
            sceneList.AddRange(Directory.GetFiles(defaultPath, "*.unity", SearchOption.AllDirectories));
        }

        if (!string.IsNullOrEmpty(additionalPath) && Directory.Exists(additionalPath))
        {
            sceneList.AddRange(Directory.GetFiles(additionalPath, "*.unity", SearchOption.AllDirectories));
        }

        allScenePaths = sceneList.ToArray();
    }
}