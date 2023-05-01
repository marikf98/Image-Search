namespace ImageSearch
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            int counter = 0;
            string[] arr1 = { "Dog", "Cat", "elephant", "ant", "Bird" };
            MTMergeSort mergeSort = new MTMergeSort();
            mergeSort.MergeSort(arr1);
            string[] arr1Test = { "ant", "Bird", "Cat", "Dog", "elephant" };
            if (arr1 == arr1Test)
            {
                counter++;
            }
            string[] arr2 = {"Dog", "Cat", "Dog", "Elephant", "Cat", "Elephant" , "Bird"};
            MTMergeSort mergeSort2 = new MTMergeSort();
            mergeSort.MergeSort(arr2);
            string[] arr2Test = { "Bird", "Cat", "Dog", "Dog", "Elephant", "Elephant" };
            if (arr2 == arr2Test)
            {
                counter++;
            }
            
            

        }
    }
}