using UnityEngine;

namespace ModLibrary.LevelEditor
{
    /// <summary>
    /// Disables <see cref="Collider"/> and <see cref="Renderer"/> components outside of the level editor
    /// </summary>
    public class LevelEditorDisableRendererAndCollision : MonoBehaviour
    {
        private void Start()
        {
            if (GameModeManager.IsInLevelEditor()) return;

            Collider collider = base.GetComponent<Collider>();
            if (collider) collider.enabled = false;

            Renderer renderer = base.GetComponent<Renderer>();
            if (renderer) renderer.enabled = false;
        }
    }
}