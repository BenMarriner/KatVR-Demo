using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public static class SceneLoader
{
    public static void LoadScenes()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }

    public static void ReloadScenes()
    {
        UnloadScenes();
        SceneManager.UnloadSceneAsync("DontDestroyOnLoad");
        LoadScenes();
    }

    private static void UnloadScenes()
    {
        SceneManager.UnloadSceneAsync(1);
        SceneManager.UnloadSceneAsync(2);
    }
}
