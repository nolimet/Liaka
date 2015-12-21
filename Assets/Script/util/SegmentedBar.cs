using UnityEngine;
using System.Collections;
namespace util
{
    public class SegmentedBar : MonoBehaviour
    {

        public float maxValue = 1;

        public float Value
        {
            get { return _Value; }
            set
            {
                if (value != _Value)
                {
                    _Value = value;
                    if (_Value > maxValue)
                        _Value = maxValue;
                    Update_Value();
                }
            }
        }
        float _Value;
        public int MaxSegments = 7;

        public GameObject SegmentPrefab;
        public Transform Container;

        public GameObject[] Segments;

        void Awake()
        {

            if (Segments == null || Segments.Length == 0)
            {
                float w = ((RectTransform)SegmentPrefab.transform).rect.width;
                GameObject g;
                RectTransform r;
                Segments = new GameObject[MaxSegments];
                for (int i = 0; i < MaxSegments; i++)
                {
                    g = Instantiate(SegmentPrefab);
                    r = (RectTransform)g.transform;
                    g.name = "Segment - " + i;
                    Segments[i] = g;

                    g.transform.SetParent(Container, false);
                    r.anchoredPosition = new Vector2(w * i, 0);
                    g.SetActive(false);

                }
            }
        }

        void Update_Value()
        {
            int z = Mathf.CeilToInt((_Value / maxValue) * MaxSegments);
            for (int i = 0; i < MaxSegments; i++)
            {
                if (Segments[i])
                {
                    if (i <= z)
                        Segments[i].SetActive(true);
                    else if (i > z)
                        Segments[i].SetActive(false);
                }
            }
        }
    }
}