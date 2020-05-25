using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModLibrary;
using System.Collections;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to draw lines on screen
    /// </summary>
    public class DebugLineDrawingManager : Singleton<DebugLineDrawingManager>
    {
        List<LineInfo> _linesToDraw = new List<LineInfo>();

        /// <summary>
        /// Adds a line to the lines to draw this frame
        /// </summary>
        /// <param name="info"></param>
        public void AddLine(LineInfo info)
        {
            _linesToDraw.Add(info);
        }

        void Update()
        {
            Camera main = Camera.main;
            if (main == null)
            {
                FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
                if (player == null)
                    return;

                main = player.GetPlayerCamera();
                if (main == null)
                    return;

            }
            if (main.GetComponent<DebugLineDrawer>() == null)
            {
                main.gameObject.AddComponent<DebugLineDrawer>();
            }
            
            StartCoroutine(runAtEndOfFrame());
        }

        IEnumerator runAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();

            for (int i = 0; i < _linesToDraw.Count; i++)
            {
                if (_linesToDraw[i].EndTime <= Time.unscaledTime)
                {
                    _linesToDraw.RemoveAt(i);
                    i--;
                }
            }

        }

        class DebugLineDrawer : MonoBehaviour
        {
            Material _lineMaterial;

            void Awake()
            {
                _lineMaterial = InternalAssetBundleReferences.ModsWindow.GetObject<Material>("Line");
            }

            void OnPostRender()
            {
                GL.Begin(GL.LINES);

                for(int i = 0; i < Instance._linesToDraw.Count; i++)
                {
                    _lineMaterial.SetPass(0);

                    LineInfo info = Instance._linesToDraw[i];
                    GL.Color(info.Color);
                    GL.Vertex(info.Point1);
                    GL.Vertex(info.Point2);
                }

                GL.End();
            }
        }
    }
}
