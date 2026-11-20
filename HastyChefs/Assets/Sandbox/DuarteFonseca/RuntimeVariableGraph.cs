using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RuntimeVariableGraph : MonoBehaviour
{
    [Header("Target Variable")]
    [Tooltip("Drag the component containing the variable here")]
    [SerializeField] private MonoBehaviour targetScript;
    [Tooltip("Exact name of the float, int, or bool field/property (can be private)")]
    [SerializeField] private string variableName = "verticalRotation";

    [Header("Graph Layout")]
    [SerializeField] private Rect graphRect = new Rect(20f, 20f, 320f, 140f);
    [SerializeField] private int maxSamples = 200;
    [SerializeField] private bool autoScaleY = true;
    [SerializeField] private float fixedMinY = -10f;
    [SerializeField] private float fixedMaxY = 10f;

    [Header("Colors")]
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.65f);
    [SerializeField] private Color lineColor = Color.green;
    [SerializeField] private Color zeroLineColor = new Color(1f, 1f, 1f, 0.25f);

    private readonly List<float> _samples = new List<float>();
    private FieldInfo _fieldInfo;
    private PropertyInfo _propertyInfo;
    private Material _lineMaterial;
    private Texture2D _bgTexture;
    private string _errorMessage;

    private void Awake()
    {
        CreateDrawingResources();
        BindVariable();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            BindVariable();
    }

    private void BindVariable()
    {
        _fieldInfo = null;
        _propertyInfo = null;
        _errorMessage = null;
        _samples.Clear();

        if (targetScript == null || string.IsNullOrWhiteSpace(variableName))
        {
            _errorMessage = "Assign a Target Script and Variable Name.";
            return;
        }

        Type type = targetScript.GetType();
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        _fieldInfo = type.GetField(variableName, flags);
        if (_fieldInfo == null)
            _propertyInfo = type.GetProperty(variableName, flags);

        if (_fieldInfo == null && _propertyInfo == null)
            _errorMessage = $"Variable '{variableName}' not found on {type.Name}.";
    }

    private void Update()
    {
        if (_fieldInfo == null && _propertyInfo == null) return;

        object rawValue = _fieldInfo != null
            ? _fieldInfo.GetValue(targetScript)
            : _propertyInfo.GetValue(targetScript);

        if (TryConvertToFloat(rawValue, out float value))
        {
            _samples.Add(value);
            if (_samples.Count > maxSamples)
                _samples.RemoveAt(0);
        }
    }

    private bool TryConvertToFloat(object val, out float result)
    {
        switch (val)
        {
            case float f: result = f; return true;
            case int i: result = i; return true;
            case double d: result = (float)d; return true;
            case bool b: result = b ? 1f : 0f; return true;
            default:
                _errorMessage = $"Unsupported type: {val?.GetType().Name ?? "null"}";
                result = 0f;
                return false;
        }
    }

    private void OnGUI()
    {
        if (Event.current.type != EventType.Repaint) return;

        // Draw background box
        GUI.color = backgroundColor;
        GUI.DrawTexture(graphRect, _bgTexture);
        GUI.color = Color.white;

        if (!string.IsNullOrEmpty(_errorMessage))
        {
            GUI.Label(new Rect(graphRect.x + 8, graphRect.y + 8, graphRect.width - 16, 40), _errorMessage);
            return;
        }

        if (_samples.Count < 2) return;

        float minY = fixedMinY;
        float maxY = fixedMaxY;

        if (autoScaleY)
        {
            minY = float.MaxValue;
            maxY = float.MinValue;
            for (int i = 0; i < _samples.Count; i++)
            {
                if (_samples[i] < minY) minY = _samples[i];
                if (_samples[i] > maxY) maxY = _samples[i];
            }
            if (Mathf.Approximately(minY, maxY))
            {
                minY -= 1f;
                maxY += 1f;
            }
        }

        DrawGraphLines(minY, maxY);

        // Draw text readout
        float latest = _samples[_samples.Count - 1];
        GUI.Label(new Rect(graphRect.x + 8, graphRect.y + 4, graphRect.width - 16, 22),
            $"{variableName}: {latest:F3}");
        GUI.Label(new Rect(graphRect.x + 8, graphRect.yMax - 20, graphRect.width - 16, 20),
            $"Min: {minY:F2} | Max: {maxY:F2}");
    }

    private void DrawGraphLines(float minY, float maxY)
    {
        _lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);
        GL.Begin(GL.LINES);

        // Draw horizontal zero-reference line if 0 is within visible range
        if (minY < 0f && maxY > 0f)
        {
            float zeroNorm = Mathf.InverseLerp(minY, maxY, 0f);
            float zeroY = Mathf.Lerp(graphRect.yMax, graphRect.yMin, zeroNorm);
            GL.Color(zeroLineColor);
            GL.Vertex3(graphRect.xMin, zeroY, 0f);
            GL.Vertex3(graphRect.xMax, zeroY, 0f);
        }

        // Draw data curve
        GL.Color(lineColor);
        float xStep = graphRect.width / Mathf.Max(1, maxSamples - 1);

        for (int i = 0; i < _samples.Count - 1; i++)
        {
            float x1 = graphRect.xMin + i * xStep;
            float x2 = graphRect.xMin + (i + 1) * xStep;

            float y1 = Mathf.Lerp(graphRect.yMax, graphRect.yMin, Mathf.InverseLerp(minY, maxY, _samples[i]));
            float y2 = Mathf.Lerp(graphRect.yMax, graphRect.yMin, Mathf.InverseLerp(minY, maxY, _samples[i + 1]));

            GL.Vertex3(x1, y1, 0f);
            GL.Vertex3(x2, y2, 0f);
        }

        GL.End();
        GL.PopMatrix();
    }

    private void CreateDrawingResources()
    {
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        _lineMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
        _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        _lineMaterial.SetInt("_ZWrite", 0);

        _bgTexture = new Texture2D(1, 1);
        _bgTexture.SetPixel(0, 0, Color.white);
        _bgTexture.Apply();
    }

    private void OnDestroy()
    {
        if (_lineMaterial != null) Destroy(_lineMaterial);
        if (_bgTexture != null) Destroy(_bgTexture);
    }
}