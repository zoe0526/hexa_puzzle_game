using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// �������� ����� �ִ� ��ũ��Ʈ.
/// </summary>
public class MapManager : MonoBehaviour
{
    [SerializeField]
    private float _BG_distance;
    public float BG_distance { get { return _BG_distance; } }
    [SerializeField]
    private GameObject[] line_arr;  //���� ������ŭ �����ؼ� �־��ش�.(������ӿ����� 7��)

    private GameObject[,] _map_obj_arr;
    private void Start()
    {
        _map_obj_arr = new GameObject[PublicInfos.Instance.line_cnt, PublicInfos.Instance.row_cnt];
        make_map();

    }



    private void make_map()
    {
        for(int i=0; i<line_arr.Length;i++)
        {
            for(int j=0; j < PublicInfos.Instance.each_line_symbol_cnt[i];j++)
            {
                GameObject obj = PoolAllocater.Instance.get_pool_obj("MapBlock_Pool");
                obj.transform.SetParent(line_arr[i].transform);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = new Vector3(0, box_pos(i,j), 0);
                _map_obj_arr[i+1, j+1] = obj;
                obj.SetActive(true);
            }
        }
    }

    public float line_bottom_pos(int line_index)
    {
        return _BG_distance * (1 - PublicInfos.Instance.each_line_symbol_cnt[line_index]) / 2;
    }
    public float box_pos(int line_index,int row_index)
    {
        return line_bottom_pos(line_index) + _BG_distance*row_index;
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
