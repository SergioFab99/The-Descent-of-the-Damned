using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    public InvocarCadaveresSkill invocarCadaveres;
    public CaminoMuertoSkill caminoMuerto;
    public DemonioInfernalSkill demonioInfernal;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            invocarCadaveres.TryUse(gameObject);

        if (Input.GetKeyDown(KeyCode.Q))
            caminoMuerto.TryUse(gameObject);

        if (Input.GetKeyDown(KeyCode.R))
            demonioInfernal.TryUse(gameObject);
    }
}
