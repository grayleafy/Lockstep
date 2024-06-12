using LeafNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public enum LoginState
{
    Success,
    AnotherSameOnlinePlayer,
}

[Serializable]
public enum GameState
{
    Normal,
    InGaming,
}

[Serializable]
public class LoginResMsg : MessageBase
{
    public LoginState state;
    public GameState playState;
    public string name;
}

