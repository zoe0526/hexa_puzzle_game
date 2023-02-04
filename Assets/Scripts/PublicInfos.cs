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
    Yellow_Block,
    Top,

}


public class PublicInfos : MonoBehaviour
{
    public static PublicInfos Instance;
    public int combo_cnt { get; set; }
    public EDropType drop_type { get; set; }

    public byte line_cnt { get; set; }

    public byte row_cnt { get; set; }

    public byte[,] map_board { get; set; }

    public byte[] each_line_symbol_cnt { get; set; }

    public void set_init_values(byte line_cnt, byte row_cnt, EDropType drop_type, byte[,] map_board)
    {
        this.line_cnt = line_cnt;
        this.row_cnt = row_cnt;
        this.drop_type = drop_type;
        this.map_board = map_board;


        each_line_symbol_cnt = new byte[line_cnt - 2];
        for (int i = 1; i < line_cnt - 1; i++)
            for (int j = 0; j < row_cnt; j++)
                if (map_board[i, j] > 0)
                    each_line_symbol_cnt[i - 1]++;
    }


    public int get_board_value_by_index(int index)
    {
        return map_board[index / row_cnt, index % row_cnt];
    }
    public void init_combo_cnt()
    {
        combo_cnt = 0;
    }

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        init_combo_cnt();
      
    }
}
