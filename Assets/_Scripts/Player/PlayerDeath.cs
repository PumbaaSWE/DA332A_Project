using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] PlayerInput input;
    [SerializeField] float fallDuration = 1;
    [SerializeField] AnimationCurve fallCurve;
    [Tooltip("Wait for x seconds after falling")]
    [SerializeField] float waitDuration = 1;

    void OnEnable()
    {
        GetComponent<Health>().OnDeath += PlayerDied;
    }

    void OnDisable()
    {
        GetComponent<Health>().OnDeath -= PlayerDied;
    }

    [MakeButton(false)]
    void Test()
    {
        Health h = GetComponent<Health>();
        h.Damage(h.Value);
    }

    private void PlayerDied(Health health)
    {
        enabled = false;
        input.DeactivateInput();
        StartCoroutine(DeathAnim());
    }

    private IEnumerator DeathAnim()
    {
        float timer = fallDuration;

        GetComponent<NonphysController>().enabled = false;
        Camera cam = GetComponentInChildren<Camera>();
        if (cam)
        {
            Vector3 pos = transform.position + Vector3.up * 0.4f;
            Vector3 camPos = cam.transform.position;
            Vector3 camRot = cam.transform.rotation.eulerAngles;
            Vector3 rot = cam.transform.rotation.eulerAngles.WithZ(65);
            while (timer >= 0)
            {
                float t = fallCurve.Evaluate(timer / fallDuration);
                timer -= Time.deltaTime;

                Vector3 p = Vector3.Lerp(pos, camPos, t);
                Vector3 r = Vector3.Lerp(rot, camRot, t);

                Camera.main.transform.SetPositionAndRotation(p, Quaternion.Euler(r));
                yield return null;
            }
        }

        yield return new WaitForSeconds(waitDuration);

        ShowMenu();
    }

    private void ShowMenu()
    {
        LoadDeathScene lds = FindAnyObjectByType<LoadDeathScene>();

        if (lds)
            lds.LoadScene();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
