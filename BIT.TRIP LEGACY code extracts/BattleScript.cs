using System.Collections.Generic;
using UnityEngine;
using static StructsAndEnums;
using static CharacterDetails;
using static EnemyDetails;
using static Inventory;
using static GameController;

public class BattleScript : MonoBehaviour
{
    public CharacterClass[] CharacterParty;
    public EnemyClass[] EnemyParty;
    public TeamStatusBar CharacterStatus, EnemyStatus;
    private bool AttackingTeam = true, OptionSelected = false, CastSelected = false, AttackingCast, ItemSelected = false, AttackingItem;
    private int OptionID = 0, CastID = 0, ItemIndex = 0;
    private float SelectDelay = 0.25f, AttackDelay = 1f;
    private int ActiveCharacter, ActiveEnemy, CharacterPartySize, EnemyPartySize, FriendlyTarget;

    public int[] EnemyPartyDetails;
    public GameObject OptionSelectMarker, CastMenu, ItemMenu, ModeUpWordCover;
    public ModeDisplay[] ModeDisplays;
    public DetailDisplay[] TextDisplays; //0th element is for active character, 1st is for enemy
    public CastDisplayManager CastDisplays; //For the Cast Command
    public ItemDisplayManager ItemDisplays;
    public bool CanFlee, CanLose;
    public float AttackModifier, LuckModifier, ModeModifier, HealthModifier;

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
        CharacterPartySize = Mathf.Min(CharacterDetails_.PartyDetails.Length, 4);
        CharacterParty = new CharacterClass[CharacterPartySize];
        for (int i = 0; i < CharacterPartySize; i++)
        {
            CharacterParty[i] = CharacterDetails_.GetCharacter(CharacterDetails_.PartyDetails[i]);
            CharacterParty[i].ValueUpdate();
            if (CharacterParty[i].Mode > GameController_.MaxMode)
            {
                CharacterParty[i].Mode = GameController_.MaxMode;
            }
            CharacterParty[i].HealthLimit();
        }
        CharacterStatus.SetTeamSize(CharacterPartySize);
        
    }
    
    private void GrabEnemyParty() //Grabs all the enemies for the fight
    {
        EnemyPartySize = EnemyPartyDetails.Length;
        EnemyParty = new EnemyClass[EnemyPartySize];
        for (int i = 0; i < EnemyPartySize; i++)
        {
            EnemyParty[i] = EnemyDetails_.GetEnemy(EnemyPartyDetails[i]);
            EnemyParty[i].ValueUpdate();
        }
        EnemyStatus.SetTeamSize(EnemyPartySize);
    }

    void Update()
    {
        if (DeadTeamCheck() == 0)
        {
            ShortenSelectDelay();
            if (AttackingTeam)
            {
                if (!DeadCheck() && !StatusHandler.Check(ref CharacterParty[ActiveCharacter].StatusList, StatusType.Sleep))
                {
                    PartyTurn();
                }
            }
            else
            {
                if (!DeadCheck() && !StatusHandler.Check(ref EnemyParty[ActiveEnemy].StatusList, StatusType.Sleep))
                {
                    EnemyTurn();
                }
            }
            TextDisplays[0].UpdatePlayerDisplay(CharacterParty[ActiveCharacter]);
            TextDisplays[1].UpdateEnemyDisplay(EnemyParty[ActiveEnemy]);
            DetailDisplayUpdate();
        }
        else if (DeadTeamCheck() == 1) //player team wins
        {
            BattleVictory();
        }     
        else //player team loses
        {
            BattleDefeat();
        }
    }

    private void PartyTurn() //Player performs their actions here
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
                    Defend();
                    break;
                }
                case 2: //cast
                {
                    Cast();
                    break;
                }
                case 3: //item
                {
                    Item();
                    break;
                }
                case 4: //flee
                {
                    if (CanFlee)
                    {
                        Flee();
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

    private void OptionSelect() //player selects an option from the left side
    {
        if (SelectDelay <= 0)
        {
            if (Input.GetAxis("Vertical") < 0 && OptionID < 4)
            {
                OptionID++;
                SelectDelay = 0.25f;
                OptionSelectMarker.transform.position -= new Vector3(0, 32, 0); //distance between option marker positions is 32
            }
            else if (Input.GetAxis("Vertical") > 0 && OptionID > 0)
            {
                OptionID--;
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
                if (EnemyParty[ActiveEnemy].CurrentHealth > 0)
                {   
                    AttackEnemy();
                }
            }
        }
    }

    private void Defend()
    {
        FortifyTarget(  ref CharacterParty[ActiveCharacter].DefenceBuffList,
                        Defence.OneTurn(CharacterParty[ActiveCharacter].CurrentDefence));
        StatusHandler.Add(ref CharacterParty[ActiveCharacter].StatusList, new Status(StatusType.DefenceUp, 1));
        AdvanceTurn();
    }

    private void Cast()
    {
        CastMenuToggle(true);
        CastDisplays.CastUpdate(ref CharacterParty[ActiveCharacter].Commands);
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
    }

    private void CastSelect() //player selects an option from the left side
    {
        CastDisplays.ColourChange(CastID);
        if (SelectDelay <= 0)
        {
           if (Input.GetAxis("Vertical") < 0 && CastID < CharacterParty[ActiveCharacter].Commands.Length-1)
            {
                SelectDelayReset();
                CastID++;
            }
            else if (Input.GetAxis("Vertical") > 0 && CastID > 0)
            {
                SelectDelayReset();
                CastID--;
            }
            else if (Input.GetAxis("Cancel") > 0)
            {
                SelectDelayReset();
                OptionSelected = false;
                CastMenuToggle(false);
            }
            else if (Input.GetAxis("Submit") > 0)
            {
                if (CharacterParty[ActiveCharacter].Commands[CastID].CurrentCooldown == 0)
                {
                    SelectDelayReset();
                    CastSelected = true;
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

    private void Item()
    {
        ItemMenuToggle(true);
        if (!ItemSelected)
        {
            ItemSelect();
        }
    }

    private void ItemSelect() //player selects an option from the left side
    {
        Debug.Log(ItemDisplays);
        ItemDisplays.UpdateDisplay(ItemIndex, ref Inventory_.Items);
        if (SelectDelay <= 0)
        {
           if (Input.GetAxis("Vertical") < 0 && ItemIndex < Inventory_.Items.Count-1)
            {
                SelectDelayReset();
                ItemIndex++;
            }
            else if (Input.GetAxis("Vertical") > 0 && ItemIndex > 0)
            {
                SelectDelayReset();
                ItemIndex--;
            }
            else if (Input.GetAxis("Cancel") > 0)
            {
                SelectDelayReset();
                OptionSelected = false;
                ItemMenuToggle(false);
            }
            else if (Input.GetAxis("Submit") > 0)
            {
                // if (CharacterParty[ActiveCharacter].Commands[CastID].CurrentCooldown == 0)
                // {
                //     SelectDelayReset();
                //     CastSelected = true;
                // }
            }
        }
    }

    private void Flee()
    {
        OptionSelected = false; //allow the player to choose another option
        //while this isn't implemented, the player can't select this option
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

    private void EnemyTurn() //automatically attacks the player party at random
    {
        //this will be reworked later to include casting and defending

        if (AttackDelay > 0)
        {
            AttackDelay -= Time.deltaTime;
        }
        else
        {
            ActiveCharacter = Random.Range(0, CharacterPartySize);
            int TotalDamage = TotalDamage = CalculateDamage(CharacterParty[ActiveCharacter].CurrentDefence, EnemyParty[ActiveEnemy].CurrentAttack, 0); 
            AttackTarget(ref CharacterParty[ActiveCharacter].CurrentHealth, TotalDamage);
            CharacterParty[ActiveCharacter].ModeDownCheck();
            ModeBarUpdate();
            AdvanceTurn(); 
            AttackDelay = 1f;
        }
    }

    private void AttackEnemy()
    {
        int TotalDamage = 0;
        if (OptionID == 2) //cast
        {
            //casts bypass defence, but the values can't be influenced by attack.up or attack.down statuses
            CastCooldown(ref CharacterParty[ActiveCharacter].Commands[CastID]);
            StatusHandler.Add(ref EnemyParty[ActiveEnemy].StatusList, CharacterParty[ActiveCharacter].Commands[CastID].Status);
            TotalDamage = CalculateDamage(EnemyParty[ActiveEnemy].CurrentDefence, CharacterParty[ActiveCharacter].Commands[CastID].CastValue, 0);
        }
        else //ordinary attack
        {
            TotalDamage = CalculateDamage(EnemyParty[ActiveEnemy].CurrentDefence, CharacterParty[ActiveCharacter].CurrentAttack, CharacterParty[ActiveCharacter].CurrentLuck);
        }
        AttackTarget(ref EnemyParty[ActiveEnemy].CurrentHealth, TotalDamage);
        AddModeUp(ref CharacterParty[ActiveCharacter].CurrentModeUp, ref CharacterParty[ActiveCharacter].StatusList, TotalDamage);
        CharacterParty[ActiveCharacter].ModeUpCheck();
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
                if (CharacterParty[ActiveCharacter].Commands[CastID].CastType == CastType.Heal)
                {
                    CastCooldown(ref CharacterParty[ActiveCharacter].Commands[CastID]);
                    HealTarget(ref CharacterParty[FriendlyTarget].CurrentHealth, CharacterMaxHealth(FriendlyTarget), CharacterParty[ActiveCharacter].Commands[CastID].CastValue);
                    StatusHandler.Add(ref CharacterParty[FriendlyTarget].StatusList, CharacterParty[ActiveCharacter].Commands[CastID].Status);
                    CharacterParty[FriendlyTarget].ValueUpdate();
                    CharacterParty[ActiveCharacter].HealthLimit();
                    ModeBarUpdate();
                    AdvanceTurn();
                }
                else if (CharacterParty[ActiveCharacter].Commands[CastID].CastType == CastType.Fortify)
                {
                    CastCooldown(ref CharacterParty[ActiveCharacter].Commands[CastID]);
                    FortifyTarget(ref CharacterParty[FriendlyTarget].DefenceBuffList, new Defence(CharacterParty[ActiveCharacter].Commands[CastID].CastValue, CharacterParty[ActiveCharacter].Commands[CastID].Status.StatusDuration));
                    StatusHandler.Add(ref CharacterParty[FriendlyTarget].StatusList, CharacterParty[ActiveCharacter].Commands[CastID].Status);
                    CharacterParty[FriendlyTarget].ValueUpdate();
                    AdvanceTurn();
                }
            }
        }
    }

    private void AdvanceTurn() //Does exactly what it says on the tin
    {
        if (AttackingTeam)
        {
            PostActionInfliction(ref CharacterParty[ActiveCharacter].StatusList, ref CharacterParty[ActiveCharacter].CurrentHealth);
            CharacterParty[ActiveCharacter].ModeDownCheck();
            ModeBarUpdate();
            if (ActiveCharacter+1 == CharacterPartySize)
            {
                SwapActiveTeam();
            }
            else
            {
                ActiveCharacter++;
                CharacterParty[ActiveCharacter].ValueUpdate();
                CastCooldownDecrease(ref CharacterParty[ActiveCharacter].Commands);
                StatusHandler.Decrease(ref CharacterParty[ActiveCharacter].StatusList);
            }
            OptionSelected = false;
            CastSelected = false;
            CastMenuToggle(false);
            ItemMenuToggle(false);
            OptionSelectMarker.transform.position += new Vector3(0, 32*OptionID, 0); //moves the option select marker back to ATTACK
            OptionID = 0;
            CastID = 0;
            FriendlyTarget = ActiveCharacter;
        }
        else
        {
            PostActionInfliction(ref EnemyParty[ActiveEnemy].StatusList, ref EnemyParty[ActiveEnemy].CurrentHealth);
            if (ActiveEnemy+1 == EnemyPartySize)
            {
                SwapActiveTeam();
            }
            else
            {
                ActiveEnemy++;
                EnemyParty[ActiveEnemy].ValueUpdate();
                CastCooldownDecrease(ref EnemyParty[ActiveEnemy].Commands);
                StatusHandler.Decrease(ref EnemyParty[ActiveEnemy].StatusList);
            }
        }
        SelectDelayReset();
        ModeBarUpdate();
    }

    private void SwapActiveTeam()
    {
        ActiveCharacter = 0;
        ActiveEnemy = 0;
        AttackingTeam = !AttackingTeam;
        if (AttackingTeam)
        {
            CharacterParty[ActiveCharacter].ValueUpdate();
            CastCooldownDecrease(ref CharacterParty[ActiveCharacter].Commands);
            StatusHandler.Decrease(ref CharacterParty[ActiveCharacter].StatusList);
        }
        else
        {
            EnemyParty[ActiveEnemy].ValueUpdate();
            CastCooldownDecrease(ref EnemyParty[ActiveEnemy].Commands);
            StatusHandler.Decrease(ref EnemyParty[ActiveEnemy].StatusList);
            
        }
    }

    private bool DeadCheck() //checks if the active character or enemy is dead
    {
        if (AttackingTeam && CharacterParty[ActiveCharacter].Mode == 0 && CharacterParty[ActiveCharacter].CurrentHealth == 0)
        {
            AdvanceTurn();
            return true;
        }
        else if (!AttackingTeam && EnemyParty[ActiveEnemy].CurrentHealth == 0)
        {
            AdvanceTurn();
            return true;
        }
        return false;
    }

    private int DeadTeamCheck()
    {
        int CharacterPartyHealth = 0, EnemyPartyHealth = 0;
        for (int i = 0; i < CharacterPartySize; i++)
        {
            CharacterPartyHealth += CharacterParty[i].CurrentHealth + (int)CharacterParty[i].Mode;
            //modes count as extra health for this calculation
            //if the player party is at NETHER with 0 health, the result is 0.
        }
        for (int i = 0; i < EnemyPartySize; i++)
        {
            EnemyPartyHealth += EnemyParty[i].CurrentHealth;
        }
        if (EnemyPartyHealth == 0)
        {
            return 1;
        }
        else if (CharacterPartyHealth == 0)
        {
            return 2;
        }
        return 0;
    }

    private int CharacterMaxHealth(int index)
    {
        return (CharacterParty[index].BaseHealth/8)*(8-(int)CharacterParty[index].Mode);
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
            ModeDisplays[i].UpdateModeDisplay(ref CharacterParty[ActiveChar]);
            if (ActiveChar == 0)
            {
                if (CharacterParty[ActiveChar].Mode == GameController_.MaxMode)
                {
                    ModeUpWordCover.SetActive(true);
                }
                else if (CharacterParty[ActiveChar].Mode < GameController_.MaxMode)
                {
                    ModeUpWordCover.SetActive(false);
                }
            }
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

    private void BattleVictory()
    {
        for (int i = 0; i < CharacterPartySize; i++)
        {
            StatusHandler.Reset(ref CharacterParty[i].StatusList);
        }

        //disable battle screen
    }

    private void BattleDefeat()
    {

    }


    private int CalculateDamage(int Defence, int Damage, int Luck)
    {
        if (Luck > 0)
        {
            float Randomiser = Random.Range(0f, 100f);
            if (Luck > Randomiser)
            {
                Defence = 0;
            }
        }
        return Mathf.Max(Damage-Defence, 0);
    }

    private void AttackTarget(ref int Health, int Damage) //needs reworking to include mode up
    {   
        Health = Mathf.Max(Health-Damage, 0);
    }

    private void HealTarget(ref int Health, int MaxHealth, int HealValue)
    {
        Health = Mathf.Max(Health+HealValue, MaxHealth);
    }

    private void FortifyTarget(ref List<Defence> DefenceBuffList, Defence NewDefenceBoost)
    {
        DefenceBuffList.Add(NewDefenceBoost);
    }

    private void FortifyDecrease(ref List<Defence> DefenceBuffList)
    {
        for (int i = 0; i < DefenceBuffList.Count; i++)
        {
            if (DefenceBuffList[i].DefenceLength <= 1)
            {
                DefenceBuffList.RemoveAt(i);
                i--;
            }
            else
            {
                Defence Temp = DefenceBuffList[i];
                Temp.DefenceLength--;
                DefenceBuffList[i] = Temp; 
            }
        }
    }

    private void CastCooldown(ref CastCommand Cast)
    {
        Cast.CurrentCooldown = Cast.MaxCooldown;
    }

    private void CastCooldownDecrease(ref CastCommand[] Casts)
    {
        for (int i = 0; i < Casts.Length; i++)
        {
            if (Casts[i].CurrentCooldown > 0)
            {
                Casts[i].CurrentCooldown--;
            }
        }
    }

    private void CastMenuToggle(bool State)
    {
        CastMenu.SetActive(State);
        CastDisplays.gameObject.SetActive(State);
    }

    private void ItemMenuToggle(bool State)
    {
        ItemMenu.SetActive(State);
        ItemDisplays.gameObject.SetActive(State);
    }

    private void AddModeUp(ref int CurrentModeUp, ref List<Status> StatusList, int TotalDamage)
    {
        float ModeMultiplier = 1;
        if (StatusHandler.Check(ref StatusList, StatusType.DefenceUp))
        {
            ModeMultiplier += GameController_.ModeModifier;
        }
        else if (StatusHandler.Check(ref StatusList, StatusType.DefenceDown))
        {
            ModeMultiplier -= GameController_.ModeModifier;
        }
        CurrentModeUp += (int)(TotalDamage*ModeMultiplier);
    }

    private void PostActionInfliction(ref List<Status> StatusList, ref int Health)
    {
        if (StatusHandler.Check(ref StatusList, StatusType.Dying))
        {
            Health -= Mathf.Max(1, (int)(Health*GameController_.HealthModifier));
        }
        else if (StatusHandler.Check(ref StatusList, StatusType.Regenerate))
        {
            Health += Mathf.Max(1, (int)(Health*GameController_.HealthModifier));
        }
    }
}
