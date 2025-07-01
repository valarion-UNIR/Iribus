using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [SerializeField] private List<CheckPoint> checkPoints;
    [SerializeField] private List<CheckPoint> entradas;

    [SerializeField] private Camera sceneMainCamera;
    [SerializeField] private CinemachineCamera activeCamera;

    public CheckPoint GetCheckPoint(int index)
    {
        return checkPoints[index];
    }

    public CheckPoint GetEntrada(int index)
    {
        return entradas[index];
    }

    public Camera GetSceneCamera()
    {
        return sceneMainCamera;
    }

    public void ChangeBlend(CinemachineBlendDefinition.Styles blendType, float time)
    {
        sceneMainCamera.GetComponent<CinemachineBrain>().DefaultBlend = new CinemachineBlendDefinition(blendType, time);
    }

    public void SetActiveCMCamera(CinemachineCamera activeCamera)
    {
        this.activeCamera = activeCamera;
    }

    public CinemachineCamera GetActiveCMCamera()
    {
        return activeCamera;
    }
}
