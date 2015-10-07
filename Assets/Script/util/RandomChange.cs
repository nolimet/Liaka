using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
namespace util
{
    public class RandomChange
    {
        public static bool getChance(int tableSize, int chance)
        {
            List<int> Table = new List<int>();
            for (int i = 0; i < tableSize; i++)
            {
                Table.Add(0);
            }
            for (int j = 0; j < chance; j++)
            {
                    Table[j] = 1;
            }

            int r = Table.ElementAtOrDefault(new System.Random().Next() % Table.Count());

            if (r == 1)
                return true;
            return false;
        }
    }
}
