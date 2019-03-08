using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenger:MonoBehaviour {
    public Faction faction;
    public string name;
    private Personality personality;   
    private IStructure[] properties;
    private List<Unit> army;
    public ArmyScrollList armyScrollList;
    private List<Item> items;
    private Unit challenger; // this is the main character as a unit
    [SerializeField]
    public GameObject city; //closest city to set location on initialisation
    [SerializeField] public IInteractable location;
   // [SerializeField]
    //private ArmyScrollList armyList; //set in inspector
    private Vector3 headingTowards; //maybe as a string, used to communicate to player
    private int reputation; // maybe do this as a matrix per faction. 
    private int fame;
    // army related vars
    public int maxArmySize = 10;
    public int currentArmySize;
    private int armyMorale;
    private int armyUpkeep;
    private int strengthValue;
    [SerializeField] Sprite playerIcon;
    public Challenger interactingWith; // this is null usually
    public int gold = 10;
    private ArmyScrollList asl;
    //refernce to object spawner to access the 2 factions
    public ObjSpawner spawnSettings;
    
    public void SetFaction( Faction fac) {
        this.faction = fac;
    }
   public int GetMaxArmySize() {
        return maxArmySize;
    }
    public void SetMaxArmySize(int size) {
        maxArmySize = size;
    }
    public int CalculateMaxArmySize() {
        //use stats
        return 10;
    }

    //returns if recruits refillable
    public bool HasRefillableRecruits() {
        if (army.Count == 0)
            return false;
        foreach(Unit u in army) {
            if( u.currentSize < u.maxSize && u.unitRank == UnitRank.RECRUIT) { //refills recruits
                return true;
            }
        }
        return false;
    }
    //returns number of recruits to refill
    public int RefillableRecruits() {
        int count = 0;
        foreach (Unit u in army) {
            if (u.currentSize < u.maxSize && u.unitRank == UnitRank.RECRUIT) { //hardcoded check here to see level AKA if it is a base recruit 
                count += (u.maxSize - u.currentSize);
            }
        }
        return count;
    }

    //returns list of amount of recruits needed for refill per unit in player army
    public List<int> RecruitRefillList( List<Unit> list) {
        List<int> ret = new List<int>();
        foreach(Unit u in list) {
            if (u.currentSize < u.maxSize && u.unitRank == UnitRank.RECRUIT) { //hardcoded check here to see level AKA if it is a base recruit 
                ret.Add(u.maxSize - u.currentSize);
            }
        }
        return ret;
    }

    public int GetArmyTroopCount() {
        int counter = 0;
        foreach( Unit u in army) {
          counter += u.currentSize;
        }
        return counter;
    }

    public  int GetArmyPower() {
        int counter = 0;
        checked { // int overflow is happening here somehow??
            foreach (Unit u in army) {
                counter += (u.currentSize * u.power);
            }

            //challenger.power = counter;
        }
        Debug.Log("army power for " + name + " is " + counter);
        return counter;
    }

    //used to set the pie chart values for melee / ranged / cavalry
    public float[] GetTroopTypeRatios() {
        float totalTroops = GetArmyTroopCount();
        float[] ratios = new float[3];

        foreach (Unit u in army) {
            UnitType type = u.unitType;
            switch (type) {
                case (UnitType.MELEE):
                    ratios[0] += (float) u.currentSize;
                    break;
                case (UnitType.RANGED):
                    ratios[1] += (float) u.currentSize;
                    break;
                case (UnitType.CAVALRY):
                    ratios[2] += (float) u.currentSize;
                    break;
            }
        }
        //normalise
        for(int i =0; i< ratios.Length; i++) {
           ratios[i] = ratios[i] / totalTroops;
        }
        //find the end point for sections 2 and 3
        ratios[1] = ratios[0] + ratios[1];
        ratios[2] = ratios[1] + ratios[2];
        return ratios;
    }
    private void Awake() {
        //need to keep track tho?? 
        //faction = new Faction();
        //create player unit with player icon and add to army (removed atm)
        challenger = new Unit(playerIcon,1); //challenger is 1 unit recruit for debug testing
        army = new List<Unit>();
        items = new List<Item>();
        army.Add(challenger);
    }
    private void Start() {
        location = city.GetComponent<IInteractable>();
    }
    public Challenger (Faction fac, List<Unit> army, Unit self) {
        fac.AddFactionMember(this);
        this.faction = fac;
        challenger = self;
        this.army = army;
        army.Add(self);
        
    }

    public bool AddUnit(Unit unit) {
        maxArmySize = CalculateMaxArmySize(); //recalculate max army size
        currentArmySize = army.Count;
        if (currentArmySize >= maxArmySize) return false;
        army.Add(unit);
        return true;
    }

    public List<Unit> GetArmy() {
        return army;
    }

    //removes troops from own army and enemy army
    public void RemoveTroopCasualties(Challenger enemy) {
        float playerLossPerc = ArmyLossCalculation(enemy.army) / 100;
        float enemyLossPerc = (1 - playerLossPerc);
     
        ListRemover(army, playerLossPerc);

        ListRemover(enemy.army, enemyLossPerc);
    }

    private void ListRemover(List<Unit> listToRemove, float lossPercentage) {
        List<int> iToRemove = new List<int>(); // this is pointing to the same reference in memory so have to create a new list
        int counter = 0;
        foreach (Unit u in listToRemove) {
            int amountRemaining = Mathf.RoundToInt(u.currentSize - (lossPercentage * u.maxSize));
            if (amountRemaining <= 0) {
                // remove this unit after iteration
                iToRemove.Add(counter);
            }
            else {
                army[counter].currentSize = amountRemaining;
            }
            counter++;
        }
        foreach (int i in iToRemove) {
            listToRemove.RemoveAt(i);
        }
    }

    //removes troops from own army in the case where there isnt an enemy challenger eg taking structures. defending army will be totally destroyed
    public void RemoveTroopCasualties(List<Unit> defendingArmy) {
        float playerLossPerc = ArmyLossCalculation(defendingArmy) / 100;
        //remove troops from player army
        ListRemover(army, playerLossPerc);
        //defending army losses handled elsewhere: will be 100% fatal
       

    }

    //TODO take into account troop types and counters
    //returns percentage loss of this army, enemy army loss is symmetric so if this army loses 30% enemy loses 30%,
    //                  [assuming lossonequalstrength = 30]             if this army loses 15% enemy army loses 45%
    public int ArmyLossCalculation(List<Unit> comparisonArmy) {
        float numbersAdvantage = 1.2f;
        float totalPower = GetArmyPower();
        float enemyTotalPower = 0;
        float lossOnEqualStrength = 30;


        if (comparisonArmy == null) { // no defending army such as a village, shouldnt occur normally
            return 0;
        }
        foreach (Unit u in comparisonArmy) {
            enemyTotalPower += (u.power * u.currentSize);
        }
        //size difference multiplier impacts morale of army just before and during the fight
        if ( ( (float)army.Count / (float)comparisonArmy.Count ) < 0.3) {
            enemyTotalPower = Mathf.RoundToInt(numbersAdvantage * totalPower);
        } //if enemy army is smaller
        else if ( ( (float)comparisonArmy.Count / (float)army.Count ) < 0.3) {
            totalPower = Mathf.RoundToInt(numbersAdvantage * enemyTotalPower);
        } //otherwise its the same as before
        //graph is y = - x + lossonequalstrength, where lossonequal strength is the cap for the x axis aka max ratio, y = player loss
        float x = RatioCalculation(totalPower, enemyTotalPower);
        //restrict x domain as well
        x = Mathf.Clamp(x, -100, 100); 
        Debug.Log("Player army loss percentage : " + x);
        int y = Mathf.Clamp( (int)(-x + lossOnEqualStrength), 0, 100);//making sure number between 0 and 100
        Debug.Log("loss for attacking challenger is : " + y);
        return y;
       
    }

    //returns true if the opponent is weaker than you
    public bool StrongerThanOpponent(List<Unit> comparisonArmy) {
        if (comparisonArmy == null) { // no defending army such as a village
            return true;
        }
        float numbersAdvantage = 1.2f;
        float totalPower = GetArmyPower();
        float enemyTotalPower = 0;
       
        foreach (Unit u in comparisonArmy) {
            enemyTotalPower += (u.power *u.currentSize);
        }
        //size difference multiplier impacts morale of army just before and during the fight
        if ( (float) army.Count / (float) comparisonArmy.Count < 0.3) {
            enemyTotalPower = Mathf.RoundToInt(numbersAdvantage * totalPower);
        }
        else if ( (float) comparisonArmy.Count / (float) army.Count < 0.3) {
            totalPower = Mathf.RoundToInt(numbersAdvantage* enemyTotalPower);
        } //otherwise its the same as before


        if( totalPower > enemyTotalPower) {
            return true;
        } else {
            return false;
        }
    }

    //returns negative ratio as int if enemy is stronger, rather than a fraction 
    private int RatioCalculation( float power1, float power2) {
        if( power1 >= power2) {
            return Mathf.RoundToInt( (float) power1 / (float) power2);
        }
        else {
            return Mathf.RoundToInt(-(float) power2 / (float)power1);
        }
    }
    // changing army scroll list to different town
    public void ChangeOtherArmyList(ArmyScrollList newList) {
        gameObject.GetComponent<ArmyScrollList>().otherList = newList;
    }

    // this needs to be rewritten 
    public void CombatCalculation(Challenger aggressor) {
    }
   } 





[System.Serializable] //to show up in inspector
public class Faction {
    public string factionName;
    public List<IStructure> structures;
    public List<Village> villages;
    public List<Town> towns;
    public Town capital;
    public List<Challenger> challengers;
    private Challenger leader;
    public int startingStrength;
    private WarSituation warSituation;
    private CurrentOrders currentlyDoing;
    public Color colour;

    //THIS OVERWRITES THE INSPECTOR VALUESr??
    /* public Faction() {
         factionName = "Sukis Fac OP";
     }*/

    public void SetChallengersToFaction() {
        foreach( Challenger c in challengers) {
            c.faction = this;
        }
    }

    public Faction(string name, List<IStructure> cities, int strength) {
        factionName = name;
        this.structures = cities;
        startingStrength = strength;
    }

    public Faction( string name, List<IStructure> cities, List<Challenger> challengers, Challenger leader ) {
        factionName = name;
        this.structures = cities;
        this.challengers = challengers;
        this.leader = leader;

    }
    public void AddFactionMember( Challenger newMem) {
        challengers.Add(newMem);
    }
    //todo remove
    public Faction(string facName) {
        factionName = facName;
    }
    public string GetFactionName() {
        return factionName;
    }
    public void SetFactionName(string newName) {
        factionName = newName ;
    }

}
public enum Personality {
    Brave, Angry, Happy, Fearful, Decietful, Honest

}

public enum CurrentOrders {
    Explore, Regroup, Invade, Grow
}
public enum WarSituation {
    Aggressive, Equal, Passive
}

//cant have enums in interfaces
public enum BuildingType {
    Capital, Town, Village, Outpost
}

public interface IStructure {
    
    void SetBuildingType(BuildingType type);
    BuildingType GetBuildingType();
    int NetIncome();
    string GetBuildingName();
    Faction GetFaction();
    Color GetTextColour();
    Transform GetPosition();


}

//special type of challenger, more powerful/specialised and follows orders of the faction leader
public interface IGeneral {
    WarSituation GetWarSituation();
}

//leader of factions, most powerful can delegate. special type of general
public interface ILeader: IGeneral{
    Challenger[] GetGenerals();
    CurrentOrders GetCurrentOrder();
}

//instance class REPLACED 
/* public class Unit : IHaveCStats {
    
   public Sprite icon;
   public int Health { get; set; }
   public int MaxHealth { get; set; }
   public string name = "default_unit";
    
   public Unit (Sprite icon) {
        this.icon = icon;
    }
   public int ModifyHealth(int amount) {
       Health = Mathf.Clamp(Health + amount, 0, MaxHealth);
       return Health;
    }
    public int STR { get; }
    public int STA { get; }
    public int CON { get; }
    public int WIS { get; }
    public int AGI { get; }
    public int CHAR { get; }
    public int LUK { get; }
    public int CRT { get; }
    public int DGE { get; }
    public int BLK { get; }
    public int AP { get; }

    
} */

//aka destroyable/killable. used for destructable things such as sheilds/ palisades as well as units
public interface IHaveHealth {
    int Health { get; set; }
    int MaxHealth { get; }
    int ModifyHealth(int amount);

}

//stats used in out of combat situations (nc = non combat)
// strength, stam, cons, wisdom, agility, charisma, luck
public interface IHaveNCStats {
    int STR { get; }
    int STA { get; }
    int CON { get; }
    int WIS { get; }
    int AGI { get; }
    int CHAR { get;}
    int LUK { get; }
    
}
//crit, dodge, block, action/ability points
public interface IHaveCStats : IHaveHealth, IHaveNCStats {
    int CRT { get; }
    int DGE { get; }
    int BLK { get; }
    int AP { get; }
}

public abstract class Item {
    int saleValue;
    int buyValue;
    int weight;
    bool consumble;

}
public interface IWeapon {
    int DealDamage(Unit attacker, IHaveHealth defender);
}


