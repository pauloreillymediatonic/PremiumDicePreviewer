using System;
using TriLibCore.General;
using TriLibCore.Utils;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace TriLibCore.Mappers
{
    /// <summary>Represents a Material Mapper that converts TriLib Materials into Unity UniversalRP Materials.</summary>
    [Serializable]
    [CreateAssetMenu(menuName = "TriLib/Mappers/Material/Universal RP Material Mapper", fileName = "UniversalRPMaterialMapper")]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class UniversalRPMaterialMapper : MaterialMapper
    {
        static UniversalRPMaterialMapper()
        {
            AddToRegisteredMappers();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void AddToRegisteredMappers()
        {
            if (RegisteredMappers.Contains("UniversalRPMaterialMapper"))
            {
                return;
            }
            RegisteredMappers.Add("UniversalRPMaterialMapper");
        }

        public override Material MaterialPreset => Resources.Load<Material>("Materials/UniversalRP/TriLibUniversalRP");

        public override Material AlphaMaterialPreset => Resources.Load<Material>("Materials/UniversalRP/TriLibUniversalRPAlphaCutout");

        public override Material AlphaMaterialPreset2 => Resources.Load<Material>("Materials/UniversalRP/TriLibUniversalRPAlpha");

        public override Material SpecularMaterialPreset => Resources.Load<Material>("Materials/UniversalRP/TriLibUniversalRPSpecular");

        public override Material SpecularAlphaMaterialPreset => Resources.Load<Material>("Materials/UniversalRP/TriLibUniversalRPAlphaCutoutSpecular");

        public override Material SpecularAlphaMaterialPreset2 => Resources.Load<Material>("Materials/UniversalRP/TriLibUniversalRPAlphaSpecular");

        public override Material LoadingMaterial => Resources.Load<Material>("Materials/UniversalRP/TriLibUniversalRPLoading");

        ///<inheritdoc />
        public override bool IsCompatible(MaterialMapperContext materialMapperContext)
        {
            return GraphicsSettingsUtils.IsUsingUniversalPipeline;
        }

        ///<inheritdoc />
        public override void Map(MaterialMapperContext materialMapperContext)
        {
            materialMapperContext.VirtualMaterial = new VirtualMaterial();
            CheckDiffuseMapTexture(materialMapperContext);
        }

        private void CheckDiffuseMapTexture(MaterialMapperContext materialMapperContext)
        {
            var diffuseTexturePropertyName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.DiffuseTexture);
            if (materialMapperContext.Material.HasProperty(diffuseTexturePropertyName))
            {
                LoadTexture(materialMapperContext, TextureType.Diffuse, materialMapperContext.Material.GetTextureValue(diffuseTexturePropertyName), ApplyDiffuseMapTexture);
            }
            else
            {
                ApplyDiffuseMapTexture(materialMapperContext, TextureType.Diffuse, null);
            }
        }

        private void ApplyDiffuseMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            materialMapperContext.VirtualMaterial.SetProperty("_BaseMap", texture);
            CheckGlossinessValue(materialMapperContext);
        }

        private void CheckGlossinessValue(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericPropertyValueMultiplied(GenericMaterialProperty.Glossiness, materialMapperContext.Material.GetGenericFloatValue(GenericMaterialProperty.Glossiness));
            materialMapperContext.VirtualMaterial.SetProperty("_Glossiness", value);
            CheckMetallicValue(materialMapperContext);
        }

        private void CheckMetallicValue(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericFloatValue(GenericMaterialProperty.Metallic);
            materialMapperContext.VirtualMaterial.SetProperty("_Metallic", value);
            CheckEmissionMapTexture(materialMapperContext);
        }

        private void CheckEmissionMapTexture(MaterialMapperContext materialMapperContext)
        {
            var emissionTexturePropertyName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.EmissionTexture);
            if (materialMapperContext.Material.HasProperty(emissionTexturePropertyName))
            {
                LoadTexture(materialMapperContext, TextureType.Emission, materialMapperContext.Material.GetTextureValue(emissionTexturePropertyName), ApplyEmissionMapTexture);
            }
            else
            {
                ApplyEmissionMapTexture(materialMapperContext, TextureType.Emission, null);
            }
        }

        private void ApplyEmissionMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            materialMapperContext.VirtualMaterial.SetProperty("_EmissionMap", texture);
            if (texture)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_EMISSION");
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_EMISSION");
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
            CheckNormalMapTexture(materialMapperContext);
        }

        private void CheckNormalMapTexture(MaterialMapperContext materialMapperContext)
        {
            var normalMapTexturePropertyName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.NormalTexture);
            if (materialMapperContext.Material.HasProperty(normalMapTexturePropertyName))
            {
                LoadTexture(materialMapperContext, TextureType.NormalMap, materialMapperContext.Material.GetTextureValue(normalMapTexturePropertyName), ApplyNormalMapTexture);
            }
            else
            {
                ApplyNormalMapTexture(materialMapperContext, TextureType.NormalMap, null);
            }
        }

        private void ApplyNormalMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            materialMapperContext.VirtualMaterial.SetProperty("_BumpMap", texture);
            if (texture != null)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_NORMALMAP");
                materialMapperContext.VirtualMaterial.SetProperty("_NormalScale", materialMapperContext.Material.GetGenericPropertyValueMultiplied(GenericMaterialProperty.NormalTexture, 1f));
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_NORMALMAP");
            }
            CheckSpecularTexture(materialMapperContext);
        }

        private void CheckSpecularTexture(MaterialMapperContext materialMapperContext)
        {
            var specularTexturePropertyName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.SpecularTexture);
            if (materialMapperContext.Material.HasProperty(specularTexturePropertyName))
            {
                LoadTexture(materialMapperContext, TextureType.Specular, materialMapperContext.Material.GetTextureValue(specularTexturePropertyName), ApplySpecGlossMapTexture);
            }
            else
            {
                ApplySpecGlossMapTexture(materialMapperContext, TextureType.Specular, null);
            }
        }

        private void ApplySpecGlossMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            materialMapperContext.VirtualMaterial.SetProperty("_SpecGlossMap", texture);
            if (texture != null)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_METALLICSPECGLOSSMAP");
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_METALLICSPECGLOSSMAP");
            }
            CheckOcclusionMapTexture(materialMapperContext);
        }

        private void CheckOcclusionMapTexture(MaterialMapperContext materialMapperContext)
        {
            var occlusionMapTextureName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.OcclusionTexture);
            if (materialMapperContext.Material.HasProperty(occlusionMapTextureName))
            {
                LoadTexture(materialMapperContext, TextureType.Occlusion, materialMapperContext.Material.GetTextureValue(occlusionMapTextureName), ApplyOcclusionMapTexture);
            }
            else
            {
                ApplyOcclusionMapTexture(materialMapperContext, TextureType.Occlusion, null);
            }
        }

        private void ApplyOcclusionMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            materialMapperContext.VirtualMaterial.SetProperty("_OcclusionMap", texture);
            if (texture != null)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_OCCLUSIONMAP");
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_OCCLUSIONMAP");
            }
            CheckParallaxMapTexture(materialMapperContext);
        }

        private void CheckParallaxMapTexture(MaterialMapperContext materialMapperContext)
        {
            var parallaxMapTextureName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.ParallaxMap);
            if (materialMapperContext.Material.HasProperty(parallaxMapTextureName))
            {
                LoadTexture(materialMapperContext, TextureType.Parallax, materialMapperContext.Material.GetTextureValue(parallaxMapTextureName), ApplyParallaxMapTexture);
            }
            else
            {
                ApplyParallaxMapTexture(materialMapperContext, TextureType.Parallax, null);
            }
        }

        private void ApplyParallaxMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            materialMapperContext.VirtualMaterial.SetProperty("_ParallaxMap", texture);
            if (texture)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_PARALLAXMAP");
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_PARALLAXMAP");
            }
            CheckMetallicGlossMapTexture(materialMapperContext);
        }

        private void CheckMetallicGlossMapTexture(MaterialMapperContext materialMapperContext)
        {
            var metallicGlossMapTextureName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.MetallicGlossMap);
            if (materialMapperContext.Material.HasProperty(metallicGlossMapTextureName))
            {
                LoadTexture(materialMapperContext, TextureType.Metalness, materialMapperContext.Material.GetTextureValue(metallicGlossMapTextureName), ApplyMetallicGlossMapTexture);
            }
            else
            {
                ApplyMetallicGlossMapTexture(materialMapperContext, TextureType.Metalness, null);
            }
        }

        private void ApplyMetallicGlossMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            materialMapperContext.VirtualMaterial.SetProperty("_MetallicGlossMap", texture);
            if (texture != null)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_METALLICGLOSSMAP");
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_METALLICGLOSSMAP");
            }
            CheckEmissionColor(materialMapperContext);
        }

        private void CheckEmissionColor(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericColorValue(GenericMaterialProperty.EmissionColor) * materialMapperContext.Material.GetGenericPropertyValueMultiplied(GenericMaterialProperty.EmissionColor, 1f);
            materialMapperContext.VirtualMaterial.SetProperty("_EmissionColor", value);
            if (value != Color.black)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_EMISSION");
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_EMISSION");
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
            CheckDiffuseColor(materialMapperContext);
        }

        private void CheckDiffuseColor(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericColorValue(GenericMaterialProperty.DiffuseColor) * materialMapperContext.Material.GetGenericPropertyValueMultiplied(GenericMaterialProperty.DiffuseColor, 1f);
            value.a *= materialMapperContext.Material.GetGenericFloatValue(GenericMaterialProperty.AlphaValue);
            if (!materialMapperContext.VirtualMaterial.HasAlpha && value.a < 1f)
            {
                materialMapperContext.VirtualMaterial.HasAlpha = true;
            }
            materialMapperContext.VirtualMaterial.SetProperty("_BaseColor", value);
            BuildMaterial(materialMapperContext);
        }
    }
}