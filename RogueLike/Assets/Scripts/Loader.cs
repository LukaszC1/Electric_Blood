using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Static class that handles scene loading.
/// </summary>
public static class Loader 
{
    private static Scene targetScene;

    /// <summary>
    /// Enum holding the possible scenes to load.
    /// </summary>
    public enum Scene 
    {
        MainMenu,
        GameScene,
        LoadingScene,
        LobbyScene,
        CharacterSelection,
    }

    /// <summary>
    /// Method that loads the target scene.
    /// </summary>
    /// <param name="targetScene"></param>
    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    /// <summary>
    /// Method which loads the scene for all the clients.
    /// </summary>
    /// <param name="targetScene"></param>
    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    /// <summary>
    /// Loader callback methods.
    /// </summary>
    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
