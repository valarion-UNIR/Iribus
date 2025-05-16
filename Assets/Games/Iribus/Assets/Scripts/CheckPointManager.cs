using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [SerializeField] private List<CheckPoint> checkPoints;
    [SerializeField] private List<CheckPoint> entradas;

    public CheckPoint GetCheckPoint(int index)
    {
        return checkPoints[index];
    }

    public CheckPoint GetEntrada(int index)
    {
        return entradas[index];
    }

}
