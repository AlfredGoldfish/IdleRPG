using UnityEngine;

/// Add this to the TMP child (the object that has the MeshRenderer).
/// It forces a high sorting order so the number appears above sprites.
[RequireComponent(typeof(Renderer))]
public class RendererSorting : MonoBehaviour
{
    [SerializeField] string sortingLayerName = "Default"; // or create/use "UI"
    [SerializeField] int sortingOrder = 500;               // higher = in front

    void Awake()
    {
        var r = GetComponent<Renderer>();
        if (!string.IsNullOrEmpty(sortingLayerName)) r.sortingLayerName = sortingLayerName;
        r.sortingOrder = sortingOrder;
    }
}
