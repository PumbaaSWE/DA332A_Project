using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEffect : MonoBehaviour, IDamageble
{
    public Material objectMaterial;

    public GameObject decalPrefab;
    //public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    //{

    //    GameObject decal = Instantiate(decalPrefab, point, Quaternion.identity);

    //    decal.transform.forward = direction;
    //    decal.transform.parent = transform;
    //    Destroy(decal, 2f); 
    //}

   

    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        Debug.Log($"Damage taken at point: {point}, direction: {direction}, amount: {damage}");

 

        Vector3 localPoint = transform.InverseTransformPoint(point);

        objectMaterial.SetVector("_ImpactPoint", new Vector4(localPoint.x, localPoint.y,localPoint.z, 1));
        objectMaterial.SetColor("_ImpactColor", Color.red);
        objectMaterial.SetFloat("_ImpactRadius", 0.1f);

        Debug.Log($"Impact Point: {localPoint}");
        Debug.Log($"Material ImpactPoint: {objectMaterial.GetVector("_ImpactPoint")}");
        Debug.Log($"Material ImpactRadius: {objectMaterial.GetFloat("_ImpactRadius")}");

        StartCoroutine(ResetImpactEffect());
    }


    private IEnumerator ResetImpactEffect()
    {
        yield return new WaitForSeconds(1.5f);
        objectMaterial.SetFloat("_ImpactRadius", 0.0f);
    }

}
