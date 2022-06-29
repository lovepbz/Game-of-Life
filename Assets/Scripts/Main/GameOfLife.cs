using UnityEngine;
using Random = UnityEngine.Random;

namespace Main
{
    public class GameOfLife : MonoBehaviour
    {
        public ComputeShader computeShader;
        public Material material;

        public int frameRate = 0;

        public int width = 32;
        public int height = 18;

        public int initialAliveRate = 20;

        private ComputeBuffer data;
        private ComputeBuffer nextData;
        private ComputeBuffer vertexDataBuffer;

        private int updateKernel;
        private int computeVertexKernel;
        private int dataId;
        private int nextDataId;

        private int[] array;

        private int frameCount = 0;

        void Start()
        {
            if (frameRate > 0)
            {
                Application.targetFrameRate = frameRate;
            }

            float[] posOffset =
            {
                0, 0, 0, 0,
                0, 1, 0, 0,
                1, 1, 0, 0,
                1, 0, 0, 0
            };
            computeShader.SetFloats("posOffset", posOffset);
            computeShader.SetInt("width", width);
            computeShader.SetInt("height", height);

            data = new ComputeBuffer(width * height, 4);
            nextData = new ComputeBuffer(width * height, 4);
            vertexDataBuffer = new ComputeBuffer(width * height * 4, 4 * 7);

            array = new int[width * height];

            updateKernel = computeShader.FindKernel("Update");
            computeVertexKernel = computeShader.FindKernel("ComputeVertex");

            computeShader.SetBuffer(computeVertexKernel, "vertexDataBuffer", vertexDataBuffer);
            material.SetBuffer("vertexDataBuffer", vertexDataBuffer);

            transform.position += new Vector3(width / 2f, height / 2f, 0);
            Camera.main.orthographicSize = height / 2f;

            dataId = Shader.PropertyToID("data");
            nextDataId = Shader.PropertyToID("nextData");

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Random.Range(0, 100) < initialAliveRate ? 1 : 0;
            }

            data.SetData(array);
        }

        void Update()
        {
            if (frameCount % 2 == 0)
            {
                computeShader.SetBuffer(updateKernel, dataId, data);
                computeShader.SetBuffer(updateKernel, nextDataId, nextData);
                computeShader.SetBuffer(computeVertexKernel, dataId, data);
            }
            else
            {
                computeShader.SetBuffer(updateKernel, dataId, nextData);
                computeShader.SetBuffer(updateKernel, nextDataId, data);
                computeShader.SetBuffer(computeVertexKernel, dataId, nextData);
            }

            computeShader.Dispatch(updateKernel, width / 8 + 8, height / 8 + 8, 1);
            computeShader.Dispatch(computeVertexKernel, width / 2 + 2, height / 8 + 8, 1);

            frameCount++;
        }

        private void OnRenderObject()
        {
            material.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Quads, width * height * 4);
        }

        private void OnDestroy()
        {
            data.Dispose();
            nextData.Dispose();
            vertexDataBuffer.Dispose();
        }
    }
}