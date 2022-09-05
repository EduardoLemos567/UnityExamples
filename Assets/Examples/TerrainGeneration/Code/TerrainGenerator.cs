using System.Collections;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TerraGen
{
    [RequireComponent(typeof(Terrain))]
    public class TerrainGenerator : MonoBehaviour
    {
        public ScriptableLayerObject[] scriptableLayers;
        [SerializeField] ComputeShader computeShader;
        Terrain targetTerrain;
        RenderTexture heightmap;
        Vector2 heightmapSize;
        int textureParamId;
        int textureSizeParamId;
        IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            ApplyLayers();
        }
        void FindInfos()
        {
            if (targetTerrain == null) { targetTerrain = GetComponent<Terrain>(); }
            if (heightmap != null) { heightmap.Release(); }
            heightmap = new(targetTerrain.terrainData.heightmapTexture);
            heightmap.enableRandomWrite = true;
            heightmapSize = new(heightmap.width, heightmap.height);
            textureParamId = Shader.PropertyToID("resultTexture");
            textureSizeParamId = Shader.PropertyToID("textureSize");
            foreach (var layer in scriptableLayers)
            { layer.FindInfos(computeShader); }
        }
        void SetParameters()
        {
            computeShader.SetVector(textureSizeParamId, new Vector4(heightmapSize.x, heightmapSize.y));
            foreach (var layer in scriptableLayers)
            {
                layer.SetParameters(computeShader);
                computeShader.SetTexture(layer.KernelId, textureParamId, heightmap);
            }
        }
        void Execute()
        {
            foreach (var layer in scriptableLayers)
            { layer.Execute(computeShader, heightmapSize); }
        }
        void Cleanup()
        {
            foreach (var layer in scriptableLayers)
            { layer.Cleanup(computeShader); }
        }
        void CopyTextureIntoTerrain()
        {
            var temp = RenderTexture.active;
            var source_region = new RectInt(Vector2Int.zero, new(heightmap.width, heightmap.height));
            var dest_origin = Vector2Int.zero;
            RenderTexture.active = heightmap;
            targetTerrain.terrainData.CopyActiveRenderTextureToHeightmap(
                sourceRect: source_region,
                dest: dest_origin,
                syncControl: TerrainHeightmapSyncControl.HeightAndLod
            );
            RenderTexture.active = temp;
        }
        void ApplyLayers()
        {
            print(computeShader.name);
            Assert.IsTrue(computeShader != null && computeShader.name == "TerraGen", "Need to set the correct reference to the TerraGen compute shader.");
            if (scriptableLayers.Length == 0) { return; }
            print($"Executing {computeShader.name} compute shader...");
            var clock = Stopwatch.StartNew();
            FindInfos();
            SetParameters();
            Execute();
            Cleanup();
            CopyTextureIntoTerrain();
            clock.Stop();
            print($"Finished applying in {clock.ElapsedMilliseconds} ms");

        }
        [CustomEditor(typeof(TerrainGenerator))]
        public class TerrainGeneratorEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                var targeted = (TerrainGenerator)target;
                if (GUILayout.Button("Apply Layers")) { targeted.ApplyLayers(); }
                DrawDefaultInspector();
            }
        }
    }
}