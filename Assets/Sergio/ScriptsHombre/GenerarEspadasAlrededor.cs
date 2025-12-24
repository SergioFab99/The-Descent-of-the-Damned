using UnityEngine;
using System.Collections.Generic;

public class GenerarEspadasAlrededor : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode claveActivacion = KeyCode.X;

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabEspada;

    [Header("Spawn Settings")]
    [SerializeField] private float radio = 2.5f;
    [SerializeField] private float desplazamientoAltura = 1.0f;

    [Header("Rotation Settings")]
    [SerializeField] private float velocidadRotacion = 600f;

    [Header("Projectile Settings")]
    [SerializeField] private float velocidadProyectil = 50f;
    [SerializeField] private float vidaProyectil = 4f;

    private Transform pivote;
    private List<GameObject> espadasGeneradas = new List<GameObject>();
    private bool estaActivo = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(claveActivacion))
        {
            if (!estaActivo) ActivarHabilidad();
            else if (espadasGeneradas.Count > 0) DispararEspada();
        }

        if (estaActivo && pivote != null)
        {
            pivote.Rotate(Vector3.up, velocidadRotacion * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void ActivarHabilidad()
    {
        if (prefabEspada == null) return;

        GameObject objetoJuegoPivote = new GameObject("PivoteEspadas");
        pivote = objetoJuegoPivote.transform;
        pivote.SetParent(this.transform);
        pivote.localPosition = new Vector3(0, desplazamientoAltura, 0);
        pivote.localRotation = Quaternion.identity;

        GenerarEspadas();
        estaActivo = true;
    }

    void GenerarEspadas()
    {
        Vector3[] direcciones = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

        foreach (Vector3 direccion in direcciones)
        {
            GameObject espada = Instantiate(prefabEspada, pivote);
            espada.transform.localPosition = direccion * radio;
            espada.transform.localRotation = Quaternion.LookRotation(-direccion);
            espada.transform.Rotate(90, 0, 0, Space.Self);
            espadasGeneradas.Add(espada);
        }
    }

    void DispararEspada()
    {
        if (espadasGeneradas.Count == 0) return;

        GameObject espada = espadasGeneradas[0];
        espadasGeneradas.RemoveAt(0);

        if (espada != null)
        {
            Vector3 adelanteCamara = Camera.main != null ? Camera.main.transform.forward : transform.forward;
            Vector3 direccionObjetivo = new Vector3(adelanteCamara.x, 0, adelanteCamara.z).normalized;
            
            if (direccionObjetivo == Vector3.zero) direccionObjetivo = transform.forward;

            espada.transform.SetParent(null);

            espada.transform.rotation = Quaternion.LookRotation(direccionObjetivo);
            espada.transform.Rotate(90, 0, 0, Space.Self);

            Rigidbody cuerpoRigido = espada.GetComponent<Rigidbody>();
            if (cuerpoRigido == null) cuerpoRigido = espada.AddComponent<Rigidbody>();

            cuerpoRigido.isKinematic = false;
            cuerpoRigido.useGravity = false;
            
            cuerpoRigido.linearVelocity = Vector3.zero;
            cuerpoRigido.angularVelocity = Vector3.zero;

            cuerpoRigido.linearVelocity = direccionObjetivo * velocidadProyectil;

            Destroy(espada, vidaProyectil);
        }

        if (espadasGeneradas.Count == 0) DesactivarHabilidad();
    }

    void DesactivarHabilidad()
    {
        estaActivo = false;
        if (pivote != null) Destroy(pivote.gameObject);
        espadasGeneradas.Clear();
    }
}