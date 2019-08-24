using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StructsAndEnums;

public class StatusHandler : MonoBehaviour
{
    public static StatusHandler StatusHandler_;

    public static void Add(ref List<Status> StatusList, Status NewStatus)
    {
        if (NewStatus.StatusType == StatusType.NoStatus)
        {
            return;
        }
        if (StatusList.Count > 0)
        {
            for (int i = 0; i < StatusList.Count; i++)
            {
                if (StatusList[i].StatusType == NewStatus.StatusType)
                {
                    if (NewStatus.StatusType != StatusType.DefenceUp) //defence up gets set to, rather than added
                    {
                        NewStatus.StatusDuration = Mathf.Min(NewStatus.StatusDuration + StatusList[i].StatusDuration, 9);
                    }
                    StatusList[i] = NewStatus;
                    return;
                }
            }
            if (NewStatus.StatusType != StatusType.Sleep && NewStatus.StatusType != StatusType.LuckUp)
            {
                for (int i = 0; i < StatusList.Count; i++)
                {
                    if ((int)StatusList[i].StatusType + (int)NewStatus.StatusType == 11)
                    {
                        StatusList.RemoveAt(i);
                    }
                }
            }
        }
        StatusList.Add(NewStatus);
        return;
    }

    public static bool Check(ref List<Status> StatusList, StatusType Type)
    {
        if (StatusList.Count > 0)
        {
            for (int i = 0; i < StatusList.Count; i++)
            {
                if (StatusList[i].StatusType == Type)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static void Decrease(ref List<Status> StatusList)
    {
        if (StatusList.Count > 0)
        {
            for (int i = 0; i < StatusList.Count; i++)
            {
                if (StatusList[i].StatusDuration <= 1)
                {
                    StatusList.RemoveAt(i);
                    i--;
                }
                else
                {
                    Status Temp = StatusList[i];
                    Temp.StatusDuration--;
                    StatusList[i] = Temp; 
                }
            }
        }
    }

    public static void Reset(ref List<Status> StatusList)
    {
        StatusList.Clear();
    }
}
