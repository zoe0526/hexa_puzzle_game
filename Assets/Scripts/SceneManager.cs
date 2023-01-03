using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum scene_name
{
    LogoScene,
    LoadingScene,
    GameScene
}
public class SceneManager : MonoBehaviour
{
    public void load_scene(scene_name scene,Action callback=null)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.ToString());
    }
}
