using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace ImageSearch
{
    public static class MTMergeSort
    {
        /**
         * This is the main function of the merge sort, this function calls the recursive merge sort.
         * After the merge sort is done the output is then converted to a list which then returned to the user.*
         */
        public static List<string> MergeSort(string[] starList, int nMin = 2)
        {
            Sort(starList, nMin);
            List<string>  result = starList.ToList();
            return result;
        }

        /**
         *This is the recursive sort function that splits the array into two sub arrays and then merges it*
         */
        private static void Sort(string[] startList, int nMin)
        {
            if (startList.Length > nMin) // this is the the minimal threshold for crating a new tread for the sort, over-wise it just sorts the array manually 
            {
                //We split the array into two halfs 
                int mid = startList.Length / 2;

                string[] firstHalf = new string[mid];
                string[] secondHalf = new string[startList.Length - mid];
                Array.Copy(startList,0, firstHalf, 0,mid);
                Array.Copy(startList, mid, secondHalf, 0, startList.Length - mid);
                // then we create two threads that each will run the sort function with half of the original array
                Thread threadFirstHalf = new Thread(() => Sort(firstHalf,nMin));
                Thread threadSecondHalf = new Thread(() => Sort(secondHalf,nMin));
                // we launch the threads
                threadFirstHalf.Start();
                threadSecondHalf.Start();
                // we wait for the threads to finish
                threadFirstHalf.Join();
                threadSecondHalf.Join();
                // then we merge the results 
                Merge(firstHalf, secondHalf, startList);
            }
            
            else // in case the array size is smaller then the nMin
            {
                Array.Sort(startList);
            }
        }
        /**
         *This is the merge function it receives three arrays - two half arrays that are sorted and then merges it to the third array*
         */
        private static void Merge(string[] first, string[] second, string[] arr)
        {
            int currFirst = 0;
            int currSecond = 0;
            int currArr = 0;

            while(currFirst < first.Length && currSecond < second.Length) 
            { 
                /*
                 * This part merges the sub arrays into the big array*
                 */
                if (string.Compare(first[currFirst], second[currSecond]) == -1) // compares the strings
                    { 
                        arr[currArr] = first[currFirst];
                        currFirst++;
                        currArr++;
                    }
                    else
                    {
                        arr[currArr] = second[currSecond];
                        currSecond++;
                        currArr++;
                    }
            }
            /*
             * fills the remaining elements to the big array*
             */
            while (currFirst < first.Length)
            {
                arr[currArr] = first[currFirst];
                currFirst++;
                currArr++;
            }
            while (currSecond < second.Length)
            {
                arr[currArr] = second[currSecond];
                currSecond++;
                currArr++;
            }
        }
    }
}