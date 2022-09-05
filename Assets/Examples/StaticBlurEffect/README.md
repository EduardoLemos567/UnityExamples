Implementation of a static blur effect.
Good use when the background is not animated, you apply the effect only once 
and the only drawcall you need is from the RawImage. 
You can disable the background scene and save some render time.

BlurImageEffect (material/shader) is where the effect is applied. 
Its used into a Graphics.Blit call with material.

BlurBackgroundColor (material/shader) is used only to correctly tint the affected texture.