using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    PlayerController _playerController = null;
    Dictionary<int, PlayerAbleController>  _playerAbleDic = new Dictionary<int, PlayerAbleController>();  

    public PlayerController PlayerController { get { return _playerController; } set { _playerController = value; } }
    public Dictionary<int, PlayerAbleController> PlayerAbleDic { get { return _playerAbleDic; } }
}
