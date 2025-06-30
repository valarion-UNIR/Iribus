using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private InteractableHablarMV guardia;
    [SerializeField] private InteractableHablarMV marginado;
    [SerializeField] private GameObject verja;

    [SerializeField] private int floresRecogidas = 0;

    public void FlorRecogida()
    {
        floresRecogidas++;

        switch (floresRecogidas)
        {
            case 1:
                if(marginado.GetIndexDialogos() == 4)
                {
                    marginado.SetIndexDialogos(5);
                }
                else if (marginado.GetIndexDialogos() == 0)
                {
                    marginado.SetIndexDialogos(8);
                }
                break;
            case 3:
                if (guardia.GetIndexDialogos() != 3)
                {
                    guardia.SetIndexDialogos(7);
                    break;
                }
                guardia.SetIndexDialogos(4);
                break;
            default:
                Debug.Log("Algo no encaja con las flores: " + floresRecogidas);
                break;
        }
    }

    public void AbrirVerja()
    {
        verja.GetComponent<Animator>().SetTrigger("Abrir");
    }
}
