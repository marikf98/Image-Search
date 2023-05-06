using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Channels;
using System.Threading;

namespace ImageSearch
{
    public class ImageSearch
    {
        public static void FirstSearchOption(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Not enough arguments ");
                Environment.Exit(1);
            }
            string image1Input = args[0]; // the bigger picture
            string image2Input = args[1]; // the smaller picture
            int nThreadsInput = int.Parse(args[2]);
            string algorithmInput = args[3];
            
            Image image1 = Image.FromFile(image1Input);
            Image image2 = Image.FromFile(image2Input);
            Bitmap bitMapImage2 = new Bitmap(image2Input);

            int smallWidth = image2.Width;
            int smallHeight = image2.Height;
            int overlapWidth = smallWidth / 2;
            int overlapHeight = smallHeight / 2;
            
            // int numTilesX = (image1.Width - overlapWidth) / (smallWidth - overlapWidth);
            // int numTilesY = (image1.Height - overlapHeight) / (smallHeight - overlapHeight);
            
            int numTilesX = (int)Math.Ceiling((double)(image1.Width - overlapWidth) / (smallWidth - overlapWidth));
            int numTilesY = (int)Math.Ceiling((double)(image1.Height - overlapHeight) / (smallHeight - overlapHeight));
            

            Bitmap[,] tiles = new Bitmap[numTilesX,numTilesY];
            Point[,] pointsOffset = new Point[numTilesX, numTilesY];

            for (int i = 0; i < numTilesX; i++)
            {
                for (int j = 0; j < numTilesY; j++)
                {
                    int startX = i * (smallWidth - overlapWidth);
                    int startY = j * (smallHeight - overlapHeight);
                    int searchWidth = smallWidth + overlapWidth;
                    int searchHeight = smallHeight + overlapHeight;

                    Bitmap tile = new Bitmap(searchWidth, searchHeight);

                    using (Graphics g = Graphics.FromImage(tile))
                    {
                        g.DrawImage(image1, new Rectangle(0, 0, searchWidth, searchHeight), new Rectangle(startX, startY, searchWidth, searchHeight), GraphicsUnit.Pixel);
                    }

                    pointsOffset[i, j] = new Point(startX, startY);
                    tiles[i, j] = tile;
                }
            }
            Bitmap[] oneDtiles = tiles.Cast<Bitmap>().ToArray();
            Point[] point1D = pointsOffset.Cast<Point>().ToArray();
            Bitmap[] compareBitmap = new Bitmap[nThreadsInput];
            
            for (int i = 0; i < nThreadsInput; i++)
            {
                compareBitmap[i] = new Bitmap(image2Input);
            }
            
            int tilesPerThread = (int)Math.Ceiling((double)(numTilesX * numTilesY) / nThreadsInput); // make sure that all the tiles are scanned

            List<Point?[]> resultFromExact = new List<Point?[]>();
            List<Thread> threads = new List<Thread>();
            Bitmap[][] bitmapSubArrays = new Bitmap[nThreadsInput][];
            Point[][] pointSunArrays = new Point[nThreadsInput][];
            
            int subArraySize = (int)Math.Ceiling((double)oneDtiles.Length / nThreadsInput);
            
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
                    subArrayLength = subArrayLength * (-1);}
                bitmapSubArrays[i] = new Bitmap[subArrayLength];
                pointSunArrays[i] = new Point[subArrayLength];
                Array.Copy(oneDtiles, startIndex, bitmapSubArrays[i], 0, subArrayLength);
                Array.Copy(point1D, startIndex, pointSunArrays[i], 0, subArrayLength);
            }


            if (algorithmInput == "exact")
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
            }

            else if(algorithmInput == "euclidian")
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
            }
            else
            {
                Console.WriteLine("Please enter exact or euclidian");
                Environment.Exit(1);

            }

            
            foreach (Thread thread in threads)
            {
                 thread.Start();
            }
            
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            
            List<Point?> resultsExact = new List<Point?>();
            foreach (Point?[] pointArr in resultFromExact)
            {
                if (pointArr == null)
                {
                    continue;
                }
                resultsExact.AddRange(pointArr);
            }
            
            
            
            //List<Point?> finalResultsExact = new List<Point?>();
            HashSet<Point?> finalResultsExact = new HashSet<Point?>();

            for (int i = 0; i < resultsExact.Count; i++)
            {
                if (!(resultsExact[i] == null))
                {
                    finalResultsExact.Add(resultsExact[i]);
                }
            }
            
            foreach (Point point in finalResultsExact)
            {
                Console.WriteLine(point.X + "," + point.Y);
            }
            
        }
        
        
        private static Point?[] FindBitMapExact(Bitmap[] bitmapsToSearch, Bitmap bitmapToFind, Point[] offset)
        {
            if (bitmapsToSearch == null || bitmapToFind == null || offset == null)
            {
                return null;
            }
           
            Point?[] results = new Point?[bitmapsToSearch.Length];
            for (int k = 0; k < bitmapsToSearch.Length; k++)
            {
                //Console.WriteLine("bitmapsToSearch.Length = " + bitmapsToSearch.Length);
                for (int x = 0; x < bitmapsToSearch[k].Width - bitmapToFind.Width + 1; x++)
                {
                    //Console.WriteLine("bitmapsToSearch[k].Width - bitmapToFind.Width + 1 = " + (bitmapsToSearch[k].Width - bitmapToFind.Width + 1));

                    for (int y = 0; y < bitmapsToSearch[k].Height - bitmapToFind.Height + 1; y++)
                    {
                        //Console.WriteLine("bitmapsToSearch[k].Height - bitmapToFind.Height + 1 = " + (bitmapsToSearch[k].Height - bitmapToFind.Height + 1));

                        bool match = true;

                        for (int i = 0; i < bitmapToFind.Width; i++)
                        {
                            for (int j = 0; j < bitmapToFind.Height; j++)
                            {
                                if (bitmapsToSearch[k].GetPixel(x + i, y + j) != bitmapToFind.GetPixel(i, j))
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

        private static Point?[] FindBitMapEuclidian(Bitmap[] bitmapsToSearch, Bitmap bitmapToFind, Point[] offset)
        {
            Point?[] results = new Point?[bitmapsToSearch.Length];
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
                                int redBig = bitmapsToSearch[k].GetPixel(x + i, y + j).R;
                                int redSmall = bitmapToFind.GetPixel(i, j).R;
                                int greenBig = bitmapsToSearch[k].GetPixel(x + i, y + j).G;
                                int greenSmall = bitmapToFind.GetPixel(i, j).G;
                                int blueSmall = bitmapsToSearch[k].GetPixel(x + i, y + j).B;
                                int blueBig = bitmapToFind.GetPixel(i, j).B;

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