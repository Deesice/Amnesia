using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(FogRenderer), PostProcessEvent.BeforeStack, "Custom/Fog")]
public sealed class Fog : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Grayscale effect intensity.")]
    public FloatParameter strength = new FloatParameter { value = 0 };
    public FloatParameter cameraClipPlane = new FloatParameter { value = 45 };
    public FloatParameter start = new FloatParameter { value = 10 };
    public FloatParameter end = new FloatParameter { value = 45 };
    public FloatParameter exp = new FloatParameter { value = 1 };
    public ColorParameter color = new ColorParameter { value = Color.white };
}

public sealed class FogRenderer : PostProcessEffectRenderer<Fog>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Custom/Fog"));
        sheet.properties.SetFloat("_CameraClipPlane", settings.cameraClipPlane);
        sheet.properties.SetFloat("_Start", settings.start);
        sheet.properties.SetFloat("_End", settings.end);
        sheet.properties.SetFloat("_Exp", settings.exp);
        sheet.properties.SetFloat("_Strength", settings.strength);
        sheet.properties.SetColor("_Color", settings.color);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}