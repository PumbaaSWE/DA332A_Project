using System.Collections;
using UnityEngine;

public class ElevatorLightEffect : MonoBehaviour
{
    Material material;
    [SerializeField]LineRenderer lineRenderer;
 //   [SerializeField] float maxSpeed = 3;
    [SerializeField] float dist = 3;
 //   [SerializeField] float maxAccel = 1;

    void Start()
    {
        material = lineRenderer.sharedMaterial;
        material.SetFloat("_Speed", 0);
    }

    [MakeButton(false)]
    public void StartDown(float t)
    {
        StartCoroutine(DoEffect(t));
    }

    //private IEnumerator DoEffectSimple(float time)
    //{
    //    material.SetFloat("_Speed", maxSpeed*.8f);
    //    yield return new WaitForSeconds(1);
    //    material.SetFloat("_Speed", maxSpeed);
    //    yield return new WaitForSeconds(time-2);
    //    material.SetFloat("_Speed", maxSpeed * .7f);
    //    yield return new WaitForSeconds(.5f);
    //    material.SetFloat("_Speed", maxSpeed * .3f);
    //    yield return new WaitForSeconds(.5f);
    //    material.SetFloat("_Speed", 0);
    //}

    private IEnumerator DoEffect(float time)
    {
        string value = "_Tile";
        float t = 0;
        float s;
        float stepSize = 1 / time;
        while(time > 0){
            float dt = Time.deltaTime;
            time -= dt;
            //v = v + a * dt;
            //float error = maxSpeed - v;
            //material.SetFloat(value, s);
            t += stepSize*dt;
            s = Mathf.SmoothStep(0, dist, t);
            material.SetFloat(value, s);
            yield return null;
        }
        material.SetFloat(value, 0);
    }
}
