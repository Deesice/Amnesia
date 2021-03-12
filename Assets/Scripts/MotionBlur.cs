using System;
using System.Collections;
using UnityEngine;

// This class implements simple ghosting type Motion Blur.
// If Extra Blur is selected, the scene will allways be a little blurred,
// as it is scaled to a smaller resolution.
// The effect works by accumulating the previous frames in an accumulation
// texture.
namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Blur/Motion Blur (Color Accumulation)")]
    [RequireComponent(typeof(Camera))]
    public class MotionBlur : ImageEffectBase
    {
        public float blurAmount = 0.8f;
        public bool extraBlur = false;

        private RenderTexture accumTexture;
        static MotionBlur instance;
        IEnumerator coroutine = null;
        private void Awake()
        {
            instance = this;
            enabled = false;
        }

        override protected void Start()
        {
            base.Start();
        }
        public static void FadeImageTrailTo(float afAmount, float afSpeed)
        {
            instance.enabled = true;
            if (instance.coroutine != null)
                instance.StopCoroutine(instance.coroutine);
            instance.coroutine = instance.Fading(afAmount, afSpeed);
            instance.StartCoroutine(instance.coroutine);
        }
        IEnumerator Fading(float afAmount, float afSpeed)
        {
            bool flag = true;
            while (flag)
            {
                if (afAmount < blurAmount)
                {
                    blurAmount -= afSpeed * Time.deltaTime;
                    if (blurAmount <= afAmount)
                    {
                        blurAmount = afAmount;
                        flag = false;
                    }
                }
                else
                {
                    blurAmount += afSpeed * Time.deltaTime;
                    if (blurAmount >= afAmount)
                    {
                        blurAmount = afAmount;
                        flag = false;
                    }
                }
                yield return null;
            }
            coroutine = null;
            if (afAmount == 0)
                enabled = false;
        }

        override protected void OnDisable()
        {
            base.OnDisable();
            DestroyImmediate(accumTexture);
        }

        // Called by camera to apply image effect
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // Create the accumulation texture
            if (accumTexture == null || accumTexture.width != source.width || accumTexture.height != source.height)
            {
                DestroyImmediate(accumTexture);
                accumTexture = new RenderTexture(source.width, source.height, 0);
                accumTexture.hideFlags = HideFlags.HideAndDontSave;
                Graphics.Blit(source, accumTexture);
            }

            // If Extra Blur is selected, downscale the texture to 4x4 smaller resolution.
            if (extraBlur)
            {
                RenderTexture blurbuffer = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
                accumTexture.MarkRestoreExpected();
                Graphics.Blit(accumTexture, blurbuffer);
                Graphics.Blit(blurbuffer, accumTexture);
                RenderTexture.ReleaseTemporary(blurbuffer);
            }

            // Setup the texture and floating point values in the shader
            material.SetTexture("_MainTex", accumTexture);
            float fPow = (1.0f / Time.deltaTime) * blurAmount; //The higher this is, the more blur!
            float fAmount = Mathf.Exp(-fPow * 0.015f);
            //Debug.Log("Now send " + fAmount + " to blur");
            material.SetFloat("_AccumOrig", fAmount);

            // We are accumulating motion over frames without clear/discard
            // by design, so silence any performance warnings from Unity
            accumTexture.MarkRestoreExpected();

            // Render the image using the motion blur shader
            Graphics.Blit(source, accumTexture, material);
            Graphics.Blit(accumTexture, destination);
        }
    }
}