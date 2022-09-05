using UnityEngine;

namespace TerraGen
{
    [CreateAssetMenu(fileName = "TerraGenRemapSettings", menuName = "ScriptableObjects/TerraGen/Remap")]
    public class Remap : ScriptableLayerObject
    {
        public Vector2 remapInputValues = new(-1, 1);
        public bool compressOutputForTerrain;
        public Vector2 remapOutputValues = new(0, 1);
        int compressForTerrainParamId;
        int remapValuesParamId;
        public override void FindInfos(ComputeShader compute_shader)
        {
            base.FindInfos(compute_shader);
            compressForTerrainParamId = Shader.PropertyToID("compressOutputForTerrain");
            remapValuesParamId = Shader.PropertyToID("remapValues");
        }
        public override void SetParameters(ComputeShader compute_shader)
        {
            base.SetParameters(compute_shader);
            compute_shader.SetBool(compressForTerrainParamId, compressOutputForTerrain);
            compute_shader.SetVector(
                remapValuesParamId,
                new(
                    remapInputValues.x,
                    remapInputValues.y,
                    remapOutputValues.x,
                    remapOutputValues.y
                )
            );
        }
    }
}