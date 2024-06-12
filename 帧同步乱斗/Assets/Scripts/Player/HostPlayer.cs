using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum PlayerState
//{
//    UnLogin,
//    Login,
//    InRoom,
//    Gaming,
//}

[Serializable]
public class HostPlayer : PlayerBase
{
    public GameState state;
    public string roomName;
    //public PlayerState playerState = PlayerState.UnLogin;
}
