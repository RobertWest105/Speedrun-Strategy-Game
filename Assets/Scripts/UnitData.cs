using System;

//A class to store unit data to be saved
[Serializable]
public class UnitData {

    public SerializableCoord position { get; private set; }
    public string unitClass { get; private set; }
    public int hp { get; private set; }
    public bool twoPlayer { get; private set; }
    
    public UnitData(Unit unit) {
        position = new SerializableCoord(unit.position);
        unitClass = unit.unitClass;
        hp = unit.hp;
        twoPlayer = GameManager.twoPlayer;
    }
}