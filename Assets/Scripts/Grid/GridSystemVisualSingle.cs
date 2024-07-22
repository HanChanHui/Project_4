using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;

    public void Show(Material material)
    {
        sprite.enabled = true;
        sprite.material = material;
        sprite.color = new Color(1f, 1f, 1f, 0.2f);
    }

    public void Hide()
    {
        sprite.enabled = false;
    }
}
