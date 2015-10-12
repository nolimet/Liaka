using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
#endif
using System.Collections;

public class GroundGenerator : MonoBehaviour {

    public Sprite FloorLeft, FloorRight, FloorMiddle, Trap;
    public int FloorLength;

	void Awake()
    {
        Destroy(this);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GroundGenerator))]
public class groundGenerator_EDITOR : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GroundGenerator tar = (GroundGenerator)target;


        #region GroundGen
        if (GUILayout.Button("GenerateGround"))
        {
            List<FloorBitHolder> bits = new List<FloorBitHolder>();
            List<int> floorLayout = new List<int>(); ;

            //adding some normal floor at the beginning
            for (int i = 0; i < 3; i++)
            {
                floorLayout.Add(0);
            }

            //generator fill patern
            int last = 1;
            for (int i = 0; i < tar.FloorLength - 6; i++)
            {
                if (last != 1 && Random.Range(0, 200) > 170)
                {
                    floorLayout.Add(1);
                    last = 1;
                }
                else
                {
                    floorLayout.Add(0);
                    last = 0;
                }

            }

            //addming some normal floor at the end
            for (int i = 0; i < 3; i++)
            {
                floorLayout.Add(0);
            }

            //Spawn the accutal floor
            int newPiec = 1;

            for (int i = 0; i < floorLayout.Count; i++)
            {
                if (floorLayout[i] != 0)
                {
                    newPiec = 0; //Trap
                }
                else if (i + 1 < floorLayout.Count && i - 1 > 0)
                {
                    //Middle
                    if (floorLayout[i + 1] == 0 && floorLayout[i - 1] == 0)
                        newPiec = 1;
                    //RightPiece
                    else if ((floorLayout[i + 1] == 1 && floorLayout[i - 1] == 0))
                        newPiec = 2;
                    //LeftPiece
                    else if (floorLayout[i + 1] == 0 && floorLayout[i - 1] == 1)
                        newPiec = 3;
                }

               

                switch (newPiec)
                {
                    case 0:
                        bits.Add(makeSprite(tar.Trap, tar.transform, newPiec));
                        break;

                    case 1:
                        bits.Add(makeSprite(tar.FloorMiddle, tar.transform, newPiec));
                        break;

                    case 2:
                        bits.Add(makeSprite(tar.FloorRight, tar.transform, newPiec));
                        break;

                    case 3:
                        bits.Add(makeSprite(tar.FloorLeft, tar.transform, newPiec));
                        break;
                }
            }


            //Set them to the right position
            Vector3 posLast = Vector3.zero;
            foreach (FloorBitHolder b in bits)
            {
                b.transform.localPosition = posLast;
                posLast = b.spriteRenderer.bounds.max;

                posLast.y = 0;
                posLast.z = 0;

                b.gameObject.layer = LayerMask.NameToLayer("Ground");
                if (b.floorNumb == 0)
                {
                    b.gameObject.tag = TagManager.Trap;
                }
                else
                    b.gameObject.tag = TagManager.Ground;
            }
        }
        #endregion

        #region Clear
        if(GUILayout.Button("Destroy Children"))
        {
            for (int i = tar.transform.childCount -1; i >= 0; i--)
            {
                DestroyImmediate(tar.transform.GetChild(i).gameObject);
            }
        }
        #endregion
    }

    /// <summary>
    /// Generats a sprite object
    /// </summary>
    /// <param name="s">The sprite that will be used</param>
    /// <param name="p">Parent Object</param>
    /// <param name="number">Number it had in the generator</param>
    /// <returns>Returns a floorBitHolder. Contains all the objects it has attached to it</returns>
    FloorBitHolder makeSprite(Sprite s, Transform p, int number)
    {
        GameObject g = new GameObject(s.name);

        g.transform.SetParent(p, false);

        SpriteRenderer ren = g.AddComponent<SpriteRenderer>();
        ren.sprite = s;

        g.AddComponent<BoxCollider2D>();

        return new FloorBitHolder(ren, g.transform, g, number);

    }

    class FloorBitHolder
    {
        public SpriteRenderer spriteRenderer;
        public Transform transform;
        public GameObject gameObject;
        public int floorNumb;

        public FloorBitHolder(SpriteRenderer spriteRenderer, Transform transform, GameObject gameObject, int floorNumb)
        {
            this.spriteRenderer = spriteRenderer;
            this.transform = transform;
            this.gameObject = gameObject;
            this.floorNumb = floorNumb;
        }
    }
}

#endif
