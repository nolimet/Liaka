using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
#endif
using System.Collections;

public class ChildAliner_SPRITE : MonoBehaviour {

    void Awake()
    {
        Destroy(this);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(ChildAliner_SPRITE))]
public class ChildALiner_SPRITE_EDITOR : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ChildAliner_SPRITE tar = (ChildAliner_SPRITE)target;
        #region GroundGen
        if (GUILayout.Button("Positions"))
        {
            List<SpriteTransform> bits = new List<SpriteTransform>();
            int l = tar.transform.childCount;
            Transform t;

            for (int i = 0; i < l; i++)
            {
                t = tar.transform.GetChild(i);

                bits.Add(new SpriteTransform(t.gameObject.GetComponent<SpriteRenderer>(), t));
            }
            


            //Set them to the right position
            Vector3 posLast = Vector3.zero;
            foreach (SpriteTransform b in bits)
            {
                b.transform.localPosition = posLast;
                posLast = b.spriteRenderer.bounds.max;
                Debug.Log(b.spriteRenderer.bounds.max.x);

                posLast.y = 0;
                posLast.z = 0;
            }
        }
        #endregion
    }


    class SpriteTransform
    {
        public SpriteRenderer spriteRenderer;
        public Transform transform;
        
        public SpriteTransform (SpriteRenderer spriteRenderer, Transform transform)
        {
            this.spriteRenderer = spriteRenderer;
            this.transform = transform;
        }
    }
}
#endif