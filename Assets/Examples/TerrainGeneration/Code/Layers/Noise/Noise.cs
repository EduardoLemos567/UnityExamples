using UnityEngine;

namespace TerraGen
{
    [CreateAssetMenu(fileName = "TerraGenNoiseSettings", menuName = "ScriptableObjects/TerraGen/Noise")]
    public class Noise : ScriptableLayerObject
    {
        public enum NOISE_TYPE
        {
            RANDOM,
            SIMPLEX,
            PERLIN,
            CELLULAR
        }
        public NOISE_TYPE noiseType;
        public Vector2 noiseRange = new(0, 1);
        public Vector2 noiseOffset;
        public Vector2 noiseScale = new(1, 1);
        int noiseTypeParamId;
        int noiseRangeParamId;
        int noiseOffsetScaleParamId;
        public override void FindInfos(ComputeShader compute_shader)
        {
            base.FindInfos(compute_shader);
            noiseTypeParamId = Shader.PropertyToID("noiseType");
            noiseRangeParamId = Shader.PropertyToID("noiseRange");
            noiseOffsetScaleParamId = Shader.PropertyToID("noiseOffsetScale");
        }
        public override void SetParameters(ComputeShader compute_shader)
        {
            base.SetParameters(compute_shader);
            compute_shader.SetInt(noiseTypeParamId, (int)noiseType);
            compute_shader.SetVector(noiseRangeParamId, new(noiseRange.x, noiseRange.y));
            compute_shader.SetVector(
                noiseOffsetScaleParamId,
                new(
                    noiseOffset.x,
                    noiseOffset.y,
                    noiseScale.x,
                    noiseScale.y
                )
            );
        }
    }
}