using UnityEngine;

public class JumpFromBelowPlatform : MonoBehaviour
{
    BoxCollider2D platform;

    void Awake()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D collider in colliders)
        {
            if (collider.isTrigger) return;

            this.platform = collider; break;
        }
    }

    private void OnTriggerEnter2D()
    {
        this.platform.enabled = false;
    }

    private void OnTriggerExit2D()
    {
        this.platform.enabled = true;
    }
}
