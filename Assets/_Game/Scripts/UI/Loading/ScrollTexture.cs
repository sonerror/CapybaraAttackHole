using UnityEngine;
using UnityEngine.UI;

public class ScrollTexture : MonoBehaviour
{
    public Image img;
    public Vector2 speed;
    void Start()
    {
        img.material = new Material(img.material);
    }
    private void OnEnable()
    {
        img.material = new Material(img.material);
    }
    void Update()
    {
        img.material.mainTextureOffset += speed * Time.deltaTime;
    }
}
