using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Place this component on the player to give yourself some magical abilites!
/// </summary>
public class Cheats : MonoBehaviour
{
    [SerializeField] float noclipSpeed = 10;
    [SerializeField] Vector3 marker;

    Health h;
    NonphysController nc;

    bool noclip;
    bool godMode;
    float health;

    Color ambientLight;
    AmbientMode ambientMode;

    void Start()
    {
        marker = transform.position;

        h = GetComponent<Health>();
        nc = GetComponent<NonphysController>();
        h.OnHealthChanged += OnHealthChanged;
    }

    void Update()
    {
        if (Time.deltaTime == 0)
            return;

        if (Input.GetKeyDown(KeyCode.BackQuote))
            ClearConsole();

        if (Input.GetKeyDown(KeyCode.C))
            ToggleNightVision();

        if (Input.GetKeyDown(KeyCode.V))
            ToggleNoclip();

        if (Input.GetKeyDown(KeyCode.B))
            ToggleGodMode();

        if (Input.GetKeyDown(KeyCode.Home))
            StartElevator();

        if (Input.GetKeyDown(KeyCode.End))
            EndElevator();

        if (Input.GetKeyDown(KeyCode.Return))
            SetMarker();

        if (Input.GetKeyDown(KeyCode.Backspace))
            GotoMarker();

        Noclip(Time.deltaTime);
    }


    [MakeButton("Clear console", false)]
    void ClearConsole()
    {
#if UNITY_EDITOR
        // There where some comments on unity formus about this potentially causing a memory leak :D but I will use it still
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
#endif
    }

    [MakeButton("Toggle night vision", false)]
    void ToggleNightVision()
    {
        if (RenderSettings.ambientMode != AmbientMode.Flat || RenderSettings.ambientLight != Color.white)
        {
            Debug.Log("Night vision enabled");
            // save ambient settings to reset to later
            ambientMode = RenderSettings.ambientMode;
            ambientLight = RenderSettings.ambientLight;

            // set ambient to bright
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = Color.white;
            return;
        }

        Debug.Log("Night vision disabled");
        // reset to previous ambient settings
        RenderSettings.ambientMode = ambientMode;
        RenderSettings.ambientLight = ambientLight;
    }

    [MakeButton("Toggle Noclip", false)]
    void ToggleNoclip()
    {
        if (!noclip && !nc.enabled)
        {
            Debug.Log("Can only noclip when nonphyscontroller is enabled");
            return;
        }

        noclip = !noclip;
        nc.enabled = !noclip;

        Debug.Log("Noclip " + (noclip ? "enabled" : "disabled"));
    }

    void Noclip(float dt)
    {
        if (!noclip)
            return;

        if (Camera.main == null)
        {
            Debug.LogWarning("Cheats.Noclip - Couldn't find main camera :(");
            return;
        }

        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            move += Vector3.forward;
        if (Input.GetKey(KeyCode.A))
            move += Vector3.left;
        if (Input.GetKey(KeyCode.S))
            move += Vector3.back;
        if (Input.GetKey(KeyCode.D))
            move += Vector3.right;
        if (Input.GetKey(KeyCode.Space))
            move += Vector3.up;
        if (Input.GetKey(KeyCode.LeftControl))
            move += Vector3.down;

        // divided by 2 to match the new input system that nonphyscontroller uses
        float mouseX = Input.GetAxisRaw("Mouse X") * nc.MouseSensitivity / dt / 2f;
        float mouseY = Input.GetAxisRaw("Mouse Y") * nc.MouseSensitivity / dt / 2f;
        nc.Rotate(mouseY, mouseX);

        Vector3 fwd = Camera.main.transform.forward;
        Vector3 rgt = Camera.main.transform.right;
        Vector3 up = Vector3.up;

        bool sprint = Input.GetKey(KeyCode.LeftShift);

        transform.position += (move.x * rgt + move.y * up + move.z * fwd) * noclipSpeed * dt * (sprint ? 2f : 1f);
    }

    [MakeButton("Toggle God mode", false)]
    void ToggleGodMode()
    {
        godMode = !godMode;

        if (godMode)
        {
            health = h.Value;
            h.SetHealth(float.MaxValue);
        }
        else
        {
            h.SetHealth(health);
        }

        Debug.Log("God mode " + (godMode ? "enabled" : "disabled"));
    }

    // This isn't true godmode because player can potentially die if float.Max health is taken in one frame..
    // Yeah, like that's ever going to happen!

    void OnHealthChanged(Health health, float amount)
    {
        if (!godMode)
            return;

        health.SetHealth(float.MaxValue);
    }

    // Player will teleport to the last known elevator positions, or Vector3.zero if there is none. So use with caution i guess.

    [MakeButton("Teleport to start", false)]
    void StartElevator()
    {
        Debug.Log("Teleporting to start elevator...");
        TeleportTo(Blackboard.Instance.Get<Vector3>("startElevator"));
    }

    [MakeButton("Teleport to end", false)]
    void EndElevator()
    {
        Debug.Log("Teleporting to end elevator...");
        TeleportTo(Blackboard.Instance.Get<Vector3>("endElevator"));
    }
    [MakeButton("Set marker", false)]
    void SetMarker()
    {
        marker = transform.position;
        Debug.Log("Set marker to " + marker.ToString());
    }

    // If you messed up your teleport, you can teleport back to your marker. Marker is set to the player pos on start!

    [MakeButton("Teleport to marker", false)]
    void GotoMarker()
    {
        Debug.Log("Teleporting to marker...");
        TeleportTo(marker);
    }

    void TeleportTo(Vector3 pos)
    {
        Debug.Log("Teleported to " + pos.ToString());
        transform.position = pos;
    }
}
