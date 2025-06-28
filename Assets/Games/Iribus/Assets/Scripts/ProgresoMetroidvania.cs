using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class ProgresoMetroidvania
{
    public int escena;
    public int checkpoint;
    public Dictionary<string, int> objetos;

    public ProgresoMetroidvania(int escena, int checkpoint)
    {
        this.escena = escena;
        this.checkpoint = checkpoint;
    }
}