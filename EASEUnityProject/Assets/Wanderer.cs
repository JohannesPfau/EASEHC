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
        // method 2: recursive
        testRecursively(0, new List<int>(), days);

        for(int i=0; i < optimalDays.Length; i++)
            Debug.Log("Tag " + (i+1) + ":" + optimalDays[i] + " km");
        Debug.Log("Maximum:" + Mathf.Max(optimalDays) + " km");

        return;

        //brute force: day 1
        for (int start1 = 0, end1 = 0; end1 < distances.Length; end1++)
        {
            int day1 = 0;
            for (int i = start1; i < end1; i++)
                day1 += distances[i];

            for (int start2 = end1, end2 = end1; end2 < distances.Length; end2++)
            {
                int day2 = 0;
                for (int i = start2; i < end2; i++)
                    day2 += distances[i];

                // last day:
                int start3 = end2;
                int day3 = 0;
                for (int i = start3; i < distances.Length; i++)
                    day3 += distances[i];

                //max distance:
                int maxD = Mathf.Max(day1, day2, day3);
                if(maxD < smallestDist)
                {
                    smallestDist = maxD;
                    day1opt = day1;
                    day2opt = day2;
                    day3opt = day3;
                }
            }
        }
        Debug.Log("Smallest dist: (" + day1opt + "," + day2opt + "," + day3opt + ")");

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
                //Debug.Log("MINIMAL:");
                //string s = "";
                //foreach (int i in optimalDays)
                //    s += i + ",";
                //Debug.Log("Days: " + s);
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
