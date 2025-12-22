using UnityEngine;

[System.Serializable]
public class DemonioInfernalSkill : SkillBase
{
    public GameObject demonioPrefab;
    private GameObject demonioActual;

    protected override void Activate(GameObject caster)
    {
        if (demonioActual != null)
            Object.Destroy(demonioActual);

        demonioActual = Object.Instantiate(
            demonioPrefab,
            caster.transform.position + Vector3.forward * 2f,
            Quaternion.Euler(-90, 0, 0)
        );

        demonioActual.GetComponent<SummonBase>().Initialize(caster);
    }
}