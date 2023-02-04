using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 블록들이 생성될 방식
/// </summary>
public enum EDropType
{
    JustDrop,   //그냥 각 열 맨 위에서 떨어짐
    Spread,     //중앙에서 떨어져서 흩어짐
    Popup       //제자리에서 뿅 나타남

}

/// <summary>
/// 퍼즐판을 만들어 주는 스크립트.
/// </summary>
/// 
public class MapManager : MonoBehaviour
{
    public const float _symbols_distance = 90f;   //블록들 사이 거리

    private byte _line_cnt;   //세로 라인 갯수
    private byte _row_cnt;   //가로 라인 갯수

    [SerializeField]
    public EDropType _drop_type;
    [SerializeField]
    public Transform _map_objs_parent;  //맵 컨테이너

    private GameObject[,] _map_obj_arr;

    [SerializeField]
    private byte[,] _map_board ={

        { 0,0,0,0,0,0,0,0 },
        { 0,1,1,1,0,0,0,0 },
        { 0,1,1,1,1,0,0,0 },
        { 0,1,1,1,1,1,0,0 },
        { 0,1,1,1,1,1,1,0 },
        { 0,1,1,1,1,1,0,0 },
        { 0,1,1,1,1,0,0,0 },
        { 0,1,1,1,0,0,0,0 },
        { 0,0,0,0,0,0,0,0 },

    };
    /*
    private byte[,] _map_board ={

        { 0,0,0,0,0,0 },
        { 0,1,1,1,1,0 },
        { 0,1,1,1,1,0 },
        { 0,1,1,1,1,0 },
        { 0,0,0,0,0,0 },

    };
    */
    private void Awake()
    {
        _line_cnt = (byte)_map_board.GetLength(0);
        _row_cnt = (byte)_map_board.GetLength(1);
        PublicInfos.Instance.set_init_values(_line_cnt,_row_cnt,_drop_type,_map_board);
        _map_obj_arr = new GameObject[_line_cnt, _row_cnt];
        make_map();
    }
    private void Start()
    {
    }



    private void make_map()
    {
        for(int i=0; i<_line_cnt-2;i++)
        {
            for(int j=0; j < PublicInfos.Instance.each_line_symbol_cnt[i];j++)
            {
                GameObject obj = PoolAllocater.Instance.get_pool_obj("MapBlock_Pool");
                obj.transform.SetParent(_map_objs_parent);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = new Vector3(line_pos(i), row_pos(i,j), 0);
                obj.SetActive(true);

                _map_obj_arr[i+1, j+1] = obj;
                obj.SetActive(true);
            }
        }
    }

    public float line_bottom_pos(int line_index)
    {
        return _symbols_distance * (1 - PublicInfos.Instance.each_line_symbol_cnt[line_index]) / 2;
    }
    public float line_pos(int line_index)
    {
        return _symbols_distance * line_index - (_line_cnt - 2) / 2*_symbols_distance;
    }
    public float row_pos(int line_index,int row_index)
    {
        return line_bottom_pos(line_index) + _symbols_distance * row_index;
    }
    public Vector3 get_box_pos_by_coordinate(int line,int row)
    {
        return new Vector3(line_pos(line), row_pos(line,row), 0);
    }

    public Transform get_box_pos_by_index(int index)
    {
        if (_map_obj_arr[index / PublicInfos.Instance.row_cnt, index % PublicInfos.Instance.row_cnt] == null)
        {
            Debug.Log("#####박스가 null인데 접근하려 함!!"+index);
            return null;
        }
            
        return _map_obj_arr[index / PublicInfos.Instance.row_cnt, index % PublicInfos.Instance.row_cnt].transform;

    }
}
