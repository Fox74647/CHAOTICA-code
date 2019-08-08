using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScript : MonoBehaviour
{
    private CharacterDetails Characters;
    private EnemyDetails Enemies;
    private Inventory PlayerInventory;

    public CharacterDetails.Character[] CharacterParty;
    public EnemyDetails.Enemy[] EnemyParty;
    public TeamStatusBar CharacterStatus, EnemyStatus;
    private bool AttackingTeam = true, OptionSelected, CastSelected, AttackingCast;
    private int OptionID = 0, CastID = 0;
    private float SelectDelay = 0.25f, AttackDelay = 1f;
    private int ActiveCharacter, ActiveEnemy, CharacterPartySize, EnemyPartySize, FriendlyTarget;

    public int[] EnemyPartyDetails;
    public GameObject OptionSelectMarker, CastMenu, CastMenuText;
    public ModeDisplay[] ModeDisplays;
    public DetailDisplay[] TextDisplays; //0th element is for active character, 1st is for enemy
    public CastDisplay[] CastDisplays; //For the Cast Command
    public bool CanFlee;

    void Awake() //Runs before every other function
    {
        Characters = CharacterDetails.Details_Characters;
        Enemies = EnemyDetails.Details_Enemy;
        PlayerInventory = Inventory.PlayerInventory;
    }

    void OnEnable() //Runs every time the battle screen is switched from disabled to enabled
    {
        GrabPlayerParty(); 
        GrabEnemyParty(); 
        ModeBarUpdate();
        FriendlyTarget = ActiveCharacter;
    }

    void OnDisable() //Runs every time the battle screen is switched from enabled to disabled
    {
        for (int i = 0; i < 4; i++)
        {
            ModeDisplays[i].NullModeDisplay();
        }
    }

    private void GrabPlayerParty() //Grabs all the characters for the fight
    {
        CharacterPartySize = Mathf.Min(Characters.PartyDetails.Length, 4);
        CharacterParty = new CharacterDetails.Character[CharacterPartySize];
        for (int i = 0; i < CharacterPartySize; i++)
        {
            CharacterParty[i] = Characters.GetCharacter(Characters.PartyDetails[i]);
        }
        for (int i = 0; i < CharacterPartySize; i++)
        {
            CharacterValueUpdate(i);
        }
        CharacterStatus.SetTeamSize(CharacterPartySize);
        HealthLimit();
    }
    
    private void GrabEnemyParty() //Grabs all the enemies for the fight
    {
        EnemyPartySize = EnemyPartyDetails.Length;
        EnemyParty = new EnemyDetails.Enemy[EnemyPartySize];
        for (int i = 0; i < EnemyPartySize; i++)
        {
            EnemyParty[i] = Enemies.GetEnemy(EnemyPartyDetails[i]);
        }
        EnemyStatus.SetTeamSize(EnemyPartySize);
    }

    void Update()
    {
        TestingFunction();

        //check for dead team here
        ShortenSelectDelay();
        if (AttackingTeam)
        {
            if (!DeadCheck())
            {
                PartyAttack();
            }
        }
        else
        {
            if (!DeadCheck())
            {
                EnemyAttack();
            }
            
        }
        TextDisplays[0].UpdatePlayerDisplay(CharacterParty[FriendlyTarget]);
        TextDisplays[1].UpdateEnemyDisplay(EnemyParty[ActiveEnemy]);
        DetailDisplayUpdate();
    }

    private void ShortenSelectDelay()
    {
        if (SelectDelay > 0)
        {
            SelectDelay -= Time.deltaTime;
        }
    }

    private void SelectDelayReset()
    {
        SelectDelay = 0.25f;
    }

    private void PartyAttack() //Player performs their actions here
    {
        if (!OptionSelected)
        {
            OptionSelectMarker.SetActive(true);
            OptionSelect();
        }
        else 
        {
            OptionSelectMarker.SetActive(false);
            switch(OptionID)
            {
                case 0: //attack
                {
                    AttackSelect();
                    break;
                }
                case 1: //defend
                {
                    if (!CharacterParty[ActiveCharacter].DefenceIncreased)
                    {
                        CharacterParty[ActiveCharacter].DefenceIncreased = true;
                        CharacterParty[ActiveCharacter].DefenceIncreaseValue = CharacterParty[ActiveCharacter].Defence;
                        CharacterParty[ActiveCharacter].DefenceIncreaseLength = 1;
                        CharacterParty[ActiveCharacter].Defence += CharacterParty[ActiveCharacter].DefenceIncreaseValue;
                        AdvanceTurn(); 
                    }               
                    break;
                }
                case 2: //cast
                {
                    CastMenu.SetActive(true);
                    CastMenuText.SetActive(true);
                    for (int i = 0; i < 3; i++)
                    {
                        if (i < CharacterParty[ActiveCharacter].Commands.Length)
                        {
                            CastDisplays[i].UpdateCastDisplay(CharacterParty[ActiveCharacter].Commands[i]);
                        }
                        else
                        {
                            CastDisplays[i].NullCastDisplay();
                        }
                    }
                    if (!CastSelected)
                    {
                        CastSelect();
                    }
                    else
                    {
                        switch((int)CharacterParty[ActiveCharacter].Commands[CastID].CastType)
                        {
                            case 0: //attack
                            {
                                AttackSelect();
                                break;
                            }
                            case 1: //heal
                            case 2: //fortify
                            {

                                FriendlySelect();
                                break;
                            }
                        }
                    }
                    break;
                }
                case 3: //item
                {
                    OptionSelected = false; //allow the player to choose another option
                    //while this isn't implemented, the player can't select this option
                    //choose item
                    break;
                }
                case 4: //flee
                {
                    if (CanFlee)
                    {
                        //flee
                    }
                    else
                    {
                        OptionSelected = false; //allow the player to choose another option
                        //not exactly fair to let the player lose a turn trying to flee from a fight they can't flee
                    }
                    break;
                }
            }
        }
    }

    private void EnemyAttack() //automatically attacks the player party at random
    {
        //this will be reworked later to include casting and defending

        if (AttackDelay > 0)
        {
            AttackDelay -= Time.deltaTime;
        }
        else
        {
            ActiveCharacter = Random.Range(0, CharacterPartySize);
            int DamageDealt = Mathf.Max((EnemyParty[ActiveEnemy].Attack - CharacterParty[ActiveCharacter].Defence), 0); 
            CharacterParty[ActiveCharacter].CurrentHealth -= DamageDealt;
            ModeDownCheck();
            ModeBarUpdate();
            AdvanceTurn(); 
            AttackDelay = 1f;
        }
    }

    private void OptionSelect() //player selects an option from the left side
    {
        if (SelectDelay <= 0)
        {
            if (Input.GetAxis("Vertical") < 0 && OptionID < 4)
            {
                OptionID ++;
                SelectDelay = 0.25f;
                OptionSelectMarker.transform.position -= new Vector3(0, 32, 0); //distance between option marker positions is 32
            }
            else if (Input.GetAxis("Vertical") > 0 && OptionID > 0)
            {
                OptionID --;
                SelectDelay = 0.25f;
                OptionSelectMarker.transform.position += new Vector3(0, 32, 0);
            }
            else if (Input.GetAxis("Submit") > 0)
            {
                SelectDelay = 0.25f;
                OptionSelected = true;
            }
        }
    }

    private void AttackSelect() //Player selects an enemy target
    {
        if (SelectDelay <= 0)
        {
            if (Input.GetAxis("Horizontal") > 0 && ActiveEnemy < EnemyPartySize-1)
            {
                SelectDelayReset();
                ActiveEnemy++;
            }
            else if (Input.GetAxis("Horizontal") < 0 && ActiveEnemy > 0)
            {
                SelectDelayReset();
                ActiveEnemy--;
            }
            else if (Input.GetAxis("Cancel") > 0)
            {
                SelectDelayReset();
                if (OptionID == 2) //cast
                {
                    CastSelected = false;
                }
                else
                {
                    OptionSelected = false;
                }
            }
            else if (Input.GetAxis("Submit") > 0)
            {  
                SelectDelayReset();
                if (EnemyParty[ActiveEnemy].CurrentHealth != 0)
                {   
                    AttackEnemy();
                }
            }
        }
    }

    private void FriendlySelect() //Player selects an enemy target
    {
        if (SelectDelay <= 0)
        {
            if (Input.GetAxis("Horizontal") > 0 && FriendlyTarget < CharacterPartySize-1)
            {
                SelectDelayReset();
                FriendlyTarget++;
            }
            else if (Input.GetAxis("Horizontal") < 0 && FriendlyTarget > 0)
            {
                SelectDelayReset();
                FriendlyTarget--;
            }
            else if (Input.GetAxis("Cancel") > 0)
            {
                SelectDelayReset();
                if (OptionID == 2) //cast
                {
                    CastSelected = false;
                }
                else
                {
                    OptionSelected = false;
                }
            }
            else if (Input.GetAxis("Submit") > 0)
            {  
                SelectDelayReset();
                HealFriend();
            }
        }
    }

    private void CastSelect() //player selects an option from the left side
    {
        CastDisplayColourChange();
        if (SelectDelay <= 0)
        {
           if (Input.GetAxis("Vertical") < 0 && CastID < CharacterParty[ActiveCharacter].Commands.Length-1)
            {
                SelectDelayReset();
                CastID ++;
            }
            else if (Input.GetAxis("Vertical") > 0 && CastID > 0)
            {
                SelectDelayReset();
                CastID --;
            }
            else if (Input.GetAxis("Submit") > 0)
            {
                if (CharacterParty[ActiveCharacter].Commands[CastID].CurrentCooldown == 0)
                {
                    SelectDelayReset();
                    CastSelected = true;
                }
            }
            else if (Input.GetAxis("Cancel") > 0)
            {
                SelectDelayReset();
                OptionSelected = false;
                CastMenu.SetActive(false);
                CastMenuText.SetActive(false);
            }
        }
    }

    private void AttackEnemy()
    {
        int DamageDealt = 0;
        if (OptionID == 2) //cast
        {
            CharacterParty[ActiveCharacter].Commands[CastID].CurrentCooldown = CharacterParty[ActiveCharacter].Commands[CastID].MaxCooldown;
            DamageDealt = CharacterParty[ActiveCharacter].Commands[CastID].CastValue; //calculate the damage, ignoring defence
            EnemyParty[ActiveEnemy].Status = AddStatus(EnemyParty[ActiveEnemy].Status, CharacterParty[ActiveCharacter].Commands[CastID].Status);
        }
        else //ordinary attack
        {
            DamageDealt = Mathf.Max((CharacterParty[ActiveCharacter].CurrentAttack - EnemyParty[ActiveEnemy].Defence), 0); //calculate the damage
        }
        EnemyParty[ActiveEnemy].CurrentHealth = Mathf.Max(EnemyParty[ActiveEnemy].CurrentHealth - DamageDealt, 0); //subtract damage from enemy
        CharacterParty[ActiveCharacter].CurrentModeUp += DamageDealt; //add damage to character mode up
        ModeUpCheck();
        ModeBarUpdate();
        AdvanceTurn();
    }

    private void HealFriend()
    {
        
        //casts can't revive party members, certain items can, however
        if(CharacterParty[FriendlyTarget].CurrentHealth == 0)
        {
            //attempting to cast on a dead party member does nothing.
            //this space here is in preparation for the ITEM menu
        }
        else
        {
            if (OptionID == 2) //cast
            {
                if (CharacterParty[ActiveCharacter].Commands[CastID].CastType == StructsAndEnums.CastType.Heal)
                {
                    CharacterParty[ActiveCharacter].Commands[CastID].CurrentCooldown = CharacterParty[ActiveCharacter].Commands[CastID].MaxCooldown;
                    CharacterParty[FriendlyTarget].CurrentHealth += CharacterParty[ActiveCharacter].Commands[CastID].CastValue; //add health
                    CharacterParty[FriendlyTarget].Status = AddStatus(CharacterParty[FriendlyTarget].Status, CharacterParty[ActiveCharacter].Commands[CastID].Status);
                    HealthLimit();
                    ModeBarUpdate();
                    AdvanceTurn();
                }
                else if (CharacterParty[ActiveCharacter].Commands[CastID].CastType == StructsAndEnums.CastType.Fortify)
                {
                    //rework to handle statuses
                    // if (!CharacterParty[FriendlyTarget].DefenceIncreased)
                    // {
                    //     CharacterParty[ActiveCharacter].Commands[CastID].CurrentCooldown = CharacterParty[ActiveCharacter].Commands[CastID].MaxCooldown;
                    //     CharacterParty[FriendlyTarget].DefenceIncreased = true;
                    //     CharacterParty[FriendlyTarget].DefenceIncreaseValue = CharacterParty[ActiveCharacter].Commands[CastID].CastValue;
                    //     CharacterParty[FriendlyTarget].DefenceIncreaseLength = CharacterParty[ActiveCharacter].Commands[CastID].StatusDuration;
                    //     CharacterParty[FriendlyTarget].Defence += CharacterParty[FriendlyTarget].DefenceIncreaseValue;
                    //     //handle statuses
                    //     HealthLimit();
                    //     ModeBarUpdate();
                    //     AdvanceTurn();
                    // }
                }
            }
        }
    }

    private void CastDisplayColourChange()
    {
        for (int i = 0; i < 3; i++)
        {
            CastDisplays[i].ColourChange(i==CastID);
        }
    }

    private void AdvanceTurn() //Does exactly what it says on the tin
    {
        if (AttackingTeam)
        {
            if (ActiveCharacter+1 == CharacterPartySize)
            {
                SwapActiveTeam();
            }
            else
            {
                ActiveCharacter++;
            }
            OptionSelected = false;
            CastSelected = false;
            CastMenu.SetActive(false);
            CastMenuText.SetActive(false);
            OptionSelectMarker.transform.position += new Vector3(0, 32*OptionID, 0);
            OptionID = 0;
            CastID = 0;
            
        }
        else
        {
            if (ActiveEnemy+1 == EnemyPartySize)
            {
                SwapActiveTeam();
            }
            else
            {
                ActiveEnemy++;
            }
        }
        CastRechargeDecrease();
        SelectDelayReset();
        
        ModeBarUpdate();
    }

    private void SwapActiveTeam()
    {
        ActiveCharacter = 0;
        ActiveEnemy = 0;
        AttackingTeam = !AttackingTeam;
    }

    private void CastRechargeDecrease()
    {
        if (AttackingTeam)
        {
            for (int i = 0; i < CharacterParty[ActiveCharacter].Commands.Length; i++)
            {
                if (CharacterParty[ActiveCharacter].Commands[i].CurrentCooldown > 0)
                {
                    CharacterParty[ActiveCharacter].Commands[i].CurrentCooldown--;
                }
            }
        }
        else
        {
            for (int i = 0; i < EnemyParty[ActiveEnemy].Commands.Length; i++)
            {
                if (EnemyParty[ActiveEnemy].Commands[i].CurrentCooldown > 0)
                {
                    EnemyParty[ActiveEnemy].Commands[i].CurrentCooldown--;
                }
            }
        }

        
    }

    private bool DeadCheck() //checks if the active character or enemy is dead
    {
        if (AttackingTeam)
        {
            if (CharacterParty[ActiveCharacter].Mode == 0 && CharacterParty[ActiveCharacter].CurrentHealth == 0)
            {
                AdvanceTurn();
                return true;
            }
        }
        else
        {
            if (EnemyParty[ActiveEnemy].CurrentHealth == 0)
            {
                AdvanceTurn();
                return true;
            }
        }
        return false;
    }

    public void ModeUpCheck()
	{
		for (int i = 0; i < CharacterPartySize; i++)
		{
			if (CharacterParty[i].CurrentModeUp >= CharacterParty[i].BaseModeUp*((int)CharacterParty[i].Mode+1))
			{
				if ((int)CharacterParty[i].Mode < 7)
				{
					CharacterParty[i].CurrentModeUp = 0;
					CharacterParty[i].Mode++;
                    CharacterValueUpdate(i);
					HealthReset(i);
				}
				else
				{
					CharacterParty[i].CurrentModeUp = CharacterParty[i].BaseModeUp*((int)CharacterParty[i].Mode+1);
				}
			}
		}
	}

    public void ModeDownCheck()
	{
		for (int i = 0; i < CharacterPartySize; i++)
		{
			if (CharacterParty[i].CurrentHealth <= 0)
			{
                if (CharacterParty[i].Mode > 0)
                {
                    CharacterParty[i].CurrentModeUp = 0;
                    CharacterParty[i].Mode--;
                    CharacterValueUpdate(i);
                    HealthReset(i);
                }
				else
                {
                    //sets mode up bar to 0, to prevent possible moding up while dead
                    CharacterParty[i].CurrentModeUp = 0;
				    CharacterParty[i].CurrentHealth = 0;
                }
			}
        }
	}

    public void HealthReset(int CharacterID)
	{
		CharacterParty[CharacterID].CurrentHealth = (CharacterParty[CharacterID].BaseHealth/8)*(8-(int)CharacterParty[CharacterID].Mode);
	}

    public void HealthLimit()
	{
		for (int i = 0; i < CharacterPartySize; i++)
		{
			if (CharacterParty[i].CurrentHealth > (CharacterParty[i].BaseHealth/8)*(8-(int)CharacterParty[i].Mode))
			{
				HealthReset(i);
			}
		}
	}

    private void ModeBarUpdate() //updates the mode bars
    {
        int Attacking = 0; //If the player is not currently attacking, the mode bars will remain in their initial position
        if (AttackingTeam) 
        {
            Attacking = 1;
        }

        for (int i = 0; i < CharacterPartySize; i++)
        {
            int ActiveChar = (i+(ActiveCharacter * Attacking)) % CharacterPartySize;
            ModeDisplays[i].UpdateModeDisplay(CharacterParty[ActiveChar]);
        } 
    }

    private void DetailDisplayUpdate() //update the team displays in the box at the bottom of the screen
    {
        //NOTE: CHANGE THIS LATER TO INCLUDE ARROWS THAT POINT TO THE BOXES. 
        CharacterStatus.AliveColour(CharacterParty, FriendlyTarget);
        EnemyStatus.AliveColour(EnemyParty, ActiveEnemy);
        CharacterStatus.StatusUpdate(CharacterParty);
        EnemyStatus.StatusUpdate(EnemyParty);
    }

    private void CharacterValueUpdate(int index)
    {
        CharacterParty[index].CurrentAttack = CharacterParty[index].BaseAttack * ((int)CharacterParty[index].Mode+1);
        for (int i = 0; i < CharacterParty[index].Commands.Length; i++)
        {
            CharacterParty[index].Commands[i].CastValue = CharacterParty[index].Commands[i].CastBaseValue * ((int)CharacterParty[index].Mode+1);
        } 
    }

    private List<StructsAndEnums.Status> AddStatus(List<StructsAndEnums.Status> list, StructsAndEnums.Status stat)
    {
        if (list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].StatusType == stat.StatusType)
                {
                    stat.StatusDuration = Mathf.Min(stat.StatusDuration + list[i].StatusDuration, 9);
                    list[i] = stat;
                    return list;
                }
            }
            if (stat.StatusType != StructsAndEnums.StatusType.Sleep && stat.StatusType != StructsAndEnums.StatusType.LuckUp)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if ((int)list[i].StatusType + (int)stat.StatusType == 11)
                    {
                        list.RemoveAt(i);
                    }
                }
            }
        }
        list.Add(stat);
        return list;
    }

    private void BattleVictory()
    {
        //reset cast cooldowns
        //copy player stats to CharacterDetails.cs
        //disable battle screen
    }

    private void TestingFunction()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CharacterParty[0].Status = AddStatus(CharacterParty[0].Status, new StructsAndEnums.Status(StructsAndEnums.StatusType.AttackUp, 1));
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            CharacterParty[0].Status = AddStatus(CharacterParty[0].Status, new StructsAndEnums.Status(StructsAndEnums.StatusType.AttackDown, 1));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CharacterParty[0].Status = AddStatus(CharacterParty[0].Status, new StructsAndEnums.Status(StructsAndEnums.StatusType.DefenceUp, 1));
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            CharacterParty[0].Status = AddStatus(CharacterParty[0].Status, new StructsAndEnums.Status(StructsAndEnums.StatusType.DefenceDown, 1));
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            CharacterParty[0].Status = AddStatus(CharacterParty[0].Status, new StructsAndEnums.Status(StructsAndEnums.StatusType.Sleep, 1));
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            CharacterParty[0].Status = AddStatus(CharacterParty[0].Status, new StructsAndEnums.Status(StructsAndEnums.StatusType.LuckUp, 1));
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            CharacterParty[0].Status = AddStatus(CharacterParty[0].Status, new StructsAndEnums.Status(StructsAndEnums.StatusType.Regenerate, 1));
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            CharacterParty[0].Status = AddStatus(CharacterParty[0].Status, new StructsAndEnums.Status(StructsAndEnums.StatusType.Dying, 1));
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            CharacterParty[0].Status = AddStatus(CharacterParty[0].Status, new StructsAndEnums.Status(StructsAndEnums.StatusType.ModeBoost, 1));
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            CharacterParty[0].Status = AddStatus(CharacterParty[0].Status, new StructsAndEnums.Status(StructsAndEnums.StatusType.ModeHinder, 1));
        }
    }
}
