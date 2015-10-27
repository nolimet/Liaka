using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DropTableControler : MonoBehaviour
{
    [System.Serializable]
    public struct dropChanceStruc
    {
        public PickupBase.PickupType type;
        public PickupBase.Movement movement;
        [Tooltip("chance in a thousand it will drop")]
        public int chance;
    }
    [SerializeField]
    int chanceTableSize;
    [SerializeField]
    dropChanceStruc[] dropChance;
    [SerializeField]
    List<int> Table;

    // Use this for initialization
    void Awake()
    {
        Table = new List<int>();
        for (int i = 0; i < chanceTableSize; i++)
        {
            Table.Add(0);
        }
        
        int index = 0;
        for (int j = 0; j < dropChance.Length; j++)
        {
            for (int i = 0; i < dropChance[j].chance; i++)
            {
                Table[index] = j+1;
                
                index++;
            }
        }

        System.Random rnd = new System.Random();

        Table = Table.OrderBy(x => rnd.Next()).ToList();
#if UNITY_EDITOR
        TestTable();
#endif 
    }
    
    void TestTable()
    {
        int l = 100000;
        int[] numbs = new int[dropChance.Length+1];
        int j;
        for (int i = 0; i < l; i++)
        {
            j = Table.ElementAtOrDefault(new System.Random().Next() % Table.Count());
            numbs[j]++;
        }

        string output = "";
        foreach (int i in numbs)
            output += i.ToString() + " ";

        print(output);
    }

    public PickupBase getRandomItem()
    {
        int i = Table.ElementAtOrDefault(new System.Random().Next() % Table.Count());
        i--;

        if (i < 0)
            return null;

        dropChanceStruc d = dropChance[i];
        PickupBase p = PickupPool.GetObject(d.type);
        p.moveType = d.movement;

        return p;
    }
}
