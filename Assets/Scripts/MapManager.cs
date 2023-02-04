using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ϵ��� ������ ���
/// </summary>
public enum EDropType
{
    JustDrop,   //�׳� �� �� �� ������ ������
    Spread,     //�߾ӿ��� �������� �����
    Popup       //���ڸ����� �� ��Ÿ��

}

/// <summary>
/// �������� ����� �ִ� ��ũ��Ʈ.
/// </summary>
/// 
public class MapManager : MonoBehaviour
{
    public const float _symbols_distance = 90f;   //��ϵ� ���� �Ÿ�

    private byte _line_cnt;   //���� ���� ����
    private byte _row_cnt;   //���� ���� ����

    [SerializeField]
    public EDropType _drop_type;
    [SerializeField]
    public Transform _map_objs_parent;  //�� �����̳�

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
            Debug.Log("#####�ڽ��� null�ε� �����Ϸ� ��!!"+index);
            return null;
        }
            
        return _map_obj_arr[index / PublicInfos.Instance.row_cnt, index % PublicInfos.Instance.row_cnt].transform;

    }
}
