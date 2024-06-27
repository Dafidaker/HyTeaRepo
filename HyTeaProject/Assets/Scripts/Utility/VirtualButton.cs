using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Define a virtual base class for buttons
public abstract class VirtualButton : MonoBehaviour
{
    public abstract void OnClick();
}
