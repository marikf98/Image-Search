using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageSearch
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            /*int counter = 0;
            string[] arr1 = { "Dog", "Cat", "elephant", "ant", "Bird" };
            List<string>  result1 = MTMergeSort.MergeSort(arr1);
            string[] arr1Test = { "ant", "Bird", "Cat", "Dog", "elephant" };
            if (arr1 == arr1Test)
            {
                counter++;
            }
            string[] arr2 = {"Dog", "Cat", "Dog", "Elephant", "Elephant" , "Bird"};
            List<string>  result2 = MTMergeSort.MergeSort(arr2);
            string[] arr2Test = { "Bird", "Cat", "Dog", "Dog", "Elephant", "Elephant" };
            if (arr2 == arr2Test)
            {
                counter++;
            }*/
            
            /*string[] emptyArray = new string[0];
            List<string>  result1 = MTMergeSort.MergeSort(emptyArray);
            Console.WriteLine(emptyArray.SequenceEqual(new string[0]));
            
            
            string[] singleElementArray = new string[] { "apple" };
            List<string>  result2 = MTMergeSort.MergeSort(singleElementArray);
            Console.WriteLine(singleElementArray.SequenceEqual(new string[] { "apple" })); // Output: True
            
            string[] duplicateElementArray = new string[] { "apple", "banana", "banana", "cherry", "apple" };
            List<string>  result3 = MTMergeSort.MergeSort(duplicateElementArray);
            Console.WriteLine(duplicateElementArray.SequenceEqual(new string[] { "apple", "banana", "cherry" })); // Output: True
            
            
            string[] reverseOrderArray = new string[] { "pear", "orange", "mango", "kiwi", "apple" };
            Array.Reverse(reverseOrderArray);
            List<string>  result4 = MTMergeSort.MergeSort(duplicateElementArray,4);
            Console.WriteLine(reverseOrderArray.SequenceEqual(new string[] { "apple", "kiwi", "mango", "orange", "pear" })); // Output: True
            
            string[] randomElementArray = new string[] { "pear", "apple", "banana", "orange", "kiwi", "cherry", "mango" };
            List<string>  result5 = MTMergeSort.MergeSort(randomElementArray);
            Console.WriteLine(randomElementArray.SequenceEqual(new string[] { "apple", "banana", "cherry", "kiwi", "mango", "orange", "pear" })); // Output: True
            
            string[] nullElementArray = new string[] { "pear", null, "banana", "orange", "kiwi", "cherry", "mango" };
            List<string>  result6 = MTMergeSort.MergeSort(nullElementArray);
            Console.WriteLine(nullElementArray.SequenceEqual(new string[] { null, "banana", "cherry", "kiwi", "mango", "orange", "pear" })); // Output: True
            
            string[] mixedCaseArray = new string[] { "pear", "Apple", "banana", "OrAnGe", "KiWi", "cherry", "mango" };
            List<string>  result7 = MTMergeSort.MergeSort(mixedCaseArray,3);
            Console.WriteLine(mixedCaseArray.SequenceEqual(new string[] { "Apple", "KiWi", "banana", "cherry", "mango", "orange", "pear" })); // Output: True
            
            string[] nonAlphabeticArray = new string[] { "pear", "apple", "123", "@#%$", "kiwi", "cherry", "mango" };
            List<string>  result8 = MTMergeSort.MergeSort(nonAlphabeticArray);
            Console.WriteLine(nonAlphabeticArray.SequenceEqual(new string[] { "@#%$", "123", "apple", "cherry", "kiwi", "mango", "pear" })); // Output: True*/
            
            ImageSearch.FirstSearchOption(args);
        }
    }
}