using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerClicker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Camera cam = Camera.main;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out IDamageble dmg))
                {
                    dmg.TakeDamage(hit.point, ray.direction, 50);
                }
                else //bonus...
                {
                    dmg = hit.collider.GetComponentInParent<IDamageble>();
                    if (dmg != null)
                    {
                        dmg.TakeDamage(hit.point, ray.direction, 50);
                    }
                }
                
            }
        }
        
        
    }
}
