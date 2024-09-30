using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] float time = 1;
    [SerializeField] PlayerInput input;

    void Start()
    {


        GetComponent<Health>().OnDeath += PlayerDied;

    }

    private void PlayerDied(Health health)
    {
        enabled = false;
        input.DeactivateInput();
        StartCoroutine(DeathAnim());
    }

    private IEnumerator DeathAnim()
    {
        float timer = time;


        Camera cam = GetComponentInChildren<Camera>();
        if (cam)
        {
            Vector3 pos = transform.position + Vector3.up * 0.4f;
            Vector3 camPos = cam.transform.position;
            Vector3 camRot = cam.transform.rotation.eulerAngles;
            Vector3 rot = cam.transform.rotation.eulerAngles.WithZ(65);
            while (timer >= 0)
            {
                float t = timer / time;
                timer -= Time.deltaTime;

                Vector3 p = Vector3.Lerp(pos, camPos, t);
                Vector3 r = Vector3.Lerp(rot, camRot, t);

                Camera.main.transform.SetPositionAndRotation(p, Quaternion.Euler(r));
                yield return null;
            }
        }

        

        yield return new WaitForSeconds(3);

        ShowMenu();
    }

    private void ShowMenu()
    {

        //input.ActivateInput();
    }



    // Update is called once per frame
    void Update()
    {

    }
}
