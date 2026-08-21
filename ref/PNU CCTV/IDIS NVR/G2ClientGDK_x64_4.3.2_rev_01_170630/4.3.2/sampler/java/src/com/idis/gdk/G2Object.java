package com.idis.gdk;

public abstract class G2Object {
    public abstract G2Object clone();
    public abstract void serialize(G2Stream s);
    public abstract void deserialize(G2Stream s);
}
