using UnityEngine;

[System.Serializable]
public class CaminoMuertoSkill : SkillBase
{
    public GameObject areaPrefab;

    protected override void Activate(GameObject caster)
    {
        Object.Instantiate(
            areaPrefab,
            caster.transform.position,
            Quaternion.identity
        );
    }
}
