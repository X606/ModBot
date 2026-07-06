using UnityEngine;
using UnityEngine.UI;

public class ResizeHandle : MonoBehaviour
{
    public Color MouseOverColor;

    public Color MouseOutColor;

    public bool DragX;

    public bool DragY;

    public bool DragUp;

    public bool DragRight;

    public ResizablePanel TargetPanel;

    public Image MouseOverImage;

    private void Start()
    {
        OnMouseOut();
    }

    public void OnMouseDown()
    {
    }

    public void OnMouseOver()
    {
        MouseOverImage.color = MouseOverColor;
    }

    public void OnMouseOut()
    {
        MouseOverImage.color = MouseOutColor;
    }
}