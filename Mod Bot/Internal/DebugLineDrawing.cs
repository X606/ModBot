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
                FirstPersonMover player = CharacterTracker.Instance.GetPlayer();
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
                _lineMaterial = AssetLoader.GetObjectFromFile<Material>("modswindow", "Line", "Clone Drone in the Danger Zone_Data/");
            }

            void OnPostRender()
            {
                GL.Begin(GL.LINES);

                for(int i = 0; i < DebugLineDrawingManager.Instance._linesToDraw.Count; i++)
                {
                    _lineMaterial.SetPass(0);

                    LineInfo info = DebugLineDrawingManager.Instance._linesToDraw[i];
                    GL.Color(info.Color);
                    GL.Vertex(info.Point1);
                    GL.Vertex(info.Point2);
                }

                GL.End();
            }
        }

        /// <summary>
        /// Describes a line in 3D space
        /// </summary>
        public class LineInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LineInfo"/> class
            /// </summary>
            /// <param name="point1"></param>
            /// <param name="point2"></param>
            /// <param name="color"></param>
            /// <param name="endTime"></param>
            public LineInfo(Vector3 point1, Vector3 point2, Color color, float endTime)
            {
                Point1 = point1;
                Point2 = point2;
                Color = color;
                EndTime = endTime;
            }

            /// <summary>
            /// The start point of the line
            /// </summary>
            public Vector3 Point1;

            /// <summary>
            /// The end point of the line
            /// </summary>
            public Vector3 Point2;

            /// <summary>
            /// The <see cref="UnityEngine.Color"/> of the line
            /// </summary>
            public Color Color;

            /// <summary>
            /// The time we want to stop displaying the line
            /// </summary>
            public float EndTime;
        }
    }
}
