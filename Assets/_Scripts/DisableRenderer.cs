using UnityEngine;


/**
 * Used on triggers och blocking zones to show them inspector but not in game
 * Kinda cheap solution as renderer components still exists in the scene, custom GUI drawers would be "more profesional"
 * 
 * -Jack
 * 
 */

[DisallowMultipleComponent]
public class DisableRenderer : MonoBehaviour
{
    
    void Awake()
    {
        if(TryGetComponent(out Renderer renderer))
        {
            renderer.enabled = false;
        }
    }

}
