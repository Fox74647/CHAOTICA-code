using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;
using static StructsAndEnums;

public class EnemyDetails : MonoBehaviour
{
    public static EnemyDetails EnemyDetails_;

    [System.Serializable]
    public struct EnemyClass
{
    public string Name;
    public int MaxHealth, CurrentHealth;
    public int BaseAttack, CurrentAttack, BaseDefence, CurrentDefence, Experience;
    public CastCommand[] Commands;
    public List<Status> StatusList;
    public List<Defence> DefenceBuffList;

    public void ValueUpdate()
    {
        float AttackMultiplier = 1;

        if (StatusHandler.Check(ref StatusList, StatusType.AttackUp))
        {
            AttackMultiplier += GameController_.AttackModifier;
        }
        else if (StatusHandler.Check(ref StatusList, StatusType.AttackDown))
        {
            AttackMultiplier -= GameController_.AttackModifier;
        }

        CurrentAttack = (int)(BaseAttack * AttackMultiplier);
        CurrentDefence = BaseDefence;
        for (int i = 0; i < DefenceBuffList.Count; i++)
        {
            CurrentDefence += DefenceBuffList[i].DefenceValue;
        }

        for (int i = 0; i < Commands.Length; i++)
        {
            Commands[i].CastValue = Commands[i].CastBaseValue;
        } 
    }
}

    public EnemyClass[] EnemyList;

    void Awake()
	{
		EnemyDetails_ = this;
	}

    public EnemyClass GetEnemy(int ID)
    {
        return EnemyList[ID];
    }
}
