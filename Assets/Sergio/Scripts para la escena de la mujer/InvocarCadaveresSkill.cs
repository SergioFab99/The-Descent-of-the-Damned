using UnityEngine;

[System.Serializable]
public class InvocarCadaveresSkill : SkillBase
{
    public GameObject cadaverPrefab;
    public int cantidad = 5;
    public float radio = 2f;

    protected override void Activate(GameObject caster)
    {
        for (int i = 0; i < cantidad; i++)
        {
            Vector3 offset = Random.insideUnitSphere * radio;
            offset.y = 0;

            GameObject cadaver = Object.Instantiate(
                cadaverPrefab,
                caster.transform.position + offset,
                Quaternion.Euler(-90, 0, 0)
            );

            cadaver.GetComponent<SummonBase>().Initialize(caster);
        }
    }
}