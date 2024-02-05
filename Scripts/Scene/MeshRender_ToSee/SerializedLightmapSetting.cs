using UnityEngine;
/// <summary>
/// 将灯光贴图屯在一个物体上
/// </summary>
[ExecuteInEditMode]
public class SerializedLightmapSetting : MonoBehaviour
{
    /// <summary>
    /// 灯光贴图数组
    /// </summary>
    public Texture2D[] lightmapFar, lightmapNear;

    /// <summary>
    /// 贴图方式
    /// </summary>
    public LightmapsMode mode;

    public bool HasFog;
    public FogMode FogMode;
    public Color FogColor;
    public int StartDistance;
    public int EndDistance;

    void Start()
    {
        RenderSettings.fog = HasFog;
        RenderSettings.skybox = null;

        if (HasFog)
        {
            RenderSettings.fogMode = FogMode;
            RenderSettings.fogColor = FogColor;
            RenderSettings.fogStartDistance = StartDistance;
            RenderSettings.fogEndDistance = EndDistance;
        }

        LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;

        int light1 = (lightmapFar == null) ? 0 : lightmapFar.Length;
        int light2 = (lightmapNear == null) ? 0 : lightmapNear.Length;
        int light = (light1 < light2) ? light2 : light1;
        LightmapData[] lightmaps = null;
        if (light > 0)
        {
            lightmaps = new LightmapData[light];
            for (int i = 0; i < light; i++)
            {
                lightmaps[i] = new LightmapData();
                if (i < light1)
                    lightmaps[i].lightmapColor = lightmapFar[i];
                if (i < light2)
                    lightmaps[i].lightmapDir = lightmapNear[i];
            }
        }
        LightmapSettings.lightmaps = lightmaps;
    }
}