using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SymbolManager : MonoBehaviour
{
    private Symbol _symbol;
    public Symbol[,] _ball_symbols_arr;
    public int line_cnt;
    public int row_cnt;
    public virtual void on_awake()
    {
        _ball_symbols_arr = new Symbol[PublicInfos.Instance.line_cnt, PublicInfos.Instance.row_cnt];
        line_cnt = PublicInfos.Instance.line_cnt;
        row_cnt = PublicInfos.Instance.row_cnt;
    }
    public void init_Symbol(Transform parent_pos, string item_name, float offset, int block_num)
    {
        _symbol = PoolAllocater.Instance.get_pool_obj(item_name + "_Pool").GetComponent<Symbol>();
        _symbol.transform.SetParent(parent_pos);
        _symbol.init_stat(block_num, get_around_ball_index(block_num));
        _symbol.transform.localScale = Vector3.one;
        _symbol.transform.localPosition = new Vector3(0, offset, 0);
        _symbol.gameObject.SetActive(true);
        _ball_symbols_arr[get_x_coordinate_by_index(block_num), get_y_coordinate_by_index(block_num)] = _symbol;

    }
    public Symbol make_drop_Symbol(Transform parent_pos, string item_name, int block_num)
    {
        _symbol = PoolAllocater.Instance.get_pool_obj(item_name + "_Pool").GetComponent<Symbol>();
        _symbol.play_anim("Idle");
        _symbol.transform.SetParent(parent_pos);
        _symbol.transform.localScale = Vector3.one;
        _symbol.transform.localPosition = Vector3.zero;
        _symbol.init_stat(block_num, get_around_ball_index(block_num));
        _ball_symbols_arr[get_x_coordinate_by_index(block_num), get_y_coordinate_by_index(block_num)] = _symbol;
        _symbol.gameObject.SetActive(true);

        return _symbol;
        /*
         * 추후 셋팅
        _symbol.init_stat(block_num, get_around_ball_index(block_num));
        _ball_symbols_arr[get_x_coordinate_by_index(block_num), get_y_coordinate_by_index(block_num)] = _symbol;
        */
    }
    public int get_x_coordinate_by_index(int index)
    {
        return index / PublicInfos.Instance.row_cnt;
    }
    public int get_y_coordinate_by_index(int index)
    {
        return index % PublicInfos.Instance.row_cnt;
    }
    public int get_index_by_coordinate(int pos_x, int pos_y)
    {
        return pos_x * row_cnt + pos_y;
    }
    public Symbol get_ball_by_index(int index)
    {
        return _ball_symbols_arr[get_x_coordinate_by_index(index), get_y_coordinate_by_index(index)];
    }
    int[] ball_index_arr;
    public int[] get_around_ball_index(int curr_ball_index)
    {
        ball_index_arr = new int[6];
        ball_index_arr[(int)swipe_direction.up] = curr_ball_index + 1;

        if (get_x_coordinate_by_index(curr_ball_index) < line_cnt / 2)
            ball_index_arr[(int)swipe_direction.right_up] = curr_ball_index + row_cnt + 1;
        else
            ball_index_arr[(int)swipe_direction.right_up] = curr_ball_index + row_cnt;

        if (get_x_coordinate_by_index(curr_ball_index) < line_cnt / 2)
            ball_index_arr[(int)swipe_direction.right_down] = curr_ball_index + row_cnt;
        else
            ball_index_arr[(int)swipe_direction.right_down] = curr_ball_index + row_cnt - 1;

        ball_index_arr[(int)swipe_direction.down] = curr_ball_index - 1;

        if (get_x_coordinate_by_index(curr_ball_index) <= line_cnt / 2)
            ball_index_arr[(int)swipe_direction.left_down] = curr_ball_index - row_cnt - 1;
        else
            ball_index_arr[(int)swipe_direction.left_down] = curr_ball_index - row_cnt;
        if (get_x_coordinate_by_index(curr_ball_index) <= line_cnt / 2)
            ball_index_arr[(int)swipe_direction.left_up] = curr_ball_index - row_cnt;
        else
            ball_index_arr[(int)swipe_direction.left_up] = curr_ball_index - row_cnt + 1;

        return ball_index_arr;
    }



}
