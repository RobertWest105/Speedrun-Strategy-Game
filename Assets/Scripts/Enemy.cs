using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public class Enemy : Unit {

    List<Coord> occupiedSpaces = new List<Coord>();

    public void AI() {
        List<Ally> allies = GameManager.instance.allies;
        Ally target = decideTarget(allies);

        if(validSpaces != null) {
            hideValidSpaces();
        }

        //Debug.Log("moving towards " + target.unitClass);

        moveTowards(target);
        movedEnemies.Add(this);
        GameManager.instance.checkTurnEnd();
    }

    //Make the enemy team target the weakest ally unit
    //based on this enemy's hp and atk stats
    //and each ally's hp and def stats
    Ally decideTarget(List<Ally> allies) {
        Ally target = allies[0];
        float maxHeuristic = 0;
        for(int i = 0; i < allies.Count; i++) {
            float heuristic = (hp * atk) / (allies[i].hp * allies[i].def);
            if(heuristic > maxHeuristic) {
                maxHeuristic = heuristic;
                target = allies[i];
            }
        }
        return target;
    }

    //Move closer to target ally unit, then attack it
    //If a different ally is in this enemy's attack range, attack it
    void moveTowards(Ally target) {
        findOccupiedSpaces();
        findValidSpaces(mov, position, 0, false);
        Ally allyInRange = findAllyInRange(validSpaces);
        //If target is in range, move near it and attack it
        if(validSpaces.ContainsKey(target.position)) {
            randomMoveAndAttack(target);
        } else if(allyInRange != null){
            //If target isn't in range but another ally is, attack it
            randomMoveAndAttack(allyInRange);
        } else {
            //check all edge-of-range tiles to find closest to target and move here
            //must add 1 for every forest and make it 'infinite' (999) if any mountains are in the way

            Coord destination = new Coord();
            int minDist = 999;
            Dictionary<Coord, int> distances = new Dictionary<Coord, int>();
            int x = target.position.X;
            int y = target.position.Y;

            //Fill distances with the distance from each movable space to target
            foreach(Coord space in validSpaces.Keys) {
                int distanceToTarget = Math.Abs(x - space.X) + Math.Abs(y - space.Y) + MapGenerator.allTiles[space].moveCost;
                if(distanceToTarget < minDist) {
                    minDist = distanceToTarget;
                }

                //Only add space to distances if it's unoccupied
                if(!occupiedSpaces.Contains(space)) {
                    distances.Add(space, distanceToTarget);
                }
            }

            //Find space with minDist
            foreach(Coord space in distances.Keys) {
                if(distances[space] == minDist) {
                    destination = space;
                }
            }

            //Only move if there exists a destination for which distance == minDist
            if(!destination.Equals(new Coord())) {
                moveUnit(destination);
            }
        }
    }

    //Fill the occupiedSpaces list with all currently occupied spaces
    void findOccupiedSpaces() {
        List<Ally> allies = GameManager.instance.allies;
        List<Enemy> enemies = GameManager.instance.enemies;
        occupiedSpaces = new List<Coord>();
        for(int i = 0; i < allies.Count; i++) {
            occupiedSpaces.Add(allies[i].position);
        }
        for(int i = 0; i < enemies.Count; i++) {
            occupiedSpaces.Add(enemies[i].position);
        }
    }

    //Move to a random space from which target can be attacked, then attack
    void randomMoveAndAttack(Ally target) {
        List<Coord> possibleMoves = new List<Coord>();
        System.Random randomMove = new System.Random(Time.realtimeSinceStartup.ToString().GetHashCode());
        List<Coord> checkedPossibleMoves = new List<Coord>();
        if(atkRange == 1) {
            //move adjacent to target and attack
            possibleMoves.Add(new Coord(target.position.X + 1, target.position.Y));
            possibleMoves.Add(new Coord(target.position.X - 1, target.position.Y));
            possibleMoves.Add(new Coord(target.position.X, target.position.Y - 1));
            possibleMoves.Add(new Coord(target.position.X, target.position.Y + 1));

            //Enemy must move to an unoccupied tile within its move range
            Coord chosenMove;
            do {
                chosenMove = possibleMoves[randomMove.Next(0, possibleMoves.Count)];
                if(!checkedPossibleMoves.Contains(chosenMove)) {
                    checkedPossibleMoves.Add(chosenMove);
                }
                if(checkedPossibleMoves.Count == possibleMoves.Count) return;
            } while(!validSpaces.ContainsKey(chosenMove) || occupiedSpaces.Contains(chosenMove));
            moveUnit(chosenMove);
            attack(target, false);
        } else if(atkRange == 2) {
            //move 2 away from target and attack
            possibleMoves.Add(new Coord(target.position.X + 1, target.position.Y + 1));
            possibleMoves.Add(new Coord(target.position.X + 1, target.position.Y - 1));
            possibleMoves.Add(new Coord(target.position.X + 2, target.position.Y));
            possibleMoves.Add(new Coord(target.position.X - 1, target.position.Y + 1));
            possibleMoves.Add(new Coord(target.position.X - 1, target.position.Y - 1));
            possibleMoves.Add(new Coord(target.position.X - 2, target.position.Y));
            possibleMoves.Add(new Coord(target.position.X, target.position.Y + 2));
            possibleMoves.Add(new Coord(target.position.X, target.position.Y - 2));

            //Enemy must move to an unoccupied tile within its move range
            Coord chosenMove;
            do {
                chosenMove = possibleMoves[randomMove.Next(0, possibleMoves.Count)];
                if(!checkedPossibleMoves.Contains(chosenMove)) {
                    checkedPossibleMoves.Add(chosenMove);
                }
                if(checkedPossibleMoves.Count == possibleMoves.Count) return;
            } while(!validSpaces.ContainsKey(chosenMove) || occupiedSpaces.Contains(chosenMove));
            moveUnit(chosenMove);
            attack(target, false);
        }
    }

    //Find the nearest ally within this enemy's movement range
    Ally findAllyInRange(Dictionary<Coord, int> validSpaces) {
        List<Ally> allies = GameManager.instance.allies;
        foreach(Coord space in validSpaces.Keys) {
            for(int i = 0; i < allies.Count; i++) {
                Coord allyPosition = allies[i].position;
                if(space.X == allyPosition.X && space.Y == allyPosition.Y) {
                    return allies[i];
                }
            }
        }
        return null;
    }

    public override void findValidSpaces(int move, Coord currentSpace, int callNo, bool attack) {
        //Set this enemy as the selected enemy
        GameManager.instance.setSelectedEnemy(this);

        base.findValidSpaces(move, currentSpace, callNo, attack);
    }

    public override void moveUnit(Coord destination) {
        if(GameManager.twoPlayer) {
            base.moveUnit(destination);
            //After-move options must be opened after an enemy moves in 2 player mode
            InGameMenus menus = GameObject.Find("GameManager").GetComponent<InGameMenus>();
            menus.openAfterMoveOptions();
        } else {
            movedEnemies.Add(this);
            base.moveUnit(destination);
        }
    }

    public override void defeated() {
        base.defeated();
        GameManager.instance.enemyUnits.Remove(this.gameObject);
        GameManager.instance.enemies.Remove(this);

        //Check if all enemies are defeated, if so, the ally team wins
        if(GameManager.instance.enemyUnits.Count == 0) {
            SceneManager.LoadScene("End");
        }
    }

    void OnMouseOver() {
        Ally selectedAlly = GameManager.instance.selectedAlly;
        Enemy selectedEnemy = GameManager.instance.selectedEnemy;
        if(Input.GetMouseButtonDown(0)) {
            if(selectedAlly != null && selectedAlly.validSpaces.ContainsKey(position) && InGameMenus.attacking) {
                //An enemy is in the selected ally's range and has been clicked, so attack
                selectedAlly.attack(this, false);
                selectedAlly.hideValidSpaces();
            } else {
                selected = this;
                allTiles = MapGenerator.allTiles;

                //Only one enemy movement range can be seen at a time
                if(selectedEnemy != null && selectedEnemy != this) {
                    selectedEnemy.hideValidSpaces();
                }

                //If this enemy is unmoved and not attacking, show its movement range
                if(!movedEnemies.Contains(this) && !InGameMenus.attacking) {
                    findValidSpaces(mov, position, 0, false);
                    allTiles[position].showMovableSpaces(validSpaces, this);
                }
            }
        }
    }
}