# UnityExamples (using Unity 2022.1.15f1)

## Water example:
A good looking water effect (not finished) applied on a simple quad.
Meaning it looks good with 4 vertices only.



## Static Blur Effect example:
Implementation of a static blur effect.
Good use when the background is not animated, you apply the effect only once 
and the only drawcall you need is from the RawImage. 
You can disable the background scene and save some render time.

BlurImageEffect (material/shader) is where the effect is applied. 
Its used into a Graphics.Blit call with material.

BlurBackgroundColor (material/shader) is used only to correctly tint the affected texture.



## Terrain Generation example:
Code to generate terrain using compute shaders.
(Its unfinished, only Noise and Remap layers are done.)

This example is based on WorldKit from vincekieft (https://github.com/vincekieft/WorldKit).
The idea of using layers to separate compute shaders kernel and pass different arguments into it was all his.

I changed it into ScriptableObjects, so you can keep your parameters on a prefab object, just like Materials 
do with normal shaders.
You can also add multiple layers into a TerrainGenerator, applying different effects in chain.
The code is scalable and customizable. 

Vince's solution downloads the compute shader buffer into a CPU memory array and them send it to 
terrainData using 'SetHeights', in which the data is uploaded back into GPU.

My solution pass GPU texture directly into terrain data using unity native function 
'CopyActiveRenderTextureToHeightmap', taking 28 ms to finish a 513x513 texture. 
Part of the terrain data resides into GPU, and part need to be downloaded into CPU 
memory to generate a terrain collider and LOD.



## Scripts example:
Some script examples collected from multiple solo projects.
