using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 视野渲染器
/// </summary>
public class FieldOfVisionRenderer : MonoBehaviour
{
    /// <summary>
    /// 渲染混合模式
    /// </summary>
    public enum BlendMode
    {
        Additive,
        Alpha,
        Multiply,
    }

    /// <summary>
    /// 视野角度
    /// </summary>
    public float angle
    {
        get { return m_Angle; }
        set { ResetAngle(value); }
    }

    /// <summary>
    /// 视野半径
    /// </summary>
    public float radius
    {
        get { return m_Radius; }
        set { ResetRadius(value); }
    }

    /// <summary>
    /// 颜色
    /// </summary>
    public Color color
    {
        get { return m_Color; }
        set { ResetColor(value); }
    }

    /// <summary>
    /// 混合模式
    /// </summary>
    public BlendMode blendMode
    {
        get { return m_BlendMode; }
    }

    /// <summary>
    /// 贴图
    /// </summary>
    public Texture2D texture
    {
        get { return m_Texture; }
        set { ResetTexture(value); }
    }

    public LayerMask cullingMask
    {
        get { return m_CullingMask; }
        set
        {
            if (m_CullingMask == value)
                return;
            m_CullingMask = value;
            if (!m_IsInitialized || !m_DepthRenderCamera) return;
            m_DepthRenderCamera.cullingMask = m_CullingMask;
        }
    }

    [SerializeField, Range(0.01f, 179.999f)] private float m_Angle;
    [SerializeField] private float m_Radius;
    [SerializeField] private LayerMask m_CullingMask;
    [SerializeField] private Color m_Color;
    [SerializeField] private BlendMode m_BlendMode;
    [SerializeField] private Texture2D m_Texture;
    
    private Mesh m_Mesh;
    public Camera m_DepthRenderCamera;
    private RenderTexture m_DepthTexture;
    private List<Vector3> m_VertexList = new List<Vector3>();
    private List<Vector2> m_UVList = new List<Vector2>();
    private List<Color> m_ColorList = new List<Color>();
    private Material m_Material;
    private int[] m_Indexes;

    private int m_CellCount;

    private bool m_IsInitialized;

    //private Matrix4x4 m_WToCMatrix;
    //private Matrix4x4 m_ProjMatrix;
    private Vector4 m_Params = default(Vector4);

    private Shader m_RenderShader;
    private Shader m_DepthRenderShader;

	void Start ()
	{
	    Init();
	}

    void OnRenderObject()
    {
//        if (m_DepthRenderCamera.worldToCameraMatrix != m_WToCMatrix)
//        {
//            m_Material.SetMatrix("internalWorldToCamera", m_DepthRenderCamera.worldToCameraMatrix);
//            m_WToCMatrix = m_DepthRenderCamera.worldToCameraMatrix;
//        }
        if (m_DepthRenderCamera.projectionMatrix.m00 != m_Params.y)
        {
            m_Params[1] = m_DepthRenderCamera.projectionMatrix.m00;
            m_Material.SetVector("_FORParams", m_Params);
        }
    }

    void Update()
    {
        if(m_Material && m_Mesh)
            Graphics.DrawMesh(m_Mesh, transform.localToWorldMatrix, m_Material, gameObject.layer);
    }

    void OnDestroy()
    {
        //if (m_MeshFilter && m_MeshFilter.sharedMesh)
        //    Destroy(m_MeshFilter.sharedMesh);
        if (m_Mesh)
            Destroy(m_Mesh);
        if (m_DepthTexture)
            Destroy(m_DepthTexture);
        m_DepthTexture = null;
        if (m_Material)
            Destroy(m_Material);
        m_Material = null;
        if (m_VertexList != null)
            m_VertexList.Clear();
        m_VertexList = null;
        if (m_UVList != null)
            m_UVList.Clear();
        m_UVList = null;
        if (m_ColorList != null)
            m_ColorList.Clear();
        m_ColorList = null;
        if (m_DepthRenderShader)
            Resources.UnloadAsset(m_DepthRenderShader);
        m_DepthRenderShader = null;
        if (m_RenderShader)
            Resources.UnloadAsset(m_RenderShader);
        m_RenderShader = null;
    }

    /// <summary>
    /// 检查Shader是否支持
    /// </summary>
    /// <returns></returns>
    private bool CheckSupport()
    {
        if (m_BlendMode == BlendMode.Additive)
            m_RenderShader = Resources.Load<Shader>("Shaders/FOR_Ofv_Additive");
        else if (m_BlendMode == BlendMode.Alpha)
            m_RenderShader = Resources.Load<Shader>("Shaders/FOR_Ofv_Alpha");
        else if (m_BlendMode == BlendMode.Multiply)
            m_RenderShader = Resources.Load<Shader>("Shaders/FOR_Ofv_Multiply");
        if (m_RenderShader == null || !m_RenderShader.isSupported)
            return false;
        m_DepthRenderShader = Resources.Load<Shader>("Shaders/RenderDepth");
        if (m_DepthRenderShader == null || !m_DepthRenderShader.isSupported)
            return false;
        return true;
    }

    private void Init()
    {
        if (!CheckSupport())
            return;

        //MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        //mr.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
        m_Material = new Material(m_RenderShader);
        //mr.sharedMaterial = m_Material;

        if (m_Texture)
            m_Material.SetTexture("_MainTex", m_Texture);

        //m_MeshFilter = gameObject.AddComponent<MeshFilter>();
        

        m_DepthRenderCamera = gameObject.AddComponent<Camera>();
        m_DepthRenderCamera.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
        m_DepthRenderCamera.backgroundColor = Color.white;
        m_DepthRenderCamera.clearFlags = CameraClearFlags.SolidColor;
        m_DepthRenderCamera.cullingMask = m_CullingMask;
        m_DepthRenderCamera.depth = 0;
        m_DepthRenderCamera.nearClipPlane = 0.01f;
        m_DepthRenderCamera.orthographic = false;
        m_DepthTexture = new RenderTexture(1024, 64, 16);
        m_DepthRenderCamera.targetTexture = m_DepthTexture;
        m_DepthRenderCamera.SetReplacementShader(m_DepthRenderShader, "RenderType");

        m_Material.SetTexture("_DepthTex", m_DepthTexture);

        RefreshMesh();
        RefreshCamera();

        //m_Material.SetMatrix("internalWorldToCamera", m_DepthRenderCamera.worldToCameraMatrix);
        m_Params[1] = m_DepthRenderCamera.projectionMatrix.m00;
        m_Material.SetVector("_FORParams", m_Params);

        //m_WToCMatrix = m_DepthRenderCamera.worldToCameraMatrix;
        //m_ProjMatrix = m_DepthRenderCamera.projectionMatrix;

        m_IsInitialized = true;
    }

    private void ResetAngle(float angle)
    {
        if (m_Angle == angle) return;
        m_Angle = angle;
        if (!m_IsInitialized) return;
        RefreshMesh();
        RefreshCamera();
    }

    private void ResetRadius(float radius)
    {
        if (m_Radius == radius) return;
        m_Radius = radius;
        if (!m_IsInitialized) return;
        RefreshMesh();
        RefreshCamera();
    }

    private void ResetColor(Color color)
    {
        if (m_Color == color) return;
        m_Color = color;
        if (!m_IsInitialized) return;
        RefreshMesh();
    }

    private void ResetTexture(Texture2D texture)
    {
        if (m_Texture == texture) return;
        m_Texture = texture;
        if (!m_IsInitialized) return;
        m_Material.SetTexture("_MainTex", m_Texture);
    }

    /// <summary>
    /// 更新视野Mesh
    /// </summary>
    private void RefreshMesh()
    {
        float currentAngle = Mathf.Clamp(m_Angle, 0, 179.999f);
        int cell = Mathf.CeilToInt(currentAngle / 18);

        float beginAngle = 90 - currentAngle / 2;

        float deltaAngle = currentAngle / cell;
        if (m_Mesh == null)
        {
            m_Mesh = new Mesh();
            m_Mesh.MarkDynamic();
        }
        m_Mesh.Clear();
        
        if (m_CellCount != cell)
        {
            m_Indexes = new int[cell*3];
            m_CellCount = cell;
            m_VertexList.Clear();
            m_UVList.Clear();
            m_ColorList.Clear();
        }
        if (m_VertexList.Count == 0)
        {
            m_VertexList.Add(Vector3.zero);
            m_UVList.Add(new Vector2(0.5f, 0.5f));
            m_ColorList.Add(m_Color);
        }
        else
        {
            m_VertexList[0] = Vector3.zero;
            m_UVList[0] = new Vector2(0.5f, 0.5f);
            m_ColorList[0] = m_Color;
        }

        for (int i = 0; i < cell; i++)
        {
            float from = beginAngle + i * deltaAngle;
            float to = beginAngle + (i + 1) * deltaAngle;

            float fromcos = Mathf.Cos(from * Mathf.Deg2Rad);
            float fromsin = Mathf.Sin(from * Mathf.Deg2Rad);
            float tocos = Mathf.Cos(to * Mathf.Deg2Rad);
            float tosin = Mathf.Sin(to * Mathf.Deg2Rad);

            float fx = fromcos * m_Radius;
            float fy = fromsin * m_Radius;
            float tx = tocos * m_Radius;
            float ty = tosin * m_Radius;

            Vector3 p1 = new Vector3(fx, 0, fy);
            Vector3 p2 = new Vector3(tx, 0, ty);
            Vector2 u1 = new Vector2(0.5f + fromcos*0.5f, 0.5f + fromsin*0.5f);
            Vector2 u2 = new Vector2(0.5f + tocos * 0.5f, 0.5f + tosin * 0.5f);

            if (m_VertexList.Count <= i*2 + 1)
            {
                m_VertexList.Add(p1);
                m_UVList.Add(u1);
                m_ColorList.Add(m_Color);
            }
            else
            {
                m_VertexList[i*2 + 1] = p1;
                m_UVList[i * 2 + 1] = u1;
                m_ColorList[i*2 + 1] = m_Color;
            }
            if (m_VertexList.Count <= i*2 + 2)
            {
                m_VertexList.Add(p2);
                m_UVList.Add(u2);
                m_ColorList.Add(m_Color);
            }
            else
            {
                m_VertexList[i*2 + 2] = p2;
                m_UVList[i * 2 + 2] = u2;
                m_ColorList[i * 2 + 2] = m_Color;
            }

            m_Indexes[i*3] = 0;
            m_Indexes[i*3 + 1] = i*2 + 2;
            m_Indexes[i*3 + 2] = i*2 + 1;

        }
        m_Mesh.SetVertices(m_VertexList);
        m_Mesh.SetUVs(0, m_UVList);
        m_Mesh.SetColors(m_ColorList);
        m_Mesh.SetTriangles(m_Indexes, 0);
    }

    /// <summary>
    /// 更新深度相机
    /// </summary>
    private void RefreshCamera()
    {
        if (m_DepthRenderCamera.farClipPlane != m_Radius)
        {
            m_Params[0] = m_Radius;
            m_Material.SetVector("_FORParams", m_Params);
        }
        m_DepthRenderCamera.farClipPlane = m_Radius;
        m_DepthRenderCamera.fieldOfView = Mathf.Atan(0.5f/m_Radius)*2*Mathf.Rad2Deg;
        m_DepthRenderCamera.aspect = 2*m_Radius*Mathf.Tan(m_Angle/2*Mathf.Deg2Rad);
    }

    void OnDrawGizmosSelected()
    {
        GizmosHelper.DrawWireSector(transform.position, m_Radius, transform.eulerAngles.y-90, m_Angle, Color.green);
    }
}
