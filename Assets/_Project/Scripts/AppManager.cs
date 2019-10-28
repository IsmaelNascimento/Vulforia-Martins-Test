using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //print("scene.name:: " + scene.name);
        switch (scene.name)
        {
            case "menu":
            case "ar-gondola":
                Screen.autorotateToLandscapeLeft = false;
                Screen.orientation = ScreenOrientation.Portrait;
                break;
            case "ar-videos":
                Screen.autorotateToLandscapeLeft = false;
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                break;
            default:
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToPortrait = true;
                Screen.orientation = ScreenOrientation.AutoRotation;
                break;
        }
    }
}