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

        /// <summary>
        /// Adds a line to the lines to draw this frame
        /// </summary>
        /// <param name="info"></param>
        public void AddLine(LineInfo info)
        {
            LinesToDraw.Add(info);
        }
        private List<LineInfo> LinesToDraw = new List<LineInfo>();

        //private Camera[] cameras = new Camera[0];
        

        private void Update()
        {
            /*if (Camera.allCameras.Length != cameras.Length)
            {
                cameras = Camera.allCameras;
                for (int i = 0; i < cameras.Length; i++)
                {
                    if (cameras[i].targetTexture != null)
                        continue;

                    if (cameras[i].GetComponent<DebugLineDrawer>() == null)
                    {
                        cameras[i].gameObject.AddComponent<DebugLineDrawer>();
                    }

                }
            }*/

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
            
            StartCoroutine(RunAtEndOfFrame());
        }

        IEnumerator RunAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            //LinesToDraw.Clear();
            for (int i = 0; i < LinesToDraw.Count; i++)
            {
                if (LinesToDraw[i].EndTime <= Time.unscaledTime)
                {
                    LinesToDraw.RemoveAt(i);
                    i--;
                }
            }

        }

        private class DebugLineDrawer : MonoBehaviour
        {
            private Material LineMaterial;
            private void Awake()
            {
                LineMaterial = AssetLoader.GetObjectFromFile<Material>("modswindow", "Line", "Clone Drone in the Danger Zone_Data/");
            }

            private void OnPostRender()
            {
                //GL.PushMatrix();
                
                //GL.MultMatrix(transform.localToWorldMatrix);
                GL.Begin(GL.LINES);

                for(int i = 0; i < DebugLineDrawingManager.Instance.LinesToDraw.Count; i++)
                {
                    LineMaterial.SetPass(0);

                    LineInfo info = DebugLineDrawingManager.Instance.LinesToDraw[i];
                    GL.Color(info.Color);
                    GL.Vertex(info.Point1);
                    GL.Vertex(info.Point2);
                }

                GL.End();
                //GL.PopMatrix();
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
