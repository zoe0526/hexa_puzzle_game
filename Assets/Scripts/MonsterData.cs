using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 이름
/// HP
/// 공격 세기
/// 보스 유무
/// 
/// </summary>
[CreateAssetMenu(fileName = "New Monster", menuName = "Monster", order = 2)]
public class MonsterData : ScriptableObject
{
    [SerializeField]
    private string _name;
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
    [SerializeField]
    private float _HP;
    public float HP
    {
        get { return _HP; }
        set { _HP = value; }
    }
    [SerializeField]
    private float _Attack_Point;
    public float Attack_Point
    {
        get { return _Attack_Point; }
        set { _Attack_Point = value; }
    }
    [SerializeField]
    private bool _is_boss;
    public bool Is_Boss
    {
        get { return _is_boss; }
        set { _is_boss = value; }
    }
    [SerializeField]
    private Vector3 _spawn_pos;
    public Vector3 spawn_pos
    {
        get { return _spawn_pos; }
        set { _spawn_pos = value; }
    }


}
