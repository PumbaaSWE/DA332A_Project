using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    [SerializeField] private float noiseStrength = 0.25f;
    [SerializeField] private float objectHeight = .8f;
    private float dissolveSpeed = 0.4f; 

    private Material material;
    private float currentHeight;
    public bool death;

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;

        currentHeight = transform.position.y + (objectHeight / 2.0f);
    }

    private void Update()
    {
        if (death)
        {
            Death();
        }
    }

    private void SetHeight(float height)
    {
        material.SetFloat("_Cutoff_Height", height);        
        material.SetFloat("_NoiceStreanght", noiseStrength); 
    }

    [ContextMenu("Trigger Death")] 
    public void Death()
    {

        currentHeight -= dissolveSpeed * Time.deltaTime;
        SetHeight(currentHeight);

     
        if (currentHeight <= -1)
        {
            death = false;
        }
    }
}
