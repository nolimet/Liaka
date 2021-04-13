using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class EditorUtil
{
    /// <summary>
    /// Moves existing animation clips to be part of animator controller.
    /// It does this by creating a copy and then adding a copy. It has option to delete the old copy.
    /// </summary>
    /// <param name="animationClip"></param>
    public static void CopyExistingClip(this AnimatorController animatorController, AnimationClip animationClip)
    {
        var newCopy = Object.Instantiate(animationClip) as AnimationClip;
        newCopy.name = animationClip.name;
        animatorController.AddClipToAnimatorController(newCopy);
    }

    /// <summary>
    /// Creates a new clip and adds it to the animator controller.
    /// </summary>
    /// <param name="clipName"></param>
    public static void CreateNewAnimationClip(this AnimatorController animatorController, string clipName)
    {
        var newClip = new AnimationClip { name = clipName };
        AddClipToAnimatorController(animatorController, newClip);
    }

    /// <summary>
    /// Add the clip to animator controller and re-imports
    /// </summary>
    /// <param name="animationClip"></param>
    public static void AddClipToAnimatorController(this AnimatorController animatorController, AnimationClip animationClip)
    {
        AssetDatabase.AddObjectToAsset(animationClip, animatorController);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animationClip));
    }
}