using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserInformation
{
    private static int gameMode, targetIndex;

    public static int GameMode
    {
        get
        {
            return gameMode;
        }
        set
        {
            gameMode = value;
        }
    }

    public static int TargetIndex
    {
        get
        {
            return targetIndex; 
        }
        set
        {
            targetIndex = value;    
        }
    }
}
