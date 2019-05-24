using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerAdjuster : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Escape _escapeScript;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _escapeScript = FindObjectOfType<Escape>();

        if (_escapeScript != null &&
            _escapeScript.LayerAdjusters.Contains(this) == false)
        {
            _escapeScript.LayerAdjusters.Add(this);
        }
    }

    public void UpdateLayer()
    {
        _spriteRenderer.sortingOrder += 4;
    }
}
