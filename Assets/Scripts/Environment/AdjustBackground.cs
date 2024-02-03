using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustBackground : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        Vector3 scale = transform.localScale;
        scale.x = cameraWidth / spriteSize.x;
        scale.y = cameraHeight / spriteSize.y;

        transform.localScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
