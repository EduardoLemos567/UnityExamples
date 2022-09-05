using UnityEngine;
using UnityEngine.Assertions;

namespace TerraGen
{
    public abstract class ScriptableLayerObject : ScriptableObject
    {
        public int KernelId { get; private set; }
        public string KernelName => $"{GetType().Name}Kernel";
        protected Vector3 groupSize;
        public virtual void FindInfos(ComputeShader compute_shader)
        {
            KernelId = compute_shader.FindKernel(KernelName);
            compute_shader.GetKernelThreadGroupSizes(KernelId, out var group_x, out var group_y, out var group_z);
            groupSize = new Vector3(group_x, group_y, group_z);
        }
        public virtual void SetParameters(ComputeShader compute_shader) { }
        public virtual void Execute(ComputeShader compute_shader, Vector2 texture_size)
        {
            Assert.IsTrue(groupSize.x > 0 && groupSize.y > 0 && groupSize.z > 0, "groupSize components need to be greater than zero");
            Assert.IsTrue(texture_size.x > 0 && texture_size.y > 0, "texture_size components need to be greater than zero");
            compute_shader.Dispatch(
                kernelIndex: KernelId,
                threadGroupsX: Mathf.CeilToInt(texture_size.x / groupSize.x),
                threadGroupsY: Mathf.CeilToInt(texture_size.y / groupSize.y),
                threadGroupsZ: 1
            );
            Debug.Log($"executing {KernelName}");
        }
        public virtual void Cleanup(ComputeShader compute_shader) { }
    }
}