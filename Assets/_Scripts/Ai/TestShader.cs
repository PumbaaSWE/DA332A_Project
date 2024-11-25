using UnityEngine;

public class TestShader : MonoBehaviour
{
    public Material dissolveMaterial;
    public float dissolveSpeed = 1.0f;

    private float dissolveAmount = 0.0f;

    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) 
        {
            Testing();
        }
    }

    [MakeButton]
    void Testing()
    {
        dissolveAmount += Time.deltaTime * dissolveSpeed;
        dissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);

        if (dissolveAmount >= 1.0f)
        {
            Destroy(gameObject);
        }
    }
}
