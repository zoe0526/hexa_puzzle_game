using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoSceneManager : SceneManager
{
    private void Awake()
    {
        make_immutable_obj();
    }
    public void on_click_start_game()
    {
        load_scene(scene_name.LoadingScene);
    }
    private void make_immutable_obj()
    {
        GameObject obj = Instantiate(Resources.Load("ImmutableObject")) as GameObject;
        obj.name = "ImmutableObject";
        DontDestroyOnLoad(obj);
    }
}
