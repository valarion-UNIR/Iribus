using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private InteractableHablarMV guardia;
    [SerializeField] private InteractableHablarMV marginado;

    private int floresRecogidas;

    public void FlorRecogida()
    {
        floresRecogidas = floresRecogidas + 1;

        switch (floresRecogidas)
        {
            case 1:
                marginado.SetIndexDialogos(3);
                break;
            case 3:
                guardia.SetIndexDialogos(3);
                break;
            case 4:
                marginado.SetIndexDialogos(4);
                if (guardia.GetIndexDialogos() == 3) guardia.SetIndexDialogos(4);
                break;
            default:
                Debug.Log("Algo no encaja con las flores: " + floresRecogidas);
                break;
        }
    }
}
