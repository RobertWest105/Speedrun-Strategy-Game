using UnityEngine;
using System.Collections.Generic;

public abstract class Unit : MonoBehaviour {

    public static Unit selected { get; protected set; }

    public static List<Unit> movedAllies = new List<Unit>();
    public static List<Unit> movedEnemies = new List<Unit>();

    //Stats
    public string unitClass { get; protected set; }
    public int maxHp { get; protected set; }
    public int hp { get; protected set; }
    public int atk { get; protected set; }
    public int baseDef { get; protected set; }
    public int def { get; protected set; }
    public int spd { get; protected set; }
    public int mov { get; protected set; }
    public int atkRange { get; protected set; }
    public Coord position { get; protected set; }

    public void setHp(int hpValue) {
        hp = hpValue;
    }

    public Dictionary<Coord, Tile> allTiles { get; protected set; }
    public Dictionary<Coord, int> validSpaces { get; private set; }

    //Essentially a constructor
    public void setupUnit(string unitCls, Coord startPos) {
        unitClass = unitCls;
        if(unitClass.Contains("Enemy")) {
            unitCls = unitCls.Remove(unitCls.IndexOf("Enemy"));
        }
        switch(unitCls) {
            case "Fighter":
                //hp, atk, def, spd, mov
                assignStats(22, 20, 10, 14, 6);
                break;
            case "Archer":
                assignStats(20, 14, 8, 18, 6);
                break;
            case "Mage":
                assignStats(18, 18, 8, 16, 6);
                break;
            case "Cavalier":
                assignStats(26, 16, 10, 20, 8);
                break;
            case "Knight":
                assignStats(24, 14, 16, 6, 5);
                break;
            case "Healer":
                assignStats(20, 12, 6, 18, 6);
                break;
        }
        position = startPos;
        if(unitClass.StartsWith("Archer") || unitClass.StartsWith("Mage") || unitClass.StartsWith("Healer")) {
            atkRange = 2;
        } else atkRange = 1;
    }

    void assignStats(int _maxHp, int _atk, int _baseDef, int _spd, int _mov) {
        hp = maxHp = _maxHp;
        atk = _atk;
        def = baseDef = _baseDef;
        spd = _spd;
        mov = _mov;
    }

    //Recursively find all valid spaces for a unit to move to or attack (depending on the bool paramater)
    public virtual void findValidSpaces(int move, Coord currentSpace, int callNo, bool attack) {
        if(callNo == 0) {
            validSpaces = new Dictionary<Coord, int>();
        }
        if(move < 0) return;
        if(move == 0) {
            if(validSpaces.ContainsKey(currentSpace)) {
                validSpaces[currentSpace] = move;
            } else {
                validSpaces.Add(currentSpace, move);
            }
            return;
        }

        if(validSpaces.ContainsKey(currentSpace)) {
            validSpaces[currentSpace] = move;
        } else {
            validSpaces.Add(currentSpace, move);
        }

        //Look at all compass direction neighbours of currentSpace
        for(int x = currentSpace.X - 1; x <= currentSpace.X + 1; x++) {
            for(int y = currentSpace.Y - 1; y <= currentSpace.Y + 1; y++) {
                Coord nextSpace = new Coord(x, y);
                //Only consider a space if it isn't in validSpaces
                //XOR (it's in validSpaces AND it's been reached by going through less spaces than before)
                if(!validSpaces.ContainsKey(nextSpace) != (validSpaces.ContainsKey(nextSpace) && move > validSpaces[nextSpace])) {
                    if(x == currentSpace.X || y == currentSpace.Y) {
                        int moveCost = MapGenerator.allTiles[nextSpace].moveCost;
                        if(attack) {
                            //If a mountain is a neighbour, ignore it as the player wants to attack a unit
                            //and no units can be on mountains
                            if(moveCost > 2) continue;
                            //Recursive call with 1 less move as attacks can go through trees
                            //without reducing attack range
                            findValidSpaces(move - 1, nextSpace, callNo + 1, attack);
                        } else {
                            //Recursive call with the movement the unit would have if it moved to nextSpace
                            findValidSpaces(move - moveCost, nextSpace, callNo + 1, attack);
                        }
                    }
                }
            }
        }
    }

    //Return all highlighted spaces to normal
    public void hideValidSpaces() {
        foreach(Coord validSpace in validSpaces.Keys) {
            Tile t = MapGenerator.allTiles[validSpace];
            t.GetComponent<SpriteRenderer>().sprite = t.normal;
        }
        validSpaces.Clear();
        InGameMenus.attacking = false;
    }

    //Move a unit to a new space
    public virtual void moveUnit(Coord destination) {
        position = destination;
        transform.position = destination.worldPos;
        hideValidSpaces();
    }

    //Get stats of selected unit to be displayed on the stats screen
    public string[] getStats() {
        string[] stats = new string[8];
        //gather all stats of this unit
        stats[0] = hp.ToString();
        stats[1] = atk.ToString();
        stats[2] = def.ToString();
        stats[3] = spd.ToString();
        stats[4] = mov.ToString();
        stats[5] = atkRange.ToString();
        stats[6] = this.GetType() == typeof(Ally) ? "Ally" : "Enemy";
        stats[7] = unitClass.Contains("Enemy") ? unitClass.Remove(unitClass.IndexOf("Enemy")) : unitClass;
        return stats;
    }

    //Shows the attack range of a unit if player chooses to attack
    public void selectTarget() {
        //Debug.Log("Select a target");

        allTiles = MapGenerator.allTiles;
        findValidSpaces(atkRange, position, 0, true);
        allTiles[position].showMovableSpaces(validSpaces, this);
    }

    //Handle attacks and counter-attacks
    public void attack(Unit target, bool counter) {
        //calculate hit percentage
        int hitChance = 60;

        //Proportional to spd
        if(this.spd < target.spd) {
            hitChance -= target.spd - (this.spd < target.spd - 5 ? 5 : 0);
        }else if (this.spd > target.spd){
            hitChance += spd + (this.spd > target.spd + 5 ? 5 : 0);
        }

        //Inversely proportional to atk
        if(atk > target.atk) {
            hitChance -= target.atk - atk;
        }else if(atk < target.atk) {
            hitChance += atk - target.atk;
        }

        //Ensure hitChange is a percentage
        hitChance = Mathf.Clamp(hitChance, 0, 100);

        //Debug.Log("Hit chance is: " + hitChance);

        //Change def values of battling units if they're on forest tiles
        if(!counter) {
            if(MapGenerator.allTiles[position].moveCost == 2) {
                def++;
            } else def = baseDef;
            if(MapGenerator.allTiles[target.position].moveCost == 2) {
                target.def++;
            } else target.def = target.baseDef;
        }

        //Carry out the attack
        System.Random hit = new System.Random(Time.realtimeSinceStartup.ToString().GetHashCode());
        if(hit.Next(0, 100) < hitChance) {
            //If the attack hit, the enemy takes damage
            int dmg = atk < target.def ? 0 : (atk-target.def);
            target.takeDamage(dmg);
        }

        //If the defending unit can attack back, do so
        if(!counter && atkRange == target.atkRange) {
            target.attack(this, true);
        }

        //Check if this attack was the last thing the player did in their turn
        if(counter && this.GetType() == typeof(Enemy)) {
            if(!GameManager.twoPlayer && !GameManager.instance.checkTurnEnd()) {
                for(int i = 0; i < GameManager.instance.enemies.Count; i++) {
                    GameManager.instance.enemies[i].AI();
                }
            }
        }
    }

    //Reduce the unit's hp by damage amount and check if they're defeated by this
    public void takeDamage(int damage) {
        hp -= damage;
        //Debug.Log(this.unitClass + " took " + damage + " damage and now has " + hp + " hp left");
        if(hp <= 0) {
            defeated();
        }
    }

    //Remove the unit from the game if its hp <= 0
    public virtual void defeated() {
        Destroy(this.gameObject);
    }

    //Delete all units before spawning the units from a loaded save
    public static void deleteUnits() {
        GameObject[] unitsToDelete = GameObject.FindGameObjectsWithTag("Unit");
        foreach(GameObject unit in unitsToDelete) {
            Destroy(unit);
        }
    }
}