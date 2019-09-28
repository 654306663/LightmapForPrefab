using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BakePrefabLightMaps
{
    [MenuItem("Assets/Bake Prefab Lightmaps")]
    static void GenerateLightmapInfo()
    {
        if (Lightmapping.giWorkflowMode != Lightmapping.GIWorkflowMode.OnDemand)
        {
            Debug.LogError("ExtractLightmapData requires that you have baked you lightmaps and Auto mode is disabled.");
            return;
        }
        Lightmapping.Bake();

        PrefabLightmapData[] prefabs = GameObject.FindObjectsOfType<PrefabLightmapData>();

        foreach (var instance in prefabs)
        {
            var gameObject = instance.gameObject;
            var rendererInfos = new List<PrefabLightmapData.RendererInfo>();
            var lightmaps = new List<Texture2D>();

            GenerateLightmapInfo(gameObject, rendererInfos, lightmaps);

            instance.m_RendererInfo = rendererInfos.ToArray();
            instance.m_Lightmaps = lightmaps.ToArray();

            //var targetPrefab = UnityEditor.PrefabUtility.GetPrefabParent(gameObject) as GameObject;
            var targetPrefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject) as GameObject;
            if (targetPrefab != null)
            {
                //UnityEditor.Prefab
                PrefabUtility.ReplacePrefab(gameObject, targetPrefab);
            }
        }
    }

    static void GenerateLightmapInfo(GameObject root, List<PrefabLightmapData.RendererInfo> rendererInfos, List<Texture2D> lightmaps)
    {
        var renderers = root.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.lightmapIndex != -1)
            {
                PrefabLightmapData.RendererInfo info = new PrefabLightmapData.RendererInfo();
                info.renderer = renderer;
                info.lightmapOffsetScale = renderer.lightmapScaleOffset;

                Texture2D lightmap = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;

                info.lightmapIndex = lightmaps.IndexOf(lightmap);
                if (info.lightmapIndex == -1)
                {
                    info.lightmapIndex = lightmaps.Count;
                    lightmaps.Add(lightmap);
                }

                rendererInfos.Add(info);
            }
        }
    }
}
