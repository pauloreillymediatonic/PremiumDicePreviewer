using TriLibCore.Extensions;
using UnityEngine;

namespace TriLibCore.Mappers
{
    /// <summary>Represents a Mapper that looks for the Game Object which has only a Transform component and has the biggest number of children as the root bone.</summary>
    [CreateAssetMenu(menuName = "TriLib/Mappers/Root Bone/By Bones Root Bone Mapper", fileName = "ByBonesRootBoneMapper")]
    public class ByBonesRootBoneMapper : RootBoneMapper
    {
        /// <inheritdoc />
        public override Transform Map(AssetLoaderContext assetLoaderContext)
        {
            Transform bestBone = null;
            var bestChildrenCount = 0;
            foreach (var bone in assetLoaderContext.BoneTransforms)
            {
                var childrenCount = bone.CountChild();
                if (childrenCount >= bestChildrenCount)
                {
                    bestChildrenCount = childrenCount;
                    bestBone = bone;
                }
            }
            return bestBone;
        }
    }
}