using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EMonsterAnim
{
    idle,
    attack,
    attack02,
    damage,
    walk,
    dead
}

public class MonsterManager : PooledObj
{
    private MonsterData _monster_data;
    public MonsterData monster_data
    { set { value = _monster_data; } }

    private Animator monster_anim;
    private float _current_HP;
    private Slider _monster_HP_Bar;

    private void Awake()
    {
        monster_anim = GetComponent<Animator>();
    }
    private void make_HP_Bar()
    {
        GameObject Bar = PoolAllocater.Instance.get_pool_obj("HP_Bar" + "_Pool");
        _monster_HP_Bar = Bar.GetComponent<Slider>();
        Bar.transform.SetParent(transform);
        Bar.SetActive(true);
        Bar.transform.localScale = Vector3.one;
        Bar.transform.localPosition = Vector3.zero;
        Bar.gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        _monster_HP_Bar.maxValue = _monster_data.HP;
        _monster_HP_Bar.minValue = 0;
        _monster_HP_Bar.value = _monster_data.HP;
    }
    public void init_monster(MonsterData data)
    {
        _monster_data = data;
        transform.localPosition = _monster_data.spawn_pos;
        _current_HP = _monster_data.HP;
        make_HP_Bar();

    }
    public void play_monster_anim(EMonsterAnim anim)
    {
        monster_anim.Play(anim.ToString());
    }

    public void update_HP_Bar(float value)
    {
        _monster_HP_Bar.value = value;

    }
    private IEnumerator monster_dead()
    {
        _current_HP = 0;
        update_HP_Bar(0);
        play_monster_anim(EMonsterAnim.dead);
        yield return new WaitForSeconds(.5f);
        Return();
        MonsterSpawner.Instance._monster_list.Remove(this);
    }
    public void Monster_damaged(float value)
    {
        _current_HP -= value;
        if (_current_HP >= 0)
        {
            update_HP_Bar(_current_HP);
            play_monster_anim(EMonsterAnim.damage);
        }
        else
            StartCoroutine(monster_dead());
    }

}
