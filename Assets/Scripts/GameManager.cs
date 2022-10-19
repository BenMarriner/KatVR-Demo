using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public static class GameManager
{
    public static void NewGame()
    {
        SceneManager.LoadScene(0);
    }

    public static void Quit()
    {
        Application.Quit();
    }
}
