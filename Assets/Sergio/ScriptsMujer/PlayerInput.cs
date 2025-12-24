using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour
{
    [Header("Abilities")]
    public AbilityBase abilityQ;
    public AbilityBase abilityE;
    public AbilityBase abilityX;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!this || !gameObject.activeInHierarchy)
            return;

        if (Input.GetKeyDown(KeyCode.Q) && abilityQ)
            abilityQ.Activate();

        if (Input.GetKeyDown(KeyCode.E) && abilityE)
            abilityE.Activate();

        if (Input.GetKeyDown(KeyCode.X) && abilityX)
            abilityX.Activate();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
