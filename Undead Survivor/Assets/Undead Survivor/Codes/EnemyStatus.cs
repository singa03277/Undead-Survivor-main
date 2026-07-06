using System;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [Flags]
    public enum enemyStatus
    {
        none = 0,
        projectileDot = 1 << 1,
        backDot = 1 << 2,
        stun = 1 << 3,
    }

    enemyStatus currentStatus;
    enemyStatus preStatus;

    void addStatus(enemyStatus status)
    {
        preStatus = currentStatus;
        currentStatus |= status;
    }
    void removeStatus(enemyStatus status)
    {
        preStatus = currentStatus;
        currentStatus &= ~status;
    }

}
