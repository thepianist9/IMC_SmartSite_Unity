using IATK;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.Linq;
using UnityEngine.Rendering;
public class ClickAndBrush : MonoBehaviour
{
    [SerializeField]
    public ComputeShader computeShader;

    private int kernelComputeBrushTexture;
    private int kernelComputeBrushedIndices;

    [SerializeField]
    public List<Visualisation> brushingVisualisations;
    [SerializeField]
    public List<LinkingVisualisations> brushedLinkingVisualisations;

        [SerializeField]
    public List<int> brushedIndices;


    [SerializeField]
    public Material myRenderMaterial;
    [SerializeField]
    public bool isBrushing;
    [SerializeField]
    public Color brushColor = Color.red;
    [SerializeField]
    [Range(0f, 1f)]
    public float brushRadius;
    [SerializeField]
    public bool showBrush = false;
    [SerializeField]
    [Range(1f, 10f)]
    public float brushSizeFactor = 1f;

    [SerializeField]
    public Transform input1;

    private bool hasInitialised = false;

    private RenderTexture brushedIndicesTexture;
    private int texSize;

    private ComputeBuffer dataBuffer;
    private ComputeBuffer filteredIndicesBuffer;
    private ComputeBuffer brushedIndicesBuffer;
    private bool hasFreeBrushReset = false;
    private AsyncGPUReadbackRequest brushedIndicesRequest;


    [SerializeField]
    public BrushType BRUSH_TYPE;
    public enum BrushType
    {
        SPHERE = 0,
        BOX
    };

    [SerializeField]
    public SelectionType SELECTION_TYPE;
    public enum SelectionType
    {
        FREE = 0,
        ADD,
        SUBTRACT
    }


    private void Start()
    {
        InitialiseShaders();
    }

    /// <summary>
    /// Initialises the indices for the kernels in the compute shader.
    /// </summary>
    private void InitialiseShaders()
    {
        kernelComputeBrushTexture = computeShader.FindKernel("CSMain");
        kernelComputeBrushedIndices = computeShader.FindKernel("ComputeBrushedIndicesArray");
    }


    /// <summary>
    /// Algorithm Click to Brush:
    /// 1. Initialize the shaders and indices for the kernels in the compute shader.
    /// 2. Initialize the buffers and textures necessary for the brushing and linking to work.
    /// 3. Update the computebuffers with the values specific to the currently brushed visualisation.
    /// </summary>
    public void Update()
    {
        if (isBrushing && brushingVisualisations.Count > 0)
        {
            if (hasInitialised)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    SELECTION_TYPE = SelectionType.ADD;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    SELECTION_TYPE = SelectionType.FREE;
                }
                UpdateBrushTexture();

                UpdateBrushedIndices();
            }
            else
            {
                InitialiseBuffersAndTextures(brushingVisualisations[0].dataSource.DataCount);
            }
        }

    }
    private void UpdateBrushedIndices()
    {
        // Wait for request to finish
        if (brushedIndicesRequest.done)
        {
            // Get values from request
            if (!brushedIndicesRequest.hasError)
            {
                brushedIndices = brushedIndicesRequest.GetData<int>().ToList();
            }

            // Dispatch again
            computeShader.Dispatch(kernelComputeBrushedIndices, Mathf.CeilToInt(brushedIndicesBuffer.count / 32f), 1, 1);
            brushedIndicesRequest = AsyncGPUReadback.Request(brushedIndicesBuffer);
        }
    }

    /// <summary>
    /// transform of already existing 3d object
    /// but when 
    private void UpdateBrushTexture()
    {
        Vector3 projectedPointer1;
        Vector3 projectedPointer2;

        computeShader.SetInt("BrushMode", (int)BRUSH_TYPE);
        computeShader.SetInt("SelectionMode", (int)SELECTION_TYPE);

        hasFreeBrushReset = false;

        foreach (var vis in brushingVisualisations)
        {
            UpdateComputeBuffers(vis);

           //get input position of virtual 3d object
            projectedPointer1 = vis.transform.InverseTransformPoint(input1.transform.position);
            //pass the position of the virtual 3d object to the compute shader
            computeShader.SetFloats("pointer1", projectedPointer1.x, projectedPointer1.y, projectedPointer1.z);


            

            //set the filters and normalisation values of the brushing visualisation to the computer shader
            computeShader.SetFloat("_MinNormX", vis.xDimension.minScale);
            computeShader.SetFloat("_MaxNormX", vis.xDimension.maxScale);
            computeShader.SetFloat("_MinNormY", vis.yDimension.minScale);
            computeShader.SetFloat("_MaxNormY", vis.yDimension.maxScale);
            computeShader.SetFloat("_MinNormZ", vis.zDimension.minScale);
            computeShader.SetFloat("_MaxNormZ", vis.zDimension.maxScale);

            computeShader.SetFloat("_MinX", vis.xDimension.minFilter);
            computeShader.SetFloat("_MaxX", vis.xDimension.maxFilter);
            computeShader.SetFloat("_MinY", vis.yDimension.minFilter);
            computeShader.SetFloat("_MaxY", vis.yDimension.maxFilter);
            computeShader.SetFloat("_MinZ", vis.zDimension.minFilter);
            computeShader.SetFloat("_MaxZ", vis.zDimension.maxFilter);

            computeShader.SetFloat("RadiusSphere", brushRadius);

            computeShader.SetFloat("width", vis.width);
            computeShader.SetFloat("height", vis.height);
            computeShader.SetFloat("depth", vis.depth);

            // Tell the shader whether or not the visualisation's points have already been reset by a previous brush, required to allow for
            // multiple visualisations to be brushed with the free selection tool
            if (SELECTION_TYPE == SelectionType.FREE)
                computeShader.SetBool("HasFreeBrushReset", hasFreeBrushReset);

            // Run the compute shader
            computeShader.Dispatch(kernelComputeBrushTexture, Mathf.CeilToInt(texSize / 32f), Mathf.CeilToInt(texSize / 32f), 1);

            foreach (var view in vis.theVisualizationObject.viewList)
            {
                view.BigMesh.SharedMaterial.SetTexture("_BrushedTexture", brushedIndicesTexture);
                view.BigMesh.SharedMaterial.SetFloat("_DataWidth", texSize);
                view.BigMesh.SharedMaterial.SetFloat("_DataHeight", texSize);
                view.BigMesh.SharedMaterial.SetFloat("_ShowBrush", Convert.ToSingle(showBrush));
                view.BigMesh.SharedMaterial.SetColor("_BrushColor", brushColor);
            }

            hasFreeBrushReset = true;
        }

        foreach (var linkingVis in brushedLinkingVisualisations)
        {
            linkingVis.View.BigMesh.SharedMaterial.SetTexture("_BrushedTexture", brushedIndicesTexture);
            linkingVis.View.BigMesh.SharedMaterial.SetFloat("_DataWidth", texSize);
            linkingVis.View.BigMesh.SharedMaterial.SetFloat("_DataHeight", texSize);
            linkingVis.View.BigMesh.SharedMaterial.SetFloat("_ShowBrush", Convert.ToSingle(showBrush));
            linkingVis.View.BigMesh.SharedMaterial.SetColor("_BrushColor", brushColor);
        }
    }
    /// <summary>
    /// Initialises the buffers and textures necessary for the brushing and linking to work.
    /// </summary>
    /// <param name="dataCount"></param>
    private void InitialiseBuffersAndTextures(int dataCount)
    {
        dataBuffer = new ComputeBuffer(dataCount, 12);
        dataBuffer.SetData(new Vector3[dataCount]);
        computeShader.SetBuffer(kernelComputeBrushTexture, "dataBuffer", dataBuffer);

        filteredIndicesBuffer = new ComputeBuffer(dataCount, 4);
        filteredIndicesBuffer.SetData(new float[dataCount]);
        computeShader.SetBuffer(kernelComputeBrushTexture, "filteredIndicesBuffer", filteredIndicesBuffer);

        brushedIndicesBuffer = new ComputeBuffer(dataCount, 4);
        brushedIndicesBuffer.SetData(Enumerable.Repeat(-1, dataCount).ToArray());
        computeShader.SetBuffer(kernelComputeBrushedIndices, "brushedIndicesBuffer", brushedIndicesBuffer);

        texSize = NextPowerOf2((int)Mathf.Sqrt(dataCount));
        brushedIndicesTexture = new RenderTexture(texSize, texSize, 24);
        brushedIndicesTexture.enableRandomWrite = true;
        brushedIndicesTexture.filterMode = FilterMode.Point;
        brushedIndicesTexture.Create();

        myRenderMaterial.SetTexture("_MainTex", brushedIndicesTexture);

        computeShader.SetFloat("_size", texSize);
        computeShader.SetTexture(kernelComputeBrushTexture, "Result", brushedIndicesTexture);
        computeShader.SetTexture(kernelComputeBrushedIndices, "Result", brushedIndicesTexture);

        hasInitialised = true;
    }

    /// <summary>
    /// Finds the next power of 2 for a given number.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private int NextPowerOf2(int number)
    {
        int pos = 0;

        while (number > 0)
        {
            pos++;
            number = number >> 1;
        }
        return (int)Mathf.Pow(2, pos);
    }

    /// <summary>
    /// Updates the computebuffers with the values specific to the currently brushed visualisation.
    /// FOR DATA EXTRACTION
    /// </summary>
    /// <param name="visualisation"></param>
    public void UpdateComputeBuffers(Visualisation visualisation)
    {
        if (visualisation.visualisationType == AbstractVisualisation.VisualisationTypes.SCATTERPLOT)
        {
            dataBuffer.SetData(visualisation.theVisualizationObject.viewList[0].BigMesh.getBigMeshVertices());
            computeShader.SetBuffer(kernelComputeBrushTexture, "dataBuffer", dataBuffer);

            filteredIndicesBuffer.SetData(visualisation.theVisualizationObject.viewList[0].GetFilterChannel());
            computeShader.SetBuffer(kernelComputeBrushTexture, "filteredIndicesBuffer", filteredIndicesBuffer);
        }
    }

    /// <summary>
    /// Add all visualisations with tag "BrushingVisualisation" to the list of brushing visualisations.
    /// Create n-1 Linking visualisations dynamically and add the source and target linked visualisations to linking visualisations.
    /// 
}
