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
        /*public List <string> MergeSort(string[] starList, int nMin = 2)
        {
            Sort(starList);
            return null;
        }*/
        public static List<string> MergeSort(string[] starList, int nMin = 2)
        {
            Sort(starList, nMin);
            List<string>  result = starList.ToList();
            return result;
        }

        public static void Sort(string[] startList, int nMin)
        {
            
            if (startList.Length > nMin)
            {
                int mid = startList.Length / 2;

                string[] firstHalf = new string[mid];
                string[] secondHalf = new string[startList.Length - mid];
                Array.Copy(startList,0, firstHalf, 0,mid);
                Array.Copy(startList, mid, secondHalf, 0, startList.Length - mid);

                Thread threadFirstHalf = new Thread(() => Sort(firstHalf,nMin));
                Thread threadSecondHalf = new Thread(() => Sort(secondHalf,nMin));

                threadFirstHalf.Start();
                threadSecondHalf.Start();

                threadFirstHalf.Join();
                threadSecondHalf.Join();

                Merge(firstHalf, secondHalf, startList);
            }
            else
            {
                Array.Sort(startList);
            }

            /*if (startList.Length > 1)
            {
                string[] arrays = new string[]
            }*/
        }
        
        private static void Merge(string[] first, string[] second, string[] arr)
        {
            int currFirst = 0;
            int currSecond = 0;
            int currArr = 0;

            while(currFirst < first.Length && currSecond < second.Length)
            {
                if (string.Compare(first[currFirst], second[currSecond]) == -1) // second is bigger
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
                
                
                
                    /*if (string.Compare(first[currFirst], second[currSecond]) == 1) // first is bigger
                    {
                        arr[currArr] = second[currSecond];
                        currSecond++;
                        currArr++;
                    }
                    
                    else
                    {
                        arr[currArr] = first[currFirst];
                        currFirst++;
                        currArr++;
                        arr[currArr] = second[currSecond];
                        currSecond++;
                        currArr++;
                    }*/
            }

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





    /*class MyTask
    {
        private string[] starList;
        
        public MyTask(string[] arr)
        {
            this.starList = arr;
        }

        public void Compute()
        {
            if (starList.Length > 1)
            {
                int mid = starList.Length / 2;

                string[] firstHalf = new string[mid];
                string[] secondHalf = new string[starList.Length - mid];
                Array.Copy(starList,firstHalf,mid);
                Array.Copy(starList, mid, secondHalf, 0, starList.Length - mid);

                Task firstTask = Task.Factory.StartNew(() => new MyTask(firstHalf).Compute());
                Task secondTask = Task.Factory.StartNew(() => new MyTask(secondHalf).Compute());
                Task.WaitAll(firstTask, secondTask);

                MTMergeSort.Merge(firstHalf, secondHalf,starList);

            }
        }
    }*/
    



}