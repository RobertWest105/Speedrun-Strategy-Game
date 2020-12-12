using System;

//A class to store the x and y positions of a Coord to be saved
[Serializable]
public class SerializableCoord {

    public int X { get; private set; }
    public int Y { get; private set; }

    public SerializableCoord(Coord point) {
        X = point.X;
        Y = point.Y;
    }

}