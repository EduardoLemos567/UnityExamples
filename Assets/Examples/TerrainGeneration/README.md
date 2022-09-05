Code to generate terrain using compute shaders.
(Its unfinished, only Noise and Remap layers are done.)

Using ScriptableObjects to pass parameters into compute shaders, 
just like unity Materials do on "drawing" shaders.

The code is scalable and customizable. 

The resulting GPU texture is passed directly into terrain data using
unity native function 'CopyActiveRenderTextureToHeightmap', taking
28 ms to finish a 513x513 texture. Part of the terrain data resides
into GPU, and part need to be downloaded into CPU memory to generate
a terrain collider and LOD.