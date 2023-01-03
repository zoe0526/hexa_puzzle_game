using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum swipe_direction
{
    unable = -1,
    up,
    right_up,
    right_down,
    down,
    left_down,
    left_up

}
public enum block_type
{
    Green_Block = 1,
    Red_Block,
    Blue_Block,
    Orange_Block,
    Purple_Block,
    Yellow_Block,
    Top,

}

public class PublicInfos : MonoBehaviour
{
    public static PublicInfos Instance;
    private int _combo_cnt = 0;
    public int combo_cnt
    { get { return _combo_cnt; } set { _combo_cnt = value; } }
    private int _Top_Life=2;
    public int Top_life { get { return _Top_Life; } }
    private int _line_cnt = 9;
    public int line_cnt { get { return _line_cnt; } }
    private int _row_cnt = 8;
    public int row_cnt { get { return _row_cnt; } }
    private byte[,] _map_board={
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
    public byte[,] map_board
    {
        get { return _map_board; }
        set { _map_board = value; }
    }
    private byte[] _each_line_symbol_cnt;
    public byte[] each_line_symbol_cnt { get { return _each_line_symbol_cnt; } }
    public int get_board_value_by_index(int index)
    {
        return map_board[index / row_cnt, index % row_cnt];
    }
    public void init_combo_cnt()
    {
        _combo_cnt = 0;
    }

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        init_combo_cnt();
        _each_line_symbol_cnt = new byte[line_cnt-2];
        for (int i = 1; i < line_cnt-1; i++)
            for (int j = 0; j < row_cnt; j++)
                if (_map_board[i, j] > 0)
                    _each_line_symbol_cnt[i-1]++;
    }
}
