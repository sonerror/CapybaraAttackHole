using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ChangeColorObject : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    private Color originalColor;
    public Color highlightColor;
    private Enemy enemy;
    private void Start()
    {
        originalColor = meshRenderer.material.color;
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Const.TAG_PLAYER))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (!enemy && enemy.lvCurrent > (player.lvCurrent))
            {
                GetComponent<Renderer>().material.color = highlightColor;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(Const.TAG_PLAYER))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (!enemy && enemy.lvCurrent > (player.lvCurrent + 1))
            {
                GetComponent<Renderer>().material.color = originalColor;
            }
        }
    }
}
