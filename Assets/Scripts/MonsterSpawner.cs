using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EMonsterName
{
    Cat_Pawn,
    One_Eyed_Boss,

}
public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance { get; set; }
    [SerializeField]
    private MonsterData[] _monsters;
    [SerializeField]
    private Transform _monster_parent;

    public List<MonsterManager> _monster_list { get; set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _monster_list = new List<MonsterManager>();
        spawn_mosnter(EMonsterName.One_Eyed_Boss.ToString());
    }


    public void spawn_mosnter(string name)
    {
        foreach(var mon in _monsters)
        {
            if(mon.name==name)
            {
                MonsterManager new_monster = PoolAllocater.Instance.get_pool_obj(name + "_Pool").GetComponent<MonsterManager>();
                new_monster.transform.SetParent(_monster_parent);
                new_monster.gameObject.SetActive(true);
                new_monster.transform.localScale = Vector3.one;
                new_monster.init_monster(mon);
                _monster_list.Add(new_monster);

            }
        }
    }
    public void monsters_damaged(float value)
    {
        foreach (MonsterManager monster in _monster_list)
            monster.Monster_damaged(value);
    }
}
