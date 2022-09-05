using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(RawImage))]
    public class BlurBackground : MonoBehaviour
    {
        [SerializeField] Material blurMaterial;
        [SerializeField] int repeatEffect = 2;
        [SerializeField, Range(0, 1)] float screenScale;
        [SerializeField] RawImage backgroundPanel;
        const string DIRECTION_SHADER_PARAM = "_Direction";
        IEnumerator Start()
        {
            backgroundPanel.enabled = false;
            yield return new WaitForSeconds(2);
            backgroundPanel.enabled = true;
            yield return UpdateTexture();
        }
        public IEnumerator UpdateTexture()
        {
            yield return null;
            var screen_size = new Vector2(Screen.width, Screen.height);
            var result_texture = backgroundPanel.texture as RenderTexture;
            var result_texture_size = screen_size * screenScale;
            // Setup result texture
            {
                // If RenderTexture is incorrect, (release it if needed) create a new
                if (result_texture == null
                    || result_texture_size.x != result_texture.width
                    || result_texture_size.y != result_texture.height
                )
                {
                    result_texture?.Release();
                    result_texture = new RenderTexture(
                        (int)result_texture_size.x,
                        (int)result_texture_size.y,
                        24,
                        RenderTextureFormat.ARGB32
                    );
                    backgroundPanel.texture = result_texture;
                }
            }
            // Render camera into result_texture
            {
                var main_camera = Camera.main;
                main_camera.targetTexture = result_texture;
                main_camera.Render();
                main_camera.targetTexture = null;
            }
            {
                // Apply effect result_texture -> temp_texture -> result_texture
                var temp_texture = RenderTexture.GetTemporary(result_texture.descriptor);
                try
                {
                    for (int i = 0; i < repeatEffect; i++)
                    {
                        blurMaterial.SetVector(DIRECTION_SHADER_PARAM, new(repeatEffect - i, 0, 0, 0));
                        Graphics.Blit(
                            source: result_texture,
                            dest: temp_texture,
                            blurMaterial
                        );
                        blurMaterial.SetVector(DIRECTION_SHADER_PARAM, new(0, repeatEffect - i, 0, 0));
                        Graphics.Blit(
                            source: temp_texture,
                            dest: result_texture,
                            blurMaterial
                        );
                    }
                }
                finally
                {
                    RenderTexture.ReleaseTemporary(temp_texture);
                }
            }
        }
    }
}