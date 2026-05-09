using UnityEngine;

namespace InternalModBot.LevelEditor
{
    internal class LevelEditorDisableRendererAndCollission : MonoBehaviour
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