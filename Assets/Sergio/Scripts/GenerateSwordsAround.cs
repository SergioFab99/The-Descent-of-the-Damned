using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GenerateSwordsAround : MonoBehaviour
{
    // --------- INPUT / TECLAS ---------
    // Qué tecla prende el “modo espadas girando”. Por defecto: X.
    [Header("Input")]
    [SerializeField] private KeyCode activationKey = KeyCode.X;

    // Si está en true: apretás X una vez -> activa; apretás X otra vez -> desactiva.
    // Si está en false: apretás X -> activa, y no se apaga con la tecla (solo si desactivás el GameObject).
    [SerializeField] private bool toggleOnKey = true;

    // --------- PREFABS (los objetos que vas a spawnear) ---------
    [Header("Prefabs")]
    [Tooltip("Si 'Sword Prefabs' tiene 4 elementos, se usan esos. Si no, se repite 'Sword Prefab' 4 veces.")]

    // Un solo prefab para repetirlo 4 veces.
    [SerializeField] private GameObject swordPrefab;

    // Si querés 4 distintos (o varios), los arrastrás acá.
    [SerializeField] private GameObject[] swordPrefabs;

    // --------- SPAWN / POSICION ---------
    [Header("Spawn")]
    [Min(0f)]

    // Distancia desde el Player hasta cada espada.
    [SerializeField] private float radius = 1.5f;

    // Subir/bajar todas las espadas (por ejemplo para que queden a la altura del torso).
    [SerializeField] private float heightOffset = 0f;

    // --------- ROTACION / “HELICES” ---------
    [Header("Rotation")]

    // Eje de rotación. Por defecto Vector3.up = girar en el plano XZ alrededor del Player.
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    // Velocidad de giro (grados por segundo). 360 = una vuelta por segundo.
    [SerializeField] private float rotationSpeedDegreesPerSecond = 360f;

    // Este es el “truco”: en vez de rotar 4 objetos por separado,
    // creamos un pivote vacío que rota, y las espadas van colgadas de ese pivote.
    private Transform pivot;

    // Guardamos referencias para poder destruirlas fácil cuando se apaga.
    private readonly List<GameObject> spawned = new List<GameObject>(4);

    // Estado: está prendido o apagado.
    private bool isActive;

    private void Update()
    {
        // 1) Detectamos el apretón de tecla (solo en el frame donde se presiona).
        if (Input.GetKeyDown(activationKey))
        {
            if (!isActive)
            {
                // Si estaba apagado, lo prendemos.
                Activate();
            }
            else if (toggleOnKey)
            {
                // Si estaba prendido y el modo toggle está activo, lo apagamos.
                Deactivate();
            }
        }

        // 2) Si no está activo, no hacemos nada.
        if (!isActive || pivot == null)
        {
            return;
        }

        // 3) Rotamos el pivote cada frame. Eso hace que las 4 espadas giren alrededor del Player.
        // (Si el eje que pusiste es (0,0,0), lo arreglamos usando Vector3.up para que no explote.)
        var axis = rotationAxis.sqrMagnitude > 0.0001f ? rotationAxis.normalized : Vector3.up;
        pivot.Rotate(axis, rotationSpeedDegreesPerSecond * Time.deltaTime, Space.Self);
    }

    private void Activate()
    {
        // Antes de spawnear, chequeamos que haya algún prefab asignado.
        if (!HasAnyPrefab())
        {
            Debug.LogWarning("GenerateSwordsAround: Asigna 'Sword Prefab' o 4 elementos en 'Sword Prefabs' en el Inspector.", this);
            return;
        }

        // Creamos el pivote si no existe.
        EnsurePivot();

        // Instanciamos las 4 espadas alrededor.
        SpawnSwords();

        // Marcamos como activo para que empiece a rotar en Update().
        isActive = true;
    }

    private void Deactivate()
    {
        // Apagamos el estado (así deja de rotar).
        isActive = false;

        // Destruimos las espadas instanciadas.
        CleanupSpawned();

        // Y destruimos el pivote para dejar todo limpio.
        if (pivot != null)
        {
            Destroy(pivot.gameObject);
            pivot = null;
        }
    }

    private void EnsurePivot()
    {
        // Si ya existe el pivote, listo.
        if (pivot != null)
        {
            return;
        }

        // Creamos un GameObject vacío que va a ser el centro de rotación.
        var pivotGO = new GameObject("SwordsPivot");
        pivot = pivotGO.transform;

        // Lo hacemos hijo del Player para que lo siga cuando se mueva.
        pivot.SetParent(transform);

        // Lo posicionamos en local, con el offset de altura.
        pivot.localPosition = new Vector3(0f, heightOffset, 0f);
        pivot.localRotation = Quaternion.identity;
    }

    private void SpawnSwords()
    {
        // Por si había espadas viejas (por ejemplo reactivaste), limpiamos primero.
        CleanupSpawned();

        // 4 posiciones simétricas: derecha, izquierda, adelante, atrás.
        // (En local space del pivote.)
        var offsets = new Vector3[4]
        {
            new Vector3(radius, 0f, 0f),
            new Vector3(-radius, 0f, 0f),
            new Vector3(0f, 0f, radius),
            new Vector3(0f, 0f, -radius),
        };

        for (int i = 0; i < 4; i++)
        {
            // Elegimos qué prefab usar para esta “ranura”.
            var prefab = GetPrefabForIndex(i);
            if (prefab == null)
            {
                // Si por algún motivo no hay prefab, saltamos.
                continue;
            }

            // Instanciamos como hijo del pivote para que herede la rotación.
            var instance = Instantiate(prefab, pivot);

            // Lo dejamos en su offset y con rotación local en 0.
            // Ojo: si querés que “miren hacia afuera”, podemos ajustar esto.
            instance.transform.localPosition = offsets[i];
            instance.transform.localRotation = Quaternion.identity;

            // Guardamos la referencia para poder destruirla después.
            spawned.Add(instance);
        }
    }

    private GameObject GetPrefabForIndex(int index)
    {
        // Si hay un array, intentamos usarlo.
        if (swordPrefabs != null && swordPrefabs.Length > 0)
        {
            // Si hay 4 elementos, usamos el correspondiente. Si no, repetimos el último no-nulo o caemos a swordPrefab.
            if (index < swordPrefabs.Length && swordPrefabs[index] != null)
            {
                return swordPrefabs[index];
            }

            for (int i = swordPrefabs.Length - 1; i >= 0; i--)
            {
                if (swordPrefabs[i] != null)
                {
                    return swordPrefabs[i];
                }
            }
        }

        return swordPrefab;
    }

    private bool HasAnyPrefab()
    {
        // Caso simple: con un prefab ya alcanza.
        if (swordPrefab != null)
        {
            return true;
        }

        // Si no hay array, no hay nada.
        if (swordPrefabs == null)
        {
            return false;
        }

        // Recorremos el array para ver si hay al menos uno asignado.
        for (int i = 0; i < swordPrefabs.Length; i++)
        {
            if (swordPrefabs[i] != null)
            {
                return true;
            }
        }

        return false;
    }

    private void CleanupSpawned()
    {
        // Destruimos las instancias que creamos.
        for (int i = 0; i < spawned.Count; i++)
        {
            if (spawned[i] != null)
            {
                Destroy(spawned[i]);
            }
        }

        // Limpiamos la lista para que quede prolijo.
        spawned.Clear();
    }

    private void OnDisable()
    {
        // Evita dejar objetos colgados si el GameObject se desactiva.
        if (isActive)
        {
            Deactivate();
        }
    }
}
