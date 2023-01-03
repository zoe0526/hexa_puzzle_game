using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneManager : SceneManager
{
    void Start()
    {
        StartCoroutine(move_game_Scene_delay());
    }

    private IEnumerator move_game_Scene_delay()
    {
        yield return new WaitForSeconds(1f);
        load_scene(scene_name.GameScene);

    }
}
