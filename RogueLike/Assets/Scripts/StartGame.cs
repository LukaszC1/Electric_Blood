using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void StartGameplay()
    {
        int x = Random.Range(0, 3);

        switch (x)
        {
            case 0:
                SceneManager.LoadScene("GameAssasin");
                break;

            case 1:
                SceneManager.LoadScene("GameEnforcer");
                break;

            case 2:
                SceneManager.LoadScene("GamePrototype");
                break;
        }

        SceneManager.LoadScene("Game");
    }

    public void StartAssasin()
    {
        SceneManager.LoadScene("GameAssasin");
    }
    public void StartEnforcer()
    {
        SceneManager.LoadScene("GameEnforcer");
    }
    public void StartPrototype()
    {
        SceneManager.LoadScene("GamePrototype");
    }


}
