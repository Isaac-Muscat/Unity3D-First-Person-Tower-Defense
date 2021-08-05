using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int x {get;}
    public int y {get;}
    public float f {get; set;}
    public float g {get; set;}
    public float h {get; set;}
    public Tile parent;
    public bool visited;

    public Tile(int x, int y){
        this.x = x;
        this.y = y;
        g= 10000000;
        f = g;
        parent = null;
        visited = false;
    }

    public float getDistance(Vector2 target){
        return Mathf.Abs(target.x-x) + Mathf.Abs(target.y-y);
    }

    public static int CompareTileCost(Tile a, Tile b){
        if (a == null){
            if (b == null){
                return 0;
            } else {
                return -1;
            }
        } else {
            if (b == null) {
                return 1;
            }
        }
        if(a.f > b.f)
            return 1;
        if(b.f > a.f)
            return -1;
        return 0;
    }


}
