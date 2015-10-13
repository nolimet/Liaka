using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectPlacer : MonoBehaviour {
    public int numberOfObjects;
    public Vector3 Interval;

    public GameObject copyObject;

    void Awake()
    {
        Destroy(this);
    }
	
}

#if UNITY_EDITOR
[CustomEditor(typeof(ObjectPlacer))]
public class ObjectPlacer_EDITOR : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ObjectPlacer tar = (ObjectPlacer)target;
        if (GUILayout.Button("PlaceObjects"))
        {
            int l = tar.numberOfObjects;
            Vector3 currentPos = Vector3.zero;

            for (int i = 0; i < l; i++)
            {
                if (!tar.copyObject)
                    MakeObject(tar.transform, currentPos);
                else
                    MakeObject(tar.transform, currentPos, tar.copyObject);
                currentPos += tar.Interval;
            }
        }

#region Clear
        if (GUILayout.Button("Destroy Children"))
        {
            for (int i = tar.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(tar.transform.GetChild(i).gameObject);
            }
        }
#endregion
    }

    Transform MakeObject(Transform parent, Vector3 localPos)
    {
        GameObject g = new GameObject();

        g.transform.SetParent(parent, false);
        g.transform.localPosition = localPos;

        return g.transform;
    }

    Transform MakeObject(Transform parent, Vector3 localPos, GameObject original)
    {
        GameObject g = Instantiate(original) as GameObject;

        g.transform.SetParent(parent, false);
        g.transform.localPosition = localPos;

        return g.transform;
    }
}
#endif