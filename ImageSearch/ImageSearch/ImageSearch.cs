using System;
using System.Drawing;
using System.Runtime.Remoting.Channels;
using System.Threading;

namespace ImageSearch
{
    public class ImageSearch
    {
        public static void FirstSearchOption(string[] args)
        {
            string image1Input = args[0]; // the bigger picture
            string image2Input = args[1]; // the smaller picture
            int nThreadsInput = int.Parse(args[2]);
            string algorithmInput = args[3];

            /*Bitmap image1 = new Bitmap(image1Input);
            Bitmap image2 = new Bitmap(image2Input);*/

            Image image1 = Image.FromFile(image1Input);
            Image image2 = Image.FromFile(image2Input);

            /*Color[,] colorsImg1 = new Color[image1.Width, image1.Height];
            Color[,] colorsImg2 = new Color[image2.Width, image2.Height];

            for (int y = 0; y < image1.Height; y++)
            {
                for (int x = 0; x < image1.Width; x++)
                {
                    colorsImg1[x,y] = image1.GetPixel(x, y);
                }
            }
            
            for (int y = 0; y < image2.Height; y++)
            {
                for (int x = 0; x < image2.Width; x++)
                {
                    colorsImg2[x,y] = image2.GetPixel(x, y);
                }
            }*/

            int smallWidth = image2.Width;
            int smallHeight = image2.Height;
            int overlapWidth = smallWidth / 2;
            int overlapHeight = smallHeight / 2;
            
            int numTilesX = (image1.Width - overlapWidth) / (smallWidth - overlapWidth);
            int numTilesY = (image1.Height - overlapHeight) / (smallHeight - overlapHeight);

            Bitmap[,] tiles = new Bitmap[numTilesX,numTilesY];

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

                    tiles[i, j] = tile;
                }
            }

            int tilesPerThread = (int)Math.Ceiling((double)(numTilesX * numTilesY) / nThreadsInput); // make sure that all the tiles are scanned

            Thread[] threads = new Thread[nThreadsInput];
            
        }

        private bool SearchBitMap(Bitmap tileFromBigImage, Bitmap smallImage)
        {
            for (int i = 0; i < tileFromBigImage.Width; i++)
            {
                for (int j = 0; j < tileFromBigImage.Height; j++)
                {
                    if (tileFromBigImage.GetPixel(i, j) != smallImage.GetPixel(i, j))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}