using UnityEngine;

public class Duck : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform duckWhite;                 // drag DuckWhite here

    [Header("Orbit (looping circle)")]
    [SerializeField] private float orbitRadius = 0.75f;           
    [SerializeField] private float orbitRevsPerSec = 0.25f;     

    [Header("Self Spin (optional)")]
    [SerializeField] private float selfSpinDegPerSec = 120f;     

    [Header("Audio")]
    [SerializeField] private AudioSource marioPaintDuckTalesThemeSong; 

    private bool playerInside = false;
    private Vector3 orbitCenter;    
    private float startY;         

    private void Awake()
    {
        if (duckWhite != null)
        {
            orbitCenter = duckWhite.position;
            startY = duckWhite.position.y;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;

        if (marioPaintDuckTalesThemeSong)
        {
            marioPaintDuckTalesThemeSong.loop = true;
            marioPaintDuckTalesThemeSong.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;

        if (marioPaintDuckTalesThemeSong) marioPaintDuckTalesThemeSong.Stop();

        if (duckWhite != null)
            duckWhite.position = new Vector3(orbitCenter.x, startY, orbitCenter.z);
    }

    private void Update()
    {
        if (!playerInside || duckWhite == null) return;

        float angle = Time.time * orbitRevsPerSec * Mathf.PI * 2f;
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * orbitRadius;
        duckWhite.position = new Vector3(orbitCenter.x + offset.x, startY, orbitCenter.z + offset.z);

        if (selfSpinDegPerSec != 0f)
            duckWhite.Rotate(Vector3.up, selfSpinDegPerSec * Time.deltaTime, Space.Self);
    }
}
