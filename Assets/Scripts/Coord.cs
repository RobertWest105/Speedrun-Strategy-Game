using UnityEngine;
using System;

[Serializable]
public struct Coord {

    public int X { get; private set; }
    public int Y { get; private set; }
    public Vector3 worldPos { get; private set; }

    public Coord(int x, int y) {
        X = x;
        Y = y;
        Vector3 originPos = CameraManager.originPos;
        float tileSize = GameManager.mapGen.tileSize;
        worldPos = new Vector3(originPos.x + (tileSize * x), originPos.y + (tileSize * y));
    }
}