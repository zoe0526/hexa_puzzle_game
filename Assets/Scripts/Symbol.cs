using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using DG.Tweening;
using System;


public enum ESymbol_Anim
{
    Win,
    Sleep,
    Appear,
    Tension,
    Win_2
}


[Serializable]
public class Symbol_stat
{
    public int _block_num;    //몇번째 칸인지

    public block_type _block_type; //어떤 심볼인지
    public int _block_life;    //몇번 건드려야 터지는지
    public bool _is_delete;
    public int[] _around_ball_index_arr=new int[6];    //up,right_up,right_down,down,left_down,left_up

    public void init_symbol_stat(int block_num, int block_life, block_type block_type, int[] around_ball_index)
    {
        _block_num = block_num;
        _block_type = block_type;
        _block_life = block_life;
        _is_delete = false;
        _around_ball_index_arr = around_ball_index;

    }
    public void set_ball_index(int index, int[] around_arr)
    {
        _block_num = index;
        _around_ball_index_arr = around_arr;
    }
    public void set_ball_delete()
    {
        _is_delete = true;
    }
    public block_type get_ball_type()
    {
        return _block_type;
    }
}



public class Symbol : PooledObj,IBeginDragHandler,IDragHandler
{
    [SerializeField]
    private Animator _symbol_anim;

    private string _symbol_name;
    private GameMainManager _gameMainManager;

    bool is_drag = false;
    Vector3 start_pos;
    Vector3 go_to_pos;

    public Symbol_stat _symbol_stat;
   
    private void Start()
    {
        _gameMainManager = GameObject.FindObjectOfType<GameMainManager>();
        _symbol_anim = GetComponent<Animator>();
    }

    public void init_stat(int block_num, int[] around_index)
    {
        _symbol_stat = new Symbol_stat();
        block_type _type = (block_type)Enum.Parse(typeof(block_type), get_item_name());

        //특별한 심볼값들만 따로 설정
        if(_type==block_type.Top)
        {

        }
        else
            _symbol_stat.init_symbol_stat(block_num, 1, _type,around_index);

    }



    public string get_item_name()
    {
        return _obj_name;
    }
    public void play_anim(ESymbol_Anim anim_name)
    {
        _symbol_anim.Play(anim_name.ToString());
    }
    public void play_anim(ESymbol_Anim anim_name,int layer, float normalize_time)
    {
        _symbol_anim.Play(anim_name.ToString(),layer,normalize_time);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        start_pos = transform.position;
        is_drag = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!is_drag)
            return;

        go_to_pos = Camera.main.ScreenToWorldPoint(eventData.position);
        float distance = Vector2.Distance(start_pos, go_to_pos);
        float angle = 360f - Quaternion.FromToRotation(Vector3.up, go_to_pos - start_pos).eulerAngles.z;
        swipe_direction move_dir = get_move_dir(angle);

        if (distance >= .5f)
        {
            is_drag = false;
            StartCoroutine(_gameMainManager.swipe_balls(_symbol_stat._block_num, move_dir));
        }
    }
    private swipe_direction get_move_dir(float angle)
    {
        if (angle >= 30f && angle < 90f)
            return swipe_direction.right_up;
        else if (angle >= 90f && angle < 150f)
            return swipe_direction.right_down;
        else if (angle >= 150f && angle < 210f)
            return swipe_direction.down;
        else if (angle >= 210f && angle < 270f)
            return swipe_direction.left_down;
        else if (angle >= 270f && angle < 330f)
            return swipe_direction.left_up;
        else
            return swipe_direction.up;
    }




}
