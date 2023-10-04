using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    PlayerController _playerController = null;
    Dictionary<int, PlayController>  _playDic = new Dictionary<int, PlayController>();  

    public PlayerController PlayerController { get { return _playerController; } set { _playerController = value; } }
    public Dictionary<int, PlayController> PlayerDic { get { return _playDic; } }
}
