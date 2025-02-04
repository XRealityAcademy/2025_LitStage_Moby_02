using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SeaEffectController : MonoBehaviour
{
    [Header("Child Sea Effect Objects")]
    [Tooltip("If left empty, all direct children of this prefab will be treated as sea effect objects.")]
    [SerializeField] private List<GameObject> seaEffects = new List<GameObject>();
    [Header("Fade Settings (in seconds)")]
    [SerializeField] private float fadeInDuration = 1f;   // Time to fade from transparent to opaque
    [SerializeField] private float holdDuration   = 1f;   // Time to stay fully opaque
    [SerializeField] private float fadeOutDuration = 1f;   // Time to fade back to transparent
    /// <summary>
    /// Automatically populate the list with all direct children if none were assigned.
    /// </summary>
    private void Awake()
    {
        if (seaEffects == null || seaEffects.Count == 0)
        {
            // Optionally: Only add children with a specific tag or name if needed.
            foreach (Transform child in transform)
            {
                seaEffects.Add(child.gameObject);
            }
        }
    }
    private void Start()
    {
        SeaEffect();
        // foreach (GameObject effect in seaEffects)
        // {
        //     StartCoroutine(DoSeaEffect(effect));
        // }

    }



    public void SeaEffect()
    {
        foreach (GameObject effect in seaEffects)
        {
            StartCoroutine(DoSeaEffect(effect));
        }
    }
    /// <summary>
    /// Coroutine that fades in the provided GameObject’s material from transparent to opaque,
    /// holds the opacity, then fades out and deactivates the object.
    /// </summary>
    /// <param name="effect">The sea effect GameObject.</param>
    /// <returns></returns>
    private IEnumerator DoSeaEffect(GameObject effect)
    {
        // Activate the effect if not already active.
        effect.SetActive(true);
        // Ensure the GameObject has a Renderer component.
        Renderer renderer = effect.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("GameObject '" + effect.name + "' does not have a Renderer component!");
            yield break;
        }
        // Access the material and its color.
        Material mat = renderer.material;
        Color color = mat.color;
        // Start fully transparent.
        color.a = 0f;
        mat.color = color;
        // --- Fade In: from 0 to 1 ---
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            mat.color = color;
            yield return null;
        }
        // Ensure fully opaque.
        color.a = 1f;
        mat.color = color;
        // --- Hold at full opacity ---
        yield return new WaitForSeconds(holdDuration);
        // --- Fade Out: from 1 to 0 ---
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            mat.color = color;
            yield return null;
        }
        // Ensure fully transparent.
        color.a = 0f;
        mat.color = color;
        // Optionally deactivate the effect after fading out.
        effect.SetActive(false);
    }
}
