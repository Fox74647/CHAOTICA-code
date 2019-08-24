using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;
using static StructsAndEnums;

[System.Serializable]
public class ExperienceClass
{
	public int Level, TargetExp, CurrentExp;
}

[System.Serializable]
public class CharacterClass
{
	public string Name;
	public Modes Mode;
	public int BaseHealth, CurrentHealth, BaseModeUp, CurrentModeUp;
	public int BaseAttack, CurrentAttack, BaseDefence, CurrentDefence, BaseLuck, CurrentLuck;
	public CastCommand[] Commands;
	public List <Status> StatusList;
	public List <Defence> DefenceBuffList;
	public ExperienceClass Experience;
	
	void Awake()
	{
		for (int j = 0; j < Commands.Length; j++)
		{
			Commands[j].CastValue = Commands[j].CastBaseValue * ((int)Mode+1);
			CurrentAttack = BaseAttack * ((int)Mode+1);
		}
	}
	public void HealthLimit()
	{
		if (CurrentHealth > (BaseHealth/8)*(8-(int)Mode))
		{
			HealthReset();
		}
	}

	public void HealthReset()
	{
		CurrentHealth = (BaseHealth/8)*(8-(int)Mode);
	}

	public void ModeUpCheck()
	{
		if (CurrentModeUp >= BaseModeUp*((int)Mode+1))
		{
			if (Mode < GameController_.MaxMode)
			{
				CurrentModeUp = 0;
				Mode++;
				ValueUpdate();
				HealthReset();
			}
			else
			{
				CurrentModeUp = 0;
				if (GameController_.CurrentDifficulty <= Difficulty.Easy)
				{
					HealthReset();
				}
			}
		}
	}

	public void ModeDownCheck()
	{
		if (CurrentHealth <= 0)
		{
			if (Mode > 0)
			{
				CurrentModeUp = 0;
				Mode--;
				ValueUpdate();
				HealthReset();
			}
			else
			{
				//sets mode up bar to 0, to prevent possible moding up while dead
				CurrentModeUp = 0;
				CurrentHealth = 0;
			}
		}
	}

	public void ValueUpdate()
    {
        float AttackMultiplier = 1, LuckMultiplier = 1;

        if (StatusHandler.Check(ref StatusList, StatusType.AttackUp))
        {
            AttackMultiplier += GameController_.AttackModifier;
        }
        else if (StatusHandler.Check(ref StatusList, StatusType.AttackDown))
        {
            AttackMultiplier -= GameController_.AttackModifier;
        }

        if (StatusHandler.Check(ref StatusList, StatusType.LuckUp))
        {
            LuckMultiplier = GameController_.LuckModifier;
        }

        CurrentAttack = (int)(BaseAttack * ((int)Mode+1) * AttackMultiplier);
        CurrentDefence = BaseDefence;
        for (int i = 0; i < DefenceBuffList.Count; i++)
        {
            CurrentDefence += DefenceBuffList[i].DefenceValue;
        }
        CurrentLuck = (int)(BaseLuck * LuckMultiplier);

        for (int i = 0; i < Commands.Length; i++)
        {
            Commands[i].CastValue = Commands[i].CastBaseValue * ((int)Mode+1);
        } 
    }
}

public class CharacterDetails : MonoBehaviour
{
    public static CharacterDetails CharacterDetails_;

    public CharacterClass[] CharacterList;
    public int[] PartyDetails;

	public CharacterClass GetCharacter(int ID)
    {
        return CharacterList[ID];
    }

	public int GetPartySize()
	{
		return Mathf.Min(PartyDetails.Length, 4);
	}	

    void Awake()
	{
		Debug.Log("Checking for existing character details");
		if (CharacterDetails_ == null)
		{
			DontDestroyOnLoad(gameObject);
			CharacterDetails_ = this;
			Debug.Log("Character details do not exist, creating character details");
		}
		else if (CharacterDetails_ != this)
		{
			Debug.Log("Character details exists, deleting spare gameobject");
			Destroy(gameObject);
		}
	}
}
