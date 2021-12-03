using System;
using UnityEngine;

namespace SLibrary.Components
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class SimpleSliceSpriteRender : MonoBehaviour
    {
        [Header("九宫格左")] [Range(0, 1)] [SerializeField]
        private float left;

        [Header("九宫格右")] [Range(0, 1)] [SerializeField]
        private float right;

        [Header("九宫格顶")] [Range(0, 1)] [SerializeField]
        private float top;

        [Header("九宫格底")] [Range(0, 1)] [SerializeField]
        private float bottom;

        [Header("宽")] [SerializeField] private float width = 1;
        [Header("高")] [SerializeField] private float height = 1;
        
        [Header("原始宽")] [SerializeField] private float originalWidth = 1;
        [Header("原始高")] [SerializeField] private float originalHeight = 1;

        private Mesh _mesh;
        private MeshRenderer _renderer;
        private MeshFilter _filter;

        private Vector3[] points;
        private Vector2[] uvs;
        private int[] triangles;

        private float[] xGroup;
        private float[] yGroup;


        private void Start()
        {
        }

        public float Width
        {
            get => width;
            set
            {
                width = value;
                OnValidate();
            }
        }

        public float Height
        {
            get => height;
            set
            {
                height = value;
                OnValidate();
            }
        }

        public void SetSize(float w, float h)
        {
            width = w;
            height = h;
            OnValidate();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.TransformPoint(points[0]), transform.TransformPoint(points[3]));
            Gizmos.DrawLine(transform.TransformPoint(points[4]), transform.TransformPoint(points[7]));
            Gizmos.DrawLine(transform.TransformPoint(points[8]), transform.TransformPoint(points[11]));
            Gizmos.DrawLine(transform.TransformPoint(points[12]), transform.TransformPoint(points[15]));
            
            Gizmos.DrawLine(transform.TransformPoint(points[0]), transform.TransformPoint(points[12]));
            Gizmos.DrawLine(transform.TransformPoint(points[1]), transform.TransformPoint(points[13]));
            Gizmos.DrawLine(transform.TransformPoint(points[2]), transform.TransformPoint(points[14]));
            Gizmos.DrawLine(transform.TransformPoint(points[3]), transform.TransformPoint(points[15]));
            
        }

        private void OnValidate()
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<MeshRenderer>();
            }

            if (_filter == null)
            {
                _filter = GetComponent<MeshFilter>();
            }

            if (_mesh == null)
            {
                _mesh = new Mesh();
            }

            points = points ?? new Vector3[16];

            uvs = uvs ?? new Vector2[16];

            triangles = triangles ?? new int[9 * 6];
            
            xGroup = xGroup ?? new float[4];
            yGroup = yGroup ?? new float[4];

            xGroup[0] = 0;
            xGroup[1] = left;
            xGroup[2] = (1 - right);
            xGroup[3] = 1;
            
            yGroup[0] = 0;
            yGroup[1] = -top;
            yGroup[2] = -1 + bottom;
            yGroup[3] = -1;
            

            Vector3 center = new Vector3(0, 0, 0);
            Vector2 leftTopUv = new Vector2(0, 1);


            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int index = j * 4 + i;
                    float uvxAdd = xGroup[i];
                    float uvyAdd = yGroup[j];
                    uvs[index] = leftTopUv + new Vector2(uvxAdd, uvyAdd);
                    // Debug.Log($"uv {index} {uvs[index].x * 100} {uvs[index].y * 100}");
                }
            }
            
            Vector3 leftTop = center + new Vector3(-width * .5f, height * 0.5f);
            xGroup[0] = 0;
            xGroup[1] = left * originalWidth;
            xGroup[2] = width - right * originalWidth;
            xGroup[3] = width;
            
            yGroup[0] = 0;
            yGroup[1] = -top * originalHeight;
            yGroup[2] = -height + bottom * originalHeight;
            yGroup[3] = -height;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int index = j * 4 + i;
                    points[index] = leftTop + new Vector3(xGroup[i], yGroup[j]);
                }

            }

            int triIndex = 0;
            for (int i = 0; i < 4 - 1; i++)
            {
                for (int j = 0; j < 4 - 1; j++)
                {
                    int p0 = i * 4 + j;
                    int p1 = p0 + 1;
                    int p2 = p0 + 4;
                    int p3 = p1 + 4;
                    triangles[triIndex++] = p2;
                    triangles[triIndex++] = p0;
                    triangles[triIndex++] = p1;
                    triangles[triIndex++] = p1;
                    triangles[triIndex++] = p3;
                    triangles[triIndex++] = p2;
                }
            }

            _mesh.vertices = points;
            _mesh.triangles = triangles;
            _mesh.uv = uvs;

            if (_filter.sharedMesh != _mesh)
            {
                _filter.sharedMesh = _mesh;
            }
        }

        private void Awake()
        {
            OnValidate();
        }
    }
}