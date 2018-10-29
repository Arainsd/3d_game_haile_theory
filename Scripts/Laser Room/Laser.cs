using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private Vector3 scalePerSecond = new Vector3(0.9f, 0.9f, 0.9f);

    [SerializeField]
    private Vector3 direction = new Vector3(1f, 0f, 0f);

    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private ParticleSystem particles;

    [SerializeField]
    private AudioSource laserSound;

    [SerializeField]
    private Material shrunkMaterial;

    private bool _audioPaused;

    /// <summary>
    /// Pauses and unpauses the audioSources.
    /// </summary>
    private bool audioPaused
    {
        set
        {
            if (value == _audioPaused || laserSound == null)
                return;

            // If value is true, pause all the audioSources, else unpause them.
            if (value)
                laserSound.Pause();
            else
                laserSound.UnPause();

            _audioPaused = value;
        }
    }

    /// <summary>
    /// The time in seconds until the next laser sound can be started.
    /// </summary>
    private float laserSoundCooldown = 0f;

    /// <summary>
    /// The interval between laser one shots
    /// </summary>
    [SerializeField]
    private float laserSoundInterval = 0.5f;

    private void Start()
    {
        StartCoroutine(applyMaterial());
    }

    private void Update()
    {
        audioPaused = Options.PAUSED;

        if (Options.PAUSED)
            return;

        RaycastHit hit;

        Vector3 dir = direction;
        Ray ray = new Ray(transform.position, dir);

        List<Vector3> positions = new List<Vector3>();

        positions.Add(ray.origin);

        Vector3 secondLastPoint = ray.origin;
        Vector3 lastPoint = ray.origin;

        List<Reflectable> reflected = new List<Reflectable>();

        float maxDistance = 100f;

        bool drawLastRay = true; // Used to determine if we need to finish drawing the line in case we last hit a reflectable, that never hit anything else.

        lastShrunk = null; // Reset value

        while (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.isTrigger && hit.collider.gameObject.FindComponent<Player>(RedUtil.FindMode.PARENTS) == null) // Are we a trigger, and not in the player's hand?
            {
                ray = new Ray(hit.point, ray.direction); // Continue on the ray as if we hit nothing.
                continue;
            }

            drawLastRay = false; // Reset value.

            Reflectable reflectable = hit.collider.gameObject.GetComponent<Reflectable>();
            Shrinkable shrinkable = hit.collider.gameObject.GetComponent<Shrinkable>();
            ShrinkableCollider shrinkCollider = hit.collider.gameObject.GetComponent<ShrinkableCollider>();

            if (shrinkCollider != null)
            {
                shrinkable = shrinkCollider.colliderFor;
            }

            positions.Add(hit.point);
            secondLastPoint = lastPoint; // Move down the last point
            lastPoint = hit.point; // Then update the last point

            if (shrinkable != null)
            {
                if (laserSound != null)
                {
                    if (laserSoundCooldown < Time.realtimeSinceStartup)
                    {
                        laserSound.PlayOneShot(laserSound.clip, laserSound.volume);
                        laserSoundCooldown = Time.realtimeSinceStartup + laserSoundInterval;
                    }
                }

                Transform trans = shrinkable.transform;
                Vector3 currentScale = trans.localScale;

                Vector3 scaleSubtracted = new Vector3(1, 1, 1) - scalePerSecond;
                Vector3 scaleTime = scaleSubtracted * Time.deltaTime;

                Vector3 scaler = new Vector3(1, 1, 1) - scaleTime;

                Vector3 newScale = Vector3.Scale(currentScale, scaler);
                newScale = Vector3.Max(newScale, shrinkable.minimumSize);

                float scaleDiff = newScale.x / shrinkable.originalScale.x;

                if (scaleDiff < 0.3f)
                {
                    Door door = shrinkable.FindComponent<Door>(RedUtil.FindMode.ALL);

                    if (door != null)
                    {
                        door.hoverText = door.openText;

                        Player player = Player.instance;

                        if (player != null)
                            player.itemHandler.UpdatePopup(); // Update popup in case they are looking at it.

                        if (shrinkCollider != null)
                        {
                            shrinkCollider.collider.isTrigger = true; // Set to a trigger so the player can pass.
                        }
                    }
                }

                trans.localScale = newScale;
                lastShrunk = shrinkable.gameObject;
                break;
            }

            if (reflectable == null)
                break;

            if (reflected.Contains(reflectable))
                break;

            reflected.Add(reflectable);

            dir = Vector3.Reflect(dir, hit.normal);
            ray = new Ray(hit.point, dir);

            drawLastRay = true; // We last hit something reflected
        }

        if (drawLastRay) // If the last thing we hit was a reflector, but it also hits nothing
        {
            Vector3 newLastPoint = lastPoint + (ray.direction * maxDistance); // Create an imaginary last point that doesn't hit anything, but still draws the line for the player.

            positions.Add(newLastPoint);
            secondLastPoint = lastPoint; // Move down the last point
            lastPoint = newLastPoint; // Then update the last point
        }

        particles.transform.position = lastPoint;
        particles.transform.rotation = Quaternion.FromToRotation(secondLastPoint, lastPoint);

        Vector3[] points = positions.ToArray();

        lineRenderer.useWorldSpace = true;
        lineRenderer.SetVertexCount(points.Length);
        lineRenderer.SetPositions(points);
	}

    private GameObject lastShrunk;

    private IEnumerator applyMaterial()
    {
        GameObject last = null;

        while (true)
        {
            yield return new WaitUntil(() => lastShrunk != last);

            if (last != null)
            {
                setMaterial(last, false);
            }

            setMaterial(lastShrunk, true);
            last = lastShrunk;
        }
    }

    private void setMaterial(GameObject obj, bool state)
    {
        foreach (Renderer renderer in obj.FindComponents<Renderer>())
        {
            List<Material> materials = new List<Material>(renderer.sharedMaterials); // Use a list for easy methods.

            bool contains = materials.Find((m) => m.shader.Equals(shrunkMaterial.shader));

            if (contains && !state)
                materials.Remove(shrunkMaterial);
            else if (!contains && state)
                materials.Add(shrunkMaterial);

            renderer.materials = materials.ToArray();
        }
    }
}
