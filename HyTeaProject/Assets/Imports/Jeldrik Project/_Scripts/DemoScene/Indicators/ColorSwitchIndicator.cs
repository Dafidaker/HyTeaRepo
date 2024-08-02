using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ColorSwitchIndicator : MonoBehaviour
{
    private RawImage _image;
    
    private void Awake()
    {
        _image = GetComponent<RawImage>();
    }
    
    public void SetColor(Color color)
    {
        _image.color = color;
    }
}
