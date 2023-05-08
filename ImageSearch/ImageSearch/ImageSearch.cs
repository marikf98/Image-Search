using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Channels;
using System.Threading;

namespace ImageSearch
{
    /*
     *This is the image search class that can search for a small picture in a big picture
     * The algorithm splits the big pictures into tiles that are a bit bigger then the small picture and the tiles overlap each over
     * so the image wont be missed on the stitch
     */
    public class ImageSearch
    {
        /*
         *This is the main function of the class  
         */
        public static void SearchImage(string[] args)
        {
            if (args.Length < 3) // input check for the arguments 
            {
                Console.WriteLine("Not enough arguments ");
                Environment.Exit(1);
            }
            
            string image1Input = args[0]; // the bigger picture
            string image2Input = args[1]; // the smaller picture
            int nThreadsInput = int.Parse(args[2]);
            string algorithmInput = args[3];
            // input check for the file existence 
            if (!File.Exists(image2Input))
            {
                Console.WriteLine("Image file doesnt exists.");
                Environment.Exit(1);

            }
            if (!File.Exists(image1Input))
            {
                Console.WriteLine("Image file doesnt doesnt exists.");
                Environment.Exit(1);

            }
            
            //We convert the the string input of the image source to an image object 
            Image image1 = null;
            Image image2 = null;
            // here we check their validity 
            try
            {
                image1 = Image.FromFile(image1Input);
                image2 = Image.FromFile(image2Input);
                Bitmap bitMapImage2 = new Bitmap(image2Input);
            }
            catch (Exception e)
            {
                Console.WriteLine("The image files are not valid");
                Environment.Exit(1);
            }

            // Here we calculate the size of each tile 
            int smallWidth = image2.Width;
            int smallHeight = image2.Height;
            int overlapWidth = smallWidth / 2;
            int overlapHeight = smallHeight / 2;
            
            // and here we calculate how many tiles will be on each axis 
            int numTilesX = (int)Math.Ceiling((double)(image1.Width - overlapWidth) / (smallWidth - overlapWidth));
            int numTilesY = (int)Math.Ceiling((double)(image1.Height - overlapHeight) / (smallHeight - overlapHeight));
            
            //Here we create a matrix of bitmaps and points according to the tiles that we calculated before
            Bitmap[,] tiles = new Bitmap[numTilesX,numTilesY];
            Point[,] pointsOffset = new Point[numTilesX, numTilesY];

            for (int i = 0; i < numTilesX; i++)
            {
                for (int j = 0; j < numTilesY; j++)
                {
                    // here we calculate the start coordinates of each tile
                    int startX = i * (smallWidth - overlapWidth);
                    int startY = j * (smallHeight - overlapHeight);
                    int searchWidth = smallWidth + overlapWidth;
                    int searchHeight = smallHeight + overlapHeight;
                    // here we crate the bitmap and the point
                    Bitmap tile = new Bitmap(searchWidth, searchHeight);

                    using (Graphics g = Graphics.FromImage(tile))
                    {
                        g.DrawImage(image1, new Rectangle(0, 0, searchWidth, searchHeight), new Rectangle(startX, startY, searchWidth, searchHeight), GraphicsUnit.Pixel);
                    }

                    pointsOffset[i, j] = new Point(startX, startY);
                    tiles[i, j] = tile;
                }
            }
            // after we created the matrix we convert them to single dimensional array for ease wit the thread splitting
            Bitmap[] oneDtiles = tiles.Cast<Bitmap>().ToArray();
            Point[] point1D = pointsOffset.Cast<Point>().ToArray();
            // here we create a helper array of the smaller image as a bit map so each thread can read from his own bitmap and there wont be collisions 
            Bitmap[] compareBitmap = new Bitmap[nThreadsInput];
            
            for (int i = 0; i < nThreadsInput; i++)
            {
                compareBitmap[i] = new Bitmap(image2Input);
            }
            
            int tilesPerThread = (int)Math.Ceiling((double)(numTilesX * numTilesY) / nThreadsInput); // the number of tiles each tread will scan at most 

            List<Point?[]> resultFromExact = new List<Point?[]>();
            List<Thread> threads = new List<Thread>();
            // we created a 2d array so each thread will scan its own array of point and bitmap 
            Bitmap[][] bitmapSubArrays = new Bitmap[nThreadsInput][];
            Point[][] pointSunArrays = new Point[nThreadsInput][];
            
            int subArraySize = (int)Math.Ceiling((double)oneDtiles.Length / nThreadsInput);
            
            // and here we fill each sub array with the tiles that each thread will scan
            for (int i = 0; i < nThreadsInput; i++)
            {
                int startIndex = i * subArraySize;
                if (startIndex >= oneDtiles.Length)
                {
                    continue;
                }
                int endIndex = Math.Min(startIndex + subArraySize, oneDtiles.Length);
                int subArrayLength = endIndex - startIndex;
                if (subArrayLength < 0)
                {
                    subArrayLength = subArrayLength * (-1);
                }
                bitmapSubArrays[i] = new Bitmap[subArrayLength];
                pointSunArrays[i] = new Point[subArrayLength];
                Array.Copy(oneDtiles, startIndex, bitmapSubArrays[i], 0, subArrayLength);
                Array.Copy(point1D, startIndex, pointSunArrays[i], 0, subArrayLength);
            }

            /*
             * here we create the threads for the algorithm that the user wants
             * we add them to the thread array 
             */
            switch (algorithmInput)
            {
                case "exact":
                {
                    for (int i = 0; i < nThreadsInput; ++i)
                    {
                        int index = i;
                        Thread thread = new Thread(() =>
                            {
                                resultFromExact.Add(FindBitMapExact(bitmapSubArrays[index], compareBitmap[index], pointSunArrays[index]));
                            }
                        );
                        threads.Add(thread);
                    }

                    break;
                }
                case "euclidian":
                {
                    for (int i = 0; i < nThreadsInput; ++i)
                    {
                        int index = i;
                        Thread thread = new Thread(() =>
                            {
                                resultFromExact.Add(FindBitMapEuclidian(bitmapSubArrays[index], compareBitmap[index], pointSunArrays[index]));
                            }
                        );
                        threads.Add(thread);
                    }

                    break;
                }
                //if the algorithm input is invalid we print an error message and exit 
                default:
                    Console.WriteLine("Please enter exact or euclidian");
                    Environment.Exit(1);
                    break;
            }

            //then we launch the treads 
            foreach (Thread thread in threads)
            {
                 thread.Start();
            }
            //and wait for the threads to finish
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            /*
             * here we collect all the results of each thread and create the final result array
             * we scan each sub array and check if we got any point of the start location of the small photo
             */
            List<Point?> resultsExact = new List<Point?>();
            foreach (Point?[] pointArr in resultFromExact)
            {
                if (pointArr == null)
                {
                    continue;
                }
                resultsExact.AddRange(pointArr);
            }

            HashSet<Point?> finalResultsExact = new HashSet<Point?>();

            for (int i = 0; i < resultsExact.Count; i++)
            {
                if (!(resultsExact[i] == null))
                {
                    finalResultsExact.Add(resultsExact[i]);
                }
            }
            //Finally we print out all the locations that we found 
            foreach (Point point in finalResultsExact)
            {
                Console.WriteLine(point.Y + "," + point.X);
            }
        }
        
        /*
         * This is the exact search function
         */
        private static Point?[] FindBitMapExact(Bitmap[] bitmapsToSearch, Bitmap bitmapToFind, Point[] offset)
        {
            if (bitmapsToSearch == null || bitmapToFind == null || offset == null) // validity check
            {
                return null;
            }
           
            Point?[] results = new Point?[bitmapsToSearch.Length];
            /*
             *  Here we iterate over each pixel of the tile and compare them to the small image 
             */
            for (int k = 0; k < bitmapsToSearch.Length; k++)
            {
                for (int x = 0; x < bitmapsToSearch[k].Width - bitmapToFind.Width + 1; x++)
                {
                    for (int y = 0; y < bitmapsToSearch[k].Height - bitmapToFind.Height + 1; y++)
                    {
                        bool match = true;

                        for (int i = 0; i < bitmapToFind.Width; i++)
                        {
                            for (int j = 0; j < bitmapToFind.Height; j++)
                            {
                                if (bitmapsToSearch[k].GetPixel(x + i, y + j) != bitmapToFind.GetPixel(i, j)) // if there is different pixels then  the pictures dont match
                                {
                                    match = false;
                                    break;
                                }
                            }

                            if (!match) // if one pixel is different we move to the next tile
                            {
                                break;
                            }
                        }

                        if (match) // if there is a match we create a point and add it to the array of the results 
                        {
                            int matchX = x + bitmapToFind.Width / 2 + offset[k].X;
                            int matchY = y + bitmapToFind.Height / 2 + offset[k].Y;
                            
                            results[k] = new Point(x + offset[k].X, y + offset[k].Y);
                            
                        }
                    }
                }
            }
            return results;
        }
        // This is the eucldian search function 
        private static Point?[] FindBitMapEuclidian(Bitmap[] bitmapsToSearch, Bitmap bitmapToFind, Point[] offset)
        {
            Point?[] results = new Point?[bitmapsToSearch.Length];
            /*
             *  Here we iterate over each pixel of the tile and compare them to the small image 
             */
            for (int k = 0; k < bitmapsToSearch.Length; k++)
            {
                for (int x = 0; x < bitmapsToSearch[k].Width - bitmapToFind.Width + 1; x++)
                {
                    for (int y = 0; y < bitmapsToSearch[k].Height - bitmapToFind.Height + 1; y++)
                    {
                        bool match = true;

                        for (int i = 0; i < bitmapToFind.Width; i++)
                        {
                            for (int j = 0; j < bitmapToFind.Height; j++)
                            {
                                /*
                                 * Here we collect the red green and blue values of the pixels that we are comparing now 
                                 */
                                int redBig = bitmapsToSearch[k].GetPixel(x + i, y + j).R;
                                int redSmall = bitmapToFind.GetPixel(i, j).R;
                                int greenBig = bitmapsToSearch[k].GetPixel(x + i, y + j).G;
                                int greenSmall = bitmapToFind.GetPixel(i, j).G;
                                int blueSmall = bitmapsToSearch[k].GetPixel(x + i, y + j).B;
                                int blueBig = bitmapToFind.GetPixel(i, j).B;
                                // here we calculate the euclidian distance and check if its equal to zero 
                                if (Math.Sqrt(((redBig - redSmall) * (redBig - redSmall) +
                                               (greenBig - greenSmall) * (greenBig - greenSmall) +
                                               (blueSmall - blueBig) * (blueSmall - blueBig))) != 0)
                                {
                                    match = false;
                                    break;
                                }
                            }

                            if (!match)
                            {
                                break;
                            }
                        }
                        // if all the pixels of the tile pass the test then we add them to the result array 
                        if (match)
                        {
                            int matchX = x + bitmapToFind.Width / 2 + offset[k].X;
                            int matchY = y + bitmapToFind.Height / 2 + offset[k].Y;
                            
                            results[k] = new Point(x + offset[k].X, y + offset[k].Y);
                        }
                    }
                }
            }

            return results;
        }
    }
}