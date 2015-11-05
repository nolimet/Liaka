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
        //TestTable();
#endif 
    }
    
    void TestTable()
    {
        int l = 10;
        string raw = "";
        int[] numbs = new int[dropChance.Length+1];
        int j;
        for (int i = 0; i < l; i++)
        {
            j = getRandomIntItem();
            raw += j;
            numbs[j]++;
        }

        string output = "";
        //foreach (int i in numbs)
         //   output += "[" + i.ToString() + " ";
        for (int i = 0; i < numbs.Length; i++)
        {
            output += "[" + i.ToString() + "]" + numbs[i].ToString() + " ";
        }

        print(output);
        //print(raw);
    }
    int[] lastItem = new int[10];

    public int getRandomIntItem()
    {
        int i = 0;
        int j;
        int itter = 0;
        bool check = true;
        bool anyMatch = false;
        while (check)
        {
            i = Table.ElementAtOrDefault(new System.Random().Next() % Table.Count());


            for (j = 0; j < lastItem.Length - 1; j++)
            {
                if (lastItem[j] == i || i==0)
                    anyMatch = true;
            }

            itter++;

            if (!anyMatch)
                check = false;
            else
                anyMatch = false;
        }

        for (int l = lastItem.Length - 2; l >= 0; l--)
        {
            lastItem[l] = lastItem[l + 1];
        }

        lastItem[lastItem.Length - 1] = i;

        //debuging stuff
        //string output = "";
        //for (int l = 0; l < lastItem.Length - 1; l++)
        //{
        //    output += lastItem[l].ToString() + ", ";
        //}

        //output += lastItem[lastItem.Length - 1].ToString();
        //output += " Total Itterations needed : " + itter.ToString();
        //print(output);
        //end debugging stuff

        return i;
    }


    public PickupBase getRandomItem()
    {
        
        int i = 0;
 //       int j;
 //       bool check = true;
//        bool anyMatch = false;
//        while (check)
 //       {
            i = Table.ElementAtOrDefault(new System.Random().Next() % Table.Count());
            
/*
            for (j = 0; j < lastItem.Length-1; j++)
            {
                if (lastItem[j] == i)
                    anyMatch=true;
            }

            if (!anyMatch)
                check = false;
            else
                anyMatch = false;
        }
        
        for (int l = lastItem.Length - 2; l >0; l--)
        {
            lastItem[l] = lastItem[l + 1];
        }

        lastItem[lastItem.Length - 1] = i;
        */
        i--;
        if (i < 0)
            return null;

        dropChanceStruc d = dropChance[i];
        PickupBase p = PickupPool.GetObject(d.type);
        p.moveType = d.movement;

        return p;
    }
}
