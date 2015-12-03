using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class DropTableControler : MonoBehaviour
{
    [System.Serializable]
    public struct dropChanceStruc
    {
        public PickupBase.PickupType type;
        public PickupBase.Movement movement;
        [Tooltip("how big the chance of it dropping depending on the table size")]
        public int chance;
        public int maxConcecutiveDrops;
    }

    public struct StrulastDropped
    {
        int[] drops;
        public Dictionary<int, int> dropCount;

        public void countObject()
        {
                dropCount = new Dictionary<int, int>();

            var q = drops.GroupBy(x => x)
                .Select(g => new { Value = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count).ToList();
            
            foreach (var x in q)
            {
                dropCount.Add(x.Value, x.Count);
            }
        }

        public void AddNewDrop(int i)
        {
            countObject();
            for (int l = drops.Length - 2; l >= 0; l--)
            {
                drops[l] = drops[l + 1];
            }

            drops[drops.Length - 1] = i;
        }

        public void Init(int tableSize)
        {
            drops = new int[tableSize];
            countObject();
        }
    }

    static bool isDestoryed = false;
    StrulastDropped lastDropped;
    [SerializeField]
    int chanceTableSize;
    [SerializeField]
    dropChanceStruc[] dropChance;
    [SerializeField]
    List<int> Table;

    // Use this for initialization
    void Start()
    {
        isDestoryed = false;
        StartCoroutine(CreateDropTable());
    }

    void Destory()
    {
        isDestoryed = true;
    }

    //Uses multiThreading to create the drop table;
    IEnumerator CreateDropTable()
    {
        Thread t = new Thread(ReCalcTable);

        t.Start();
        while (t.IsAlive)
        {
            Debug.Log("Thread Running-0");
            yield return new WaitForEndOfFrame();
        }
        t.Abort();
        yield return new WaitForEndOfFrame();

//#if UNITY_EDITOR
//        t = new Thread(TestTable);
//        t.Start();
//        while (t.IsAlive)
//        {
//            Debug.Log("Thread Running-1");
//            yield return new WaitForEndOfFrame();
//        }

//        t.Abort();

//#endif
        t = null;
    }

    void ReCalcTable()
    {
        lastDropped.Init(50);
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
                Table[index] = j + 1;

                index++;
            }
        }

          System.Random rnd = new System.Random();
           Table = Table.OrderBy(x => rnd.Next()).ToList();
    }

    void TestTable()
    {
        lastDropped.countObject();
        System.DateTime timeStart = System.DateTime.Now;

        int l = 1000;
        string raw = "Raw: ";
        int[] numbs = new int[dropChance.Length+1];
        int j;
        for (int i = 0; i < l; i++)
        {
            if (isDestoryed)
                break;

            j = getRandomIntItem();
            raw += j ;
            numbs[j]++;
            Thread.Sleep(1);
        }

        string output = "Output: ";
        //foreach (int i in numbs)
         //   output += "[" + i.ToString() + " ";
        for (int i = 0; i < numbs.Length; i++)
        {
            output += "[" + i.ToString() + "]" + numbs[i].ToString() + " ";
        }

        print(raw);
        print(output);
        print("Time Test Took: " + (System.DateTime.Now - timeStart).TotalMilliseconds);
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="forceResult">forces a result that is not 0. Else it will go quick and dirty</param>
    /// <returns></returns>
    public int getRandomIntItem()
    {

        int tableSize = Table.Count;
        int i = 0;
        int itter = 0;
        bool check = true;
        bool anyMatch = false;
        // string superRaw = "SuperRaw: ";
        System.DateTime t = System.DateTime.Now;
        int SEED = t.Millisecond * t.Second * t.Minute;
        System.Random r;
        while (check)
        {
            r = new System.Random(SEED);
            //new System.Random().Next(Table.Count()
            i = Table.ElementAtOrDefault(r.Next() % tableSize);

            if (itter > 1000)
                check = false;

            //superRaw += i.ToString();

            //for (j = 0; j < lastItem.Length - 1; j++)
            //{
            //    if (!forceResult && lastItem[j] == i && i!=0 || forceResult && lastItem[j]==i)
            //        anyMatch = true;
            //}
            foreach (KeyValuePair<int, int> d in lastDropped.dropCount)
            {
                if (i == 0 && d.Key == i)
                {
                    if (d.Value > 25)
                        anyMatch = true;
                }
                else
                {
                    if (d.Key == i && d.Value > dropChance[i - 1].maxConcecutiveDrops)
                    {
                        anyMatch = true;
                    }
                }
            }

            itter++;

            if (!anyMatch)
                check = false;
            else
                anyMatch = false;
        }

        lastDropped.AddNewDrop(i);
        //print(superRaw);



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
            //i = Table.ElementAtOrDefault(new System.Random().Next() % Table.Count);
            i = getRandomIntItem();
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
