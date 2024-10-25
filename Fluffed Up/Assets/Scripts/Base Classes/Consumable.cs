using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public string consumableName;
    public string description;
    public int cost;
    public float power;
    public Sprite shopArt;
    public Sprite toolbarArt;

    public Consumable(string consumableName, string description, int cost, float power, Sprite shopArt, Sprite toolbarArt)
    {
        this.consumableName = consumableName;
        this.description = description;
        this.cost = cost;
        this.power = power;
    }
}
