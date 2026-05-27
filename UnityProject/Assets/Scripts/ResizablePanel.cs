using UnityEngine;
using UnityEngine.Events;

public class ResizablePanel : MonoBehaviour
{
    public bool PreserveAspectRatio;

    public bool CanResizeViewport;

    public UnityEvent ResizedCallback;

    public float MinHeight = -1f;

    public float MinWidth = -1f;

    public float MaxHeight = -1f;

    public float MaxWidth = -1f;
}