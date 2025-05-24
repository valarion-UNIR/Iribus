using UnityEngine;

public class PachinkoCamera : SubGamePlayerController
{
    public override SubGame SubGame => SubGame.Pachinko;

    [SerializeField] private float topLimit;
    [SerializeField] private float bottomLimit;
    [SerializeField] private float speedMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        float inputV = Input.Pachinko.Move.ReadValue<Vector2>().y;

        Vector3 top = new Vector3(transform.position.x, topLimit, transform.position.z);
        Vector3 bottom = new Vector3(transform.position.x, bottomLimit, transform.position.z);

        // Dirección ahora apunta hacia arriba
        Vector3 direccion = (top - bottom).normalized;

        Vector3 movimiento = direccion * inputV * speedMovement * Time.deltaTime;
        transform.position += movimiento;

        // Clampear la posición para que no se salga de los límites
        float distanciaTotal = Vector3.Distance(top, bottom);
        float proyeccion = Vector3.Dot(transform.position - bottom, direccion);
        proyeccion = Mathf.Clamp(proyeccion, 0f, distanciaTotal);

        transform.position = bottom + direccion * proyeccion;
    }
}
