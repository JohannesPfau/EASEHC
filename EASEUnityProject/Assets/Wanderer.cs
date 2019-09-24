using System.Collections.Generic;
using UnityEngine;

public class Wanderer : MonoBehaviour
{
    private void Start() // removeme
    {
        main();
    }

    static int[] distances = new int[] { 11, 16, 5, 5, 12, 10 };
    static int days = 3;
    static int smallestDist = int.MaxValue;
    static int day1opt = 0, day2opt = 0, day3opt = 0;

    public static void main()
    {
        testRecursively(0, new List<int>(), days);

        for(int i=0; i < optimalDays.Length; i++)
            Debug.Log("Tag " + (i+1) + ":" + optimalDays[i] + " km");
        Debug.Log("Maximum:" + Mathf.Max(optimalDays) + " km");
    }

    static int[] optimalDays;
    static void testRecursively(int startIndex, List<int> previousDays, int remainingDaysCount)
    {
        if(remainingDaysCount == 1) // recursion end: last day
        {
            int lastDay = 0;
            for (int i = startIndex; i < distances.Length; i++)
                lastDay += distances[i];
            List<int> newPreviousDays = new List<int>();
            foreach (int i in previousDays)
                newPreviousDays.Add(i);
            newPreviousDays.Add(lastDay);

            //max distance:
            int maxD = Mathf.Max(newPreviousDays.ToArray());
            if (maxD < smallestDist)
            {
                smallestDist = maxD;
                optimalDays = newPreviousDays.ToArray();
            }
        }
        else
        {
            for (int end = startIndex; end < distances.Length; end++) // recursion step
            {
                int thisDay = 0;
                for (int i = startIndex; i < end; i++)
                    thisDay += distances[i];

                List<int> newPreviousDays = new List<int>();
                foreach (int i in previousDays)
                    newPreviousDays.Add(i);
                newPreviousDays.Add(thisDay);

                testRecursively(end, newPreviousDays, remainingDaysCount-1);
            }
        }
        
    }
}
