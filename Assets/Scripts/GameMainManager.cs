using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.UI;

public class GameMainManager : SymbolManager
{
    [SerializeField]
    private Transform symbol_layer_parent;
    [SerializeField]
    private MapManager _map_manager;

    [SerializeField]
    private Button _retry_btn;
    [SerializeField]
    public Text _combo_cnt_txt;

    private Vector3[] _symbol_drop_pos;
    byte[] each_line_deleted_symbol_cnt;
    private int get_random_num()
    {
        return Random.Range((int)block_type.Green_Block, (int)block_type.Yellow_Block + 1);
    }
    private int get_random_dir()
    {
        return Random.Range(0, 2);
    }
    private void update_combo_UI(int value)
    {
        _combo_cnt_txt.text = value.ToString();
    }
    public override void on_start()
    {
        base.on_start();
    }
    private void Awake()
    {
        _retry_btn.interactable = false;
    }

    private void Start()
    {
        on_start();
        StartCoroutine(check_first_board_match(true));
        update_combo_UI(0);
        set_drop_pos(PublicInfos.Instance.drop_type);
        //make_init_symbols();
    }
    private void set_drop_pos(EDropType type)
    {
        switch(type)
        {
            case EDropType.JustDrop:
                _symbol_drop_pos = new Vector3[PublicInfos.Instance.line_cnt-2];
                for(int i=0; i<_symbol_drop_pos.Length;i++)
                {
                    _symbol_drop_pos[i]= _map_manager.get_box_pos_by_coordinate(i,
                    PublicInfos.Instance.each_line_symbol_cnt[i])
                    + Vector3.up * MapManager._symbols_distance;

                }
                break;
            case EDropType.Popup:
                break;
            case EDropType.Spread:
                _symbol_drop_pos = new Vector3[1];
                _symbol_drop_pos[0] = _map_manager.get_box_pos_by_coordinate((PublicInfos.Instance.line_cnt-2)/2,
                    PublicInfos.Instance.each_line_symbol_cnt[(PublicInfos.Instance.line_cnt - 3) / 2])
                    +Vector3.up* MapManager._symbols_distance;

                break;
        }
        
    }



    private void make_init_symbols(bool first_board=false)
    {
        for (int i = 1; i < PublicInfos.Instance.line_cnt - 1; i++)
        {
            float line_bottom_pos = MapManager._symbols_distance / 2 * (1 - PublicInfos.Instance.each_line_symbol_cnt[i - 1]);
            for (int j = 1; j < PublicInfos.Instance.row_cnt - 1; j++)
            {
                if (PublicInfos.Instance.map_board[i, j] > 0)
                {
                    if(first_board)
                    {
                        init_Symbol(symbol_layer_parent,_map_manager.get_box_pos_by_coordinate(i-1,j-1), block_type.Orange_Block.ToString(),
                            i * PublicInfos.Instance.row_cnt + j);
                    }
                    else
                    {
                        init_Symbol(symbol_layer_parent, _map_manager.get_box_pos_by_coordinate(i - 1, j-1), ((block_type)get_random_num()).ToString(),
                            i * PublicInfos.Instance.row_cnt + j);
                    }
                    
                }
            }

        }
    }

    private void return_all_symbols()
    {
        for(int i=0; i<line_cnt;i++)
        {
            for(int j=0; j<row_cnt;j++)
            {
                if (_ball_symbols_arr[i,j]!=null)
                {
                    _ball_symbols_arr[i, j].Return();
                    _ball_symbols_arr[i, j] = null;
                }
            }
        }
    }


    IEnumerator check_first_board_match(bool is_first_board=false)
    {
        make_init_symbols(is_first_board);
        yield return new WaitForSeconds(.5f);
        StartCoroutine(delete_process());
    }
    
    private IEnumerator drop_remaining_symbols(List<int> deleted_symbols_list)
    {
        int deleted_symbols_cnt = deleted_symbols_list.Count;
        int go_to_index = 0;

        each_line_deleted_symbol_cnt = new byte[line_cnt];
        foreach(int num in deleted_symbols_list)
        {
            each_line_deleted_symbol_cnt[get_x_coordinate_by_index(num)]++;
        }



        //step1. 남아있는 심볼들을 바닥으로 떨궈준다.
        for (int i = 1; i < line_cnt - 1; i++)
        {
            for (int j = 2; j < row_cnt - 1; j++)
            {
                int move_cnt = 0;
                //전체적으로 떨궈야하는만큼을 훑는다.
                if (_ball_symbols_arr[i, j] != null)
                {
                    for (int z = 1; z < j; z++)
                    {
                        if (_ball_symbols_arr[i, z] == null)
                            move_cnt++;
                    }
                }
                if (move_cnt > 0)
                {
                    go_to_index = _ball_symbols_arr[i, j]._symbol_stat._block_num - move_cnt;
                    _ball_symbols_arr[i, j].transform.DOMove(_map_manager.get_box_pos_by_index(go_to_index).position, .1f);
                    _ball_symbols_arr[get_x_coordinate_by_index(go_to_index), get_y_coordinate_by_index(go_to_index)] = _ball_symbols_arr[i, j];
                    if(get_index_by_coordinate(i,j)!=go_to_index)
                        _ball_symbols_arr[i, j] = null;
                    get_ball_by_index(go_to_index)._symbol_stat.set_ball_index(go_to_index, get_around_ball_index(go_to_index));

                    yield return new WaitForSeconds(.1f);
                    //_ball_symbols_arr[get_x_coordinate_by_index(go_to_index), get_y_coordinate_by_index(go_to_index)].play_anim(ESymbol_Anim.Win);

                }

            }
            //yield return new WaitForSeconds(.1f);
        }
        //yield return new WaitForSeconds(.1f);

        Symbol base_ball = null;
        int curr_ball_index = 0;
        int random_dir = 0;

        //step2. 좌우에 빈칸이 존재하는데 맨위에 심볼들이존재할 경우 미끄러트려놔준다.
        if(PublicInfos.Instance.drop_type==EDropType.Spread)
        {
            for (int i = 1; i < line_cnt - 1; i++)
            {
                if (_ball_symbols_arr[i, PublicInfos.Instance.each_line_symbol_cnt[i - 1]] != null)
                {
                    random_dir = get_random_dir();
                    base_ball = _ball_symbols_arr[i, PublicInfos.Instance.each_line_symbol_cnt[i - 1]];
                    curr_ball_index = base_ball._symbol_stat._block_num;

                    //밑, 왼쪽아래, 오른쪽 아래 우선순위 순서로 떨궈준다.
                    while (true)
                    {
                        if (get_ball_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.down]) == null &&
                         PublicInfos.Instance.get_board_value_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.down]) != 0)
                        {
                            go_to_index = base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.down];

                        }
                        else if (get_ball_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.left_down]) == null &&
                            PublicInfos.Instance.get_board_value_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.left_down]) != 0)
                        {
                            go_to_index = base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.left_down];
                        }
                        else if (get_ball_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.right_down]) == null &&
                            PublicInfos.Instance.get_board_value_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.right_down]) != 0)
                        {
                            go_to_index = base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.right_down];
                        }
                        else
                        {

                            break;
                        }
                        _ball_symbols_arr[get_x_coordinate_by_index(curr_ball_index), get_y_coordinate_by_index(curr_ball_index)].transform.DOMove(_map_manager.get_box_pos_by_index(go_to_index).position, .1f);
                        _ball_symbols_arr[get_x_coordinate_by_index(curr_ball_index), get_y_coordinate_by_index(curr_ball_index)]._symbol_stat.set_ball_index(go_to_index, get_around_ball_index(go_to_index));
                        _ball_symbols_arr[get_x_coordinate_by_index(go_to_index), get_y_coordinate_by_index(go_to_index)] = _ball_symbols_arr[get_x_coordinate_by_index(curr_ball_index), get_y_coordinate_by_index(curr_ball_index)];
                        if (curr_ball_index != go_to_index)
                            _ball_symbols_arr[get_x_coordinate_by_index(curr_ball_index), get_y_coordinate_by_index(curr_ball_index)] = null;
                        curr_ball_index = go_to_index;
                        base_ball = _ball_symbols_arr[get_x_coordinate_by_index(go_to_index), get_y_coordinate_by_index(go_to_index)];
                        yield return new WaitForSeconds(.1f);
                        base_ball.play_anim(ESymbol_Anim.Win);
                    }

                }

            }
        }

        //yield return new WaitForSeconds(.1f);

         
        //step 3. 삭제된만큼 심볼들을 생성해준다.
        //위 포지션에 새로 생성 후 블록 위치로 이동

        switch(PublicInfos.Instance.drop_type)
        {
            case EDropType.JustDrop:
                while (deleted_symbols_cnt > 0)
                {
                    for (int i = 1; i < each_line_deleted_symbol_cnt.Length - 1; i++)
                    {
                        if (each_line_deleted_symbol_cnt[i] > 0)
                        {
                            deleted_symbols_cnt--;
                            each_line_deleted_symbol_cnt[i]--;
                            Symbol made_symbol = make_drop_Symbol(symbol_layer_parent, _symbol_drop_pos[i - 1], ((block_type)get_random_num()).ToString(),
                                get_index_by_coordinate(i, PublicInfos.Instance.each_line_symbol_cnt[i - 1]));
                            yield return new WaitForSeconds(.05f);
                            made_symbol.transform.DOMove(_map_manager.get_box_pos_by_index(get_index_by_coordinate(i, PublicInfos.Instance.each_line_symbol_cnt[i - 1])).position, .1f);
                            base_ball = made_symbol;
                            curr_ball_index = base_ball._symbol_stat._block_num;

                            while (true)
                            {
                                if (get_ball_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.down]) == null &&
                                    PublicInfos.Instance.get_board_value_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.down]) != 0)
                                {
                                    go_to_index = base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.down];

                                }
                                else
                                {
                                    break;
                                }
                                _ball_symbols_arr[get_x_coordinate_by_index(curr_ball_index), get_y_coordinate_by_index(curr_ball_index)].transform.DOMove(_map_manager.get_box_pos_by_index(go_to_index).position, .1f);
                                _ball_symbols_arr[get_x_coordinate_by_index(curr_ball_index), get_y_coordinate_by_index(curr_ball_index)]._symbol_stat.set_ball_index(go_to_index, get_around_ball_index(go_to_index));
                                _ball_symbols_arr[get_x_coordinate_by_index(go_to_index), get_y_coordinate_by_index(go_to_index)] = _ball_symbols_arr[get_x_coordinate_by_index(curr_ball_index), get_y_coordinate_by_index(curr_ball_index)];
                                if (curr_ball_index != go_to_index)
                                    _ball_symbols_arr[get_x_coordinate_by_index(curr_ball_index), get_y_coordinate_by_index(curr_ball_index)] = null;
                                curr_ball_index = go_to_index;
                                base_ball = _ball_symbols_arr[get_x_coordinate_by_index(go_to_index), get_y_coordinate_by_index(go_to_index)];
                                base_ball.play_anim(ESymbol_Anim.Win_2, -1, 0f);
                                base_ball.transform.SetParent(symbol_layer_parent);
                            }

                        }

                    }
                    yield return new WaitForSeconds(.1f);

                }
                break;
            case EDropType.Spread:
                for (int i = 0; i < deleted_symbols_cnt; i++)
                {
                    random_dir = get_random_dir();
                    Symbol made_symbol = make_drop_Symbol(symbol_layer_parent, _symbol_drop_pos[0], ((block_type)get_random_num()).ToString(), get_index_by_coordinate(line_cnt / 2, row_cnt - 2));
                    made_symbol.transform.DOMove(_map_manager.get_box_pos_by_index(get_index_by_coordinate(line_cnt / 2, row_cnt - 2)).position, .1f);

                    base_ball = made_symbol;
                    curr_ball_index = base_ball._symbol_stat._block_num;
                    yield return new WaitForSeconds(.1f);


                    //밑, 왼쪽아래, 오른쪽 아래 우선순위 순서로 떨궈준다.
                    while (true)
                    {
                        if (get_ball_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.down]) == null &&
                            PublicInfos.Instance.get_board_value_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.down]) != 0)
                        {
                            go_to_index = base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.down];

                        }
                        else if (get_ball_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.left_down]) == null &&
                            PublicInfos.Instance.get_board_value_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.left_down]) != 0)
                        {
                            go_to_index = base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.left_down];
                        }
                        else if (get_ball_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.right_down]) == null &&
                            PublicInfos.Instance.get_board_value_by_index(base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.right_down]) != 0)
                        {
                            go_to_index = base_ball._symbol_stat._around_ball_index_arr[(int)swipe_direction.right_down];
                        }
                        else
                        {
                            break;
                        }
                        _ball_symbols_arr[get_x_coordinate_by_index(curr_ball_index), get_y_coordinate_by_index(curr_ball_index)].transform.DOMove(_map_manager.get_box_pos_by_index(go_to_index).position, .1f);
                        _ball_symbols_arr[get_x_coordinate_by_index(curr_ball_index), get_y_coordinate_by_index(curr_ball_index)]._symbol_stat.set_ball_index(go_to_index, get_around_ball_index(go_to_index));
                        _ball_symbols_arr[get_x_coordinate_by_index(go_to_index), get_y_coordinate_by_index(go_to_index)] = _ball_symbols_arr[get_x_coordinate_by_index(curr_ball_index), get_y_coordinate_by_index(curr_ball_index)];
                        if (curr_ball_index != go_to_index)
                            _ball_symbols_arr[get_x_coordinate_by_index(curr_ball_index), get_y_coordinate_by_index(curr_ball_index)] = null;
                        curr_ball_index = go_to_index;
                        base_ball = _ball_symbols_arr[get_x_coordinate_by_index(go_to_index), get_y_coordinate_by_index(go_to_index)];
                        yield return new WaitForSeconds(.1f);
                        base_ball.play_anim(ESymbol_Anim.Win_2, -1, 0f);
                        base_ball.transform.SetParent(symbol_layer_parent);
                    }

                }
                break;
            case EDropType.Popup:
                break;

        }

        if (ball_drop_callback != null)
        {
            ball_drop_callback();
            ball_drop_callback = null;
        }

    }

    private int get_move_ball_index(int curr_ball_index, swipe_direction move_dir)
    {
        int ball_index = 0;
        switch (move_dir)
        {
            case swipe_direction.up:
                ball_index = curr_ball_index + 1;
                break;
            case swipe_direction.right_up:
                if (get_x_coordinate_by_index(curr_ball_index) < line_cnt / 2)
                    ball_index = curr_ball_index + row_cnt + 1;
                else
                    ball_index = curr_ball_index + row_cnt;
                break;
            case swipe_direction.right_down:
                if (get_x_coordinate_by_index(curr_ball_index) < line_cnt / 2)
                    ball_index = curr_ball_index + row_cnt;
                else
                    ball_index = curr_ball_index + row_cnt - 1;
                break;
            case swipe_direction.down:
                ball_index = curr_ball_index - 1;
                break;
            case swipe_direction.left_down:
                if (get_x_coordinate_by_index(curr_ball_index) <= line_cnt / 2)
                    ball_index = curr_ball_index - row_cnt - 1;
                else
                    ball_index = curr_ball_index - row_cnt;
                break;
            case swipe_direction.left_up:
                if (get_x_coordinate_by_index(curr_ball_index) <= line_cnt / 2)
                    ball_index = curr_ball_index - row_cnt;
                else
                    ball_index = curr_ball_index - row_cnt + 1;
                break;

        }
        return ball_index;

    }
    public IEnumerator swipe_balls(int from_index, swipe_direction move_dir)
    {
        _retry_btn.interactable = false;
        int to_index = get_move_ball_index(from_index, move_dir);
        if (PublicInfos.Instance.map_board[get_x_coordinate_by_index(to_index), get_y_coordinate_by_index(to_index)] == 0)
            yield break;

        get_ball_by_index(from_index).transform.DOMove(get_ball_by_index(to_index).transform.position, .4f);
        get_ball_by_index(to_index).transform.DOMove(get_ball_by_index(from_index).transform.position, .4f);

        get_ball_by_index(from_index)._symbol_stat.set_ball_index(to_index, get_around_ball_index(to_index));
        get_ball_by_index(to_index)._symbol_stat.set_ball_index(from_index, get_around_ball_index(from_index));


        Symbol temp = _ball_symbols_arr[get_x_coordinate_by_index(to_index), get_y_coordinate_by_index(to_index)];
        _ball_symbols_arr[get_x_coordinate_by_index(to_index), get_y_coordinate_by_index(to_index)] = _ball_symbols_arr[get_x_coordinate_by_index(from_index), get_y_coordinate_by_index(from_index)];
        _ball_symbols_arr[get_x_coordinate_by_index(from_index), get_y_coordinate_by_index(from_index)] = temp;

        yield return new WaitForSeconds(.4f);

        //매칭값 없는 경우 되돌려
        if (check_match() == false)
        {
            _ball_symbols_arr[get_x_coordinate_by_index(to_index), get_y_coordinate_by_index(to_index)].play_anim(ESymbol_Anim.Tension,-1,0f);
            _ball_symbols_arr[get_x_coordinate_by_index(from_index), get_y_coordinate_by_index(from_index)].play_anim(ESymbol_Anim.Tension,-1,0f);
            yield return new WaitForSeconds(.4f);
            get_ball_by_index(from_index).transform.DOMove(get_ball_by_index(to_index).transform.position, .4f);
            get_ball_by_index(to_index).transform.DOMove(get_ball_by_index(from_index).transform.position, .4f);

            get_ball_by_index(from_index)._symbol_stat.set_ball_index(to_index, get_around_ball_index(to_index));
            get_ball_by_index(to_index)._symbol_stat.set_ball_index(from_index, get_around_ball_index(from_index));


            Symbol temp_return = _ball_symbols_arr[get_x_coordinate_by_index(to_index), get_y_coordinate_by_index(to_index)];
            _ball_symbols_arr[get_x_coordinate_by_index(to_index), get_y_coordinate_by_index(to_index)] = _ball_symbols_arr[get_x_coordinate_by_index(from_index), get_y_coordinate_by_index(from_index)];
            _ball_symbols_arr[get_x_coordinate_by_index(from_index), get_y_coordinate_by_index(from_index)] = temp_return;

            yield return new WaitForSeconds(.4f);
            _retry_btn.interactable = true;
        }

        StartCoroutine(delete_process());

    }
    IEnumerator delete_process()
    {
        while (true)
        {
            if (check_match())
            {
                _retry_btn.interactable = false;
                bool delete_end = false;
                PublicInfos.Instance.combo_cnt+=curr_flow_match_cnt;
                update_combo_UI(PublicInfos.Instance.combo_cnt);
                StartCoroutine(delete_balls(() => {
                    delete_end = true;

                }));
                yield return new WaitUntil(() => delete_end == true);
            }
            else
            {
                _retry_btn.interactable = true;
                break;
            }

        }
    }


    Symbol_stat curr_symbol_stat;
    int curr_flow_match_cnt = 0;
    List<int> _match_symbol_list = new List<int>();
    private bool check_match()
    {
        //당첨 있나 체크
        bool has_match = false;
        curr_flow_match_cnt = 0;
        _match_symbol_list = new List<int>();
        for (int i = 1; i < line_cnt - 1; i++)
        {
            for (int j = 1; j < row_cnt - 1; j++)
            {
                if (PublicInfos.Instance.map_board[i, j] > 0 && _ball_symbols_arr[i, j] != null)
                {
                    curr_symbol_stat = _ball_symbols_arr[i, j]._symbol_stat;
                    if (get_color_match_cnt(_ball_symbols_arr[i, j]) >= 5)
                    {
                        //5개이상이면 모두 당첨

                        if (!_match_symbol_list.Contains(curr_symbol_stat._block_num))
                            _match_symbol_list.Add(curr_symbol_stat._block_num);

                        for (int z = 0; z < curr_symbol_stat._around_ball_index_arr.Length; z++)
                        {
                            if (get_ball_by_index(curr_symbol_stat._around_ball_index_arr[z]) == null)
                                continue;
                            if (get_ball_by_index(curr_symbol_stat._around_ball_index_arr[z])._symbol_stat._block_type == curr_symbol_stat.get_ball_type())
                            {
                                if (!_match_symbol_list.Contains(curr_symbol_stat._around_ball_index_arr[z]))
                                    _match_symbol_list.Add(curr_symbol_stat._around_ball_index_arr[z]);
                            }
                        }


                        curr_flow_match_cnt++;
                        has_match = true;
                    }
                    else if (get_color_match_cnt(_ball_symbols_arr[i, j]) == 4)
                    {
                        //직선당첨인 경우
                        for (int z = 0; z < 3; z++)
                        {
                            Symbol symbol_1 = get_ball_by_index(curr_symbol_stat._around_ball_index_arr[z]);
                            Symbol symbol_2 = get_ball_by_index(curr_symbol_stat._around_ball_index_arr[z + 3]);

                            if (symbol_1 == null || symbol_2 == null)
                                continue;
                            if (get_ball_by_index(curr_symbol_stat._around_ball_index_arr[z])._symbol_stat._block_type == curr_symbol_stat.get_ball_type() &&
                                get_ball_by_index(curr_symbol_stat._around_ball_index_arr[z + 3])._symbol_stat._block_type == curr_symbol_stat.get_ball_type())
                            {
                                if (!_match_symbol_list.Contains(curr_symbol_stat._block_num))
                                    _match_symbol_list.Add(curr_symbol_stat._block_num);

                                if (!_match_symbol_list.Contains(curr_symbol_stat._around_ball_index_arr[z]))
                                    _match_symbol_list.Add(curr_symbol_stat._around_ball_index_arr[z]);
                                if (!_match_symbol_list.Contains(curr_symbol_stat._around_ball_index_arr[z + 3]))
                                    _match_symbol_list.Add(curr_symbol_stat._around_ball_index_arr[z + 3]);

                                has_match = true;
                                break;
                            }
                        }
                        if (has_match)
                        {
                            curr_flow_match_cnt++;
                            continue;
                        }

                        bool no_match = false;
                        //당첨 아닌 경우(2가지 케이스)
                        for (int z = 0; z < 2; z++)
                        {
                            Symbol symbol_1 = get_ball_by_index(curr_symbol_stat._around_ball_index_arr[z]);
                            Symbol symbol_2 = get_ball_by_index(curr_symbol_stat._around_ball_index_arr[z + 2]);
                            Symbol symbol_3 = get_ball_by_index(curr_symbol_stat._around_ball_index_arr[z + 4]);

                            if (symbol_1 == null || symbol_2 == null || symbol_3 == null)
                                continue;
                            if (symbol_1._symbol_stat._block_type == curr_symbol_stat.get_ball_type() &&
                                symbol_2._symbol_stat._block_type == curr_symbol_stat.get_ball_type() &&
                                symbol_3._symbol_stat._block_type == curr_symbol_stat.get_ball_type())
                            {
                                no_match = true;
                                break;
                            }
                        }
                        if (no_match)
                            continue;



                        //그 외 사다리꼴/다이아 당첨인 경우
                        has_match = true;
                        curr_flow_match_cnt++;
                        if (!_match_symbol_list.Contains(curr_symbol_stat._block_num))
                            _match_symbol_list.Add(curr_symbol_stat._block_num);

                        for (int z = 0; z < curr_symbol_stat._around_ball_index_arr.Length; z++)
                        {
                            if (get_ball_by_index(curr_symbol_stat._around_ball_index_arr[z]) == null)
                                continue;
                            if (get_ball_by_index(curr_symbol_stat._around_ball_index_arr[z])._symbol_stat._block_type == curr_symbol_stat.get_ball_type())
                            {
                                if (!_match_symbol_list.Contains(curr_symbol_stat._around_ball_index_arr[z]))
                                    _match_symbol_list.Add(curr_symbol_stat._around_ball_index_arr[z]);
                            }
                        }


                    }
                    else if (get_color_match_cnt(_ball_symbols_arr[i, j]) == 3)
                    {
                        //한줄인 경우 당첨
                        for (int z = 0; z < 3; z++)
                        {
                            Symbol symbol_1 = get_ball_by_index(curr_symbol_stat._around_ball_index_arr[z]);
                            Symbol symbol_2 = get_ball_by_index(curr_symbol_stat._around_ball_index_arr[z + 3]);

                            if (symbol_1 == null || symbol_2 == null)
                                continue;

                            if (symbol_1._symbol_stat._block_type == curr_symbol_stat.get_ball_type() &&
                               symbol_2._symbol_stat._block_type == curr_symbol_stat.get_ball_type())
                            {
                                if (!_match_symbol_list.Contains(curr_symbol_stat._block_num))
                                    _match_symbol_list.Add(curr_symbol_stat._block_num);

                                if (!_match_symbol_list.Contains(curr_symbol_stat._around_ball_index_arr[z]))
                                    _match_symbol_list.Add(curr_symbol_stat._around_ball_index_arr[z]);
                                if (!_match_symbol_list.Contains(curr_symbol_stat._around_ball_index_arr[z + 3]))
                                    _match_symbol_list.Add(curr_symbol_stat._around_ball_index_arr[z + 3]);

                                curr_flow_match_cnt++;
                                has_match = true;
                                break;
                            }
                        }
                    }

                }
            }
        }
        set_delete_symbols(_match_symbol_list);

        return has_match;

    }

    public int get_color_match_cnt(Symbol current_ball)
    {
        Symbol_stat current_ball_stat = current_ball._symbol_stat;
        block_type curr_ball_type = current_ball_stat.get_ball_type();
        int color_match_cnt = 1;
        for (int i = 0; i < current_ball_stat._around_ball_index_arr.Length; i++)
        {
            if (PublicInfos.Instance.map_board[get_x_coordinate_by_index(current_ball_stat._around_ball_index_arr[i]), get_y_coordinate_by_index(current_ball_stat._around_ball_index_arr[i])] == 0 ||
                get_ball_by_index(current_ball_stat._around_ball_index_arr[i]) == null)
                continue;

            if (get_ball_by_index(current_ball_stat._around_ball_index_arr[i])._symbol_stat.get_ball_type() == curr_ball_type)
            {
                color_match_cnt++;
            }
        }
        return color_match_cnt;
    }
    public void set_delete_symbols(List<int> delete_symbol_id_list)
    {
        for (int i = 0; i < delete_symbol_id_list.Count; i++)
        {
            get_ball_by_index(delete_symbol_id_list[i])._symbol_stat.set_ball_delete();
        }
    }
    Action ball_drop_callback;
    public IEnumerator delete_balls(Action callback = null)
    {
        ball_drop_callback = callback;
        for (int i = 1; i < line_cnt - 1; i++)
        {
            for (int j = 1; j < row_cnt - 1; j++)
            {
                if (PublicInfos.Instance.map_board[i, j] > 0 && _ball_symbols_arr[i, j] != null)
                {
                    if (_ball_symbols_arr[i, j]._symbol_stat._is_delete)
                    {
                        _ball_symbols_arr[i, j].play_anim(ESymbol_Anim.Win_2);
                    }

                }
            }
        }
        yield return new WaitForSeconds(.4f);
        for (int i = 1; i < line_cnt - 1; i++)
        {
            for (int j = 1; j < row_cnt - 1; j++)
            {
                if (PublicInfos.Instance.map_board[i, j] > 0 && _ball_symbols_arr[i, j] != null)
                {
                    if (_ball_symbols_arr[i, j]._symbol_stat._is_delete)
                    {
                        _ball_symbols_arr[i, j].Return();
                        //Destroy(_ball_symbols_arr[i, j].gameObject);
                        _ball_symbols_arr[i, j] = null;
                    }

                }
            }
        }
        yield return new WaitForSeconds(.1f);
        yield return StartCoroutine(drop_remaining_symbols(_match_symbol_list));
    }

    public void on_click_remake()
    {
        StartCoroutine(remake_board());
        PublicInfos.Instance.init_combo_cnt();
        update_combo_UI(PublicInfos.Instance.combo_cnt);

    }
    IEnumerator remake_board()
    {
        _retry_btn.interactable = false;
        return_all_symbols();
        PublicInfos.Instance.combo_cnt = 0;
        yield return new WaitForSeconds(.1f);
        StartCoroutine(check_first_board_match());
    }

}
