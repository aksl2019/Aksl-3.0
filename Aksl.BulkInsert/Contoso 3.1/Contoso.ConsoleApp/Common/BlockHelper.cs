using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.ConsoleApp
{
    public class BlockHelper
    {
        public static (int blockCount, int minPerBlock, int maxPerBlock) GetBasciBlockInfo(int messageCount)
        {
            #region Block Method
            int blockCount = Environment.ProcessorCount * 2;//块数

           // int minPerBlock = 5000;//至少
            int minPerBlock = 10000;//至少有一块100条
            int maxPerBlock = 20000;//至多

            if (messageCount > 1000 && messageCount <= 10000)
            {
                blockCount = Environment.ProcessorCount/12;//1

                minPerBlock = 10000;
                maxPerBlock = 10000;//10000*1=10,000
            }
            else if (messageCount > 10000 && messageCount <= 100000)
            {
                blockCount = Environment.ProcessorCount/6;//2

                //minPerBlock = 20000;
                //maxPerBlock = 20000;//20000*4=80,000

                minPerBlock = 100000;
                maxPerBlock = 100000;//20000*2=40,000
            }
            else if (messageCount > 100000 && messageCount <= 500000)
            {
                blockCount = Environment.ProcessorCount ;//12

                minPerBlock =100000;
                maxPerBlock = 100000;//20000*12=2400,000
            }
            else if (messageCount > 500000 && messageCount <= 1000000)//1百万
            {
                blockCount = Environment.ProcessorCount ; 

                minPerBlock = 100000;
                maxPerBlock = 100000;//20000*24=480,000
            }
            else if (messageCount > 1000000 && messageCount <= 5000000)
            {
                blockCount = Environment.ProcessorCount/ 4;//3

                minPerBlock = 20000;
                maxPerBlock = 40000;//100000*6=600,000
            }
            else if (messageCount > 5000000 && messageCount <= 10000000)
            {
                blockCount = Environment.ProcessorCount * 5;

                minPerBlock = 25000;//80*60=4800
                maxPerBlock = 40000;//200*60=12000
            }
            else if (messageCount > 10000000 && messageCount <= 20000000)
            {
                blockCount = Environment.ProcessorCount * 5;

                minPerBlock = 30000;//80*60=4800
                maxPerBlock = 40000;//200*60=12000
            }
            #endregion

            return (blockCount, minPerBlock, maxPerBlock);
        }

        public static int[] GetBlockInfo(int messageCount, int blockCount=0,int minPerBlock = 20, int maxPerBlock = 200)
        {
            blockCount = blockCount==0?  Environment.ProcessorCount * 2 : blockCount;//块数

            //分块
            int[] blockInfos;
            if (messageCount < (blockCount * maxPerBlock))
            {
                //分块
                blockInfos = BlockHelper.MacthBlockInfoDown(blockCount, messageCount, minPerBlock);
            }
            else
            {
                blockInfos = BlockHelper.MacthBlockInfoUp(blockCount, messageCount, maxPerBlock);
            }

            return blockInfos;
        }

        public static IEnumerable<T[]> GetMessageByBlockInfo<T>(int[] blockInfos, T[] messages)
        {
            var count = blockInfos.Sum(b => b);
            var blockMessages = new List<T[]>(count);

            int startPositon = 0;
            for (int i = 0; i < blockInfos.Count(); i++)
            {
                var msgScope = new T[blockInfos[i]];
                for (int j = 0; j < blockInfos[i]; j++)
                {
                    int positon = startPositon + j;
                    var msg = messages[positon];
                    msgScope[j] = msg;
                }
                startPositon += blockInfos[i];//重置起始位置
                blockMessages.Add(msgScope);
            }

            return blockMessages;
        }

        public static int[] GetBlockInfo(int blockCount, int totalCount)
        {
            int[] chunks = new int[blockCount];//分成x块
            int chunkSize = totalCount / blockCount;//每一块的数量
            int chunkMore = totalCount % blockCount;//余下的数量

            if (chunkSize == 0)//小于x
            {
                for (int i = 0; i < chunkMore; i++)
                {
                    chunks[i] = 1;
                }
                chunkSize = 1;
            }
            else
            {
                for (int i = 0; i < blockCount; i++)
                {
                    chunks[i] = chunkSize;
                }
                if (chunkMore > 0)
                {
                    //chunks[blockCount - 1] = chunks[blockCount - 1] + chunkMore;
                    for (int i = 0; i < blockCount && chunkMore > 0; i++)
                    {
                        chunks[i] = chunks[i] + 1;
                        chunkMore--;
                    }
                }
            }

            return chunks;
        }

        public static int[] MacthBlockInfoDown(int blockCount, int totalCount, int minnumPerBlock)
        {
            int fac = blockCount;
            var blockInfos = GetBlockInfo(fac, totalCount);

            bool isLittle = blockInfos.Any(b => b <= 0) || !blockInfos.Any(b => b >= minnumPerBlock);//至少有一个

            while (isLittle && fac > 1)
            {
                fac = fac - 1;
                blockInfos = GetBlockInfo(fac, totalCount);
                isLittle = blockInfos.Any(b => b <= 0) || !blockInfos.Any(b => b >= minnumPerBlock);
                if (isLittle && fac == 2)
                {
                    if (blockInfos.Count() == 2 && blockInfos.Sum(b => b) > minnumPerBlock)
                        isLittle = false;
                }
            };

            return blockInfos;
        }

        public static int[] MacthBlockInfoUp(int blockCount, int totalCount, int maxnumPerBlock)
        {
            int fac = blockCount;
            var blockInfos = GetBlockInfo(fac, totalCount);

            bool isLarge = blockInfos.Any(b => b > maxnumPerBlock);

            while (isLarge)
            {
                fac = fac + 1;
                blockInfos = GetBlockInfo(fac, totalCount);
                isLarge = blockInfos.Any(b => b > maxnumPerBlock);
            };

            return blockInfos;
        }
    }
}

