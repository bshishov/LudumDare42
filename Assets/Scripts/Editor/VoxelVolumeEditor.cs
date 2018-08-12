using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    public class VoxelVolumeEditor : EditorWindow
    {
        private GameObject _activeObject;
        private VoxelObject _voxelObject;
        private Vector2Int _offsetPos;
        private int _y = 0;
        private Vector2 _scrollPosition;

        private const float CellSize = 15f;


        private GUIStyle _emptyVoxel;
        private GUIStyle _filledVoxel;
        private GUIStyle _pivotEmptyVoxel;
        private GUIStyle _pivotFilledVoxel;

        private GUIStyle EmptyVoxel
        {
            get
            {
                if (_emptyVoxel == null)
                {
                    _emptyVoxel = new GUIStyle(GUI.skin.box);
                    _emptyVoxel.normal.background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 1f));
                }

                return _emptyVoxel;
            }
        }
        private GUIStyle FilledVoxel
        {
            get
            {
                if (_filledVoxel == null)
                {
                    _filledVoxel = new GUIStyle("GUI.skin.button");
                    _filledVoxel.normal.background = MakeTex(2, 2, new Color(1f, 1f, 1f, 1f));
                }

                return _filledVoxel;
            }
        }
        private GUIStyle PivotEmptyVoxel
        {
            get
            {
                if (_pivotEmptyVoxel == null)
                {
                    _pivotEmptyVoxel = new GUIStyle("GUI.skin.button");
                    _pivotEmptyVoxel.normal.background = MakeTex(2, 2, new Color(0.6f, 0.2f, 0.2f, 1f));
                }

                return _pivotEmptyVoxel;
            }
        }
        private GUIStyle PivotFilledVoxel
        {
            get
            {
                if (_pivotFilledVoxel == null)
                {
                    _pivotFilledVoxel = new GUIStyle("GUI.skin.button");
                    _pivotFilledVoxel.normal.background = MakeTex(2, 2, new Color(1f, 0f, 0f, 1f));
                }

                return _pivotFilledVoxel;
            }
        }

        [MenuItem("Window/Voxel editor")]
        public static void ShowWindow()
        {
            var wnd = EditorWindow.GetWindow(typeof(VoxelVolumeEditor));
            Selection.selectionChanged += delegate
            {
                wnd.Repaint();
            };
        }

        void OnGUI()
        {
            var activeObject = Selection.activeGameObject;
            if(activeObject == null)
                return;

            _voxelObject = activeObject.GetComponent<VoxelObject>();
            if(_voxelObject == null)
                return;

            /*
            var obj = EditorGUILayout.ObjectField(_voxelObject, typeof(VoxelObject), 
                allowSceneObjects: true);

            if (obj != null)
                _voxelObject = obj as VoxelObject;*/


            _offsetPos = EditorGUILayout.Vector2IntField("View", _offsetPos);
            _y = EditorGUILayout.IntSlider("Y", _y, 0, 10);

            if (_voxelObject == null)
                return;
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginScrollView(_scrollPosition);
            for (var z = 0; z < 20; z++)
            {
                for (var x = 0; x < 20; x++)
                {
                    var voxelPos = new Vector3Int(x - _offsetPos.x, _y, 20 - z - _offsetPos.y);
                    var isVoxel = _voxelObject.Volume.HasVoxelAt(voxelPos);
                    var isPivot = voxelPos == _voxelObject.Volume.Pivot;
                    var style = isVoxel ? FilledVoxel : EmptyVoxel;
                    if (isPivot && isVoxel)
                        style = PivotFilledVoxel;
                    if (isPivot && !isVoxel)
                        style = PivotEmptyVoxel;


                    if (GUI.Button(new Rect(x * CellSize + x, z * CellSize + z, CellSize, CellSize), GUIContent.none, style))
                    {
                        if (isVoxel)
                            _voxelObject.Volume.RemoveAt(voxelPos);
                        else
                            _voxelObject.Volume.SetAt(voxelPos, new VoxelData());
                    }
                }
            }
            EditorGUILayout.EndScrollView();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_voxelObject);
                //UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                //AssetDatabase.SaveAssets();
            }
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
