using UnityEngine;
using UnityEngine.InputSystem;

public class RebindMenuManager : MonoBehaviour
{
    public InputActionReference MoveRef;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnEnable()
    {
        //MoveRef.action.Disable();
    }

    private void OnDisable()
    {
        //MoveRef.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            var action = MoveRef.action;
            var bindings = action.bindings;

            Debug.Log($"Bindings de la acción '{action.name}':");

            for (int i = 0; i < bindings.Count; i++)
            {
                var binding = bindings[i];

                // Si es un compuesto, mostrarlo y luego listar sus partes
                if (binding.isComposite)
                {
                    Debug.Log($" → {binding.name} (compuesto):");

                    // Listar sus partes
                    int j = i + 1;
                    while (j < bindings.Count && bindings[j].isPartOfComposite)
                    {
                        var part = bindings[j];
                        string path = string.IsNullOrEmpty(part.overridePath) ? part.path : part.overridePath;
                        string readable = InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice);
                        Debug.Log($"    - {part.name}: {readable}");
                        j++;
                    }

                    // Saltar los bindings que ya procesamos
                    i = j - 1;
                }
                else
                {
                    // Binding simple (no compuesto)
                    string path = string.IsNullOrEmpty(binding.overridePath) ? binding.path : binding.overridePath;
                    string readable = InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    Debug.Log($" → {readable}");
                }
            }
        }
    }
}
