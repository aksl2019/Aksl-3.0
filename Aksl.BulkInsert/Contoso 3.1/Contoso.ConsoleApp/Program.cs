using System;
using System.Threading.Tasks;

namespace Contoso.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await WebApiSender.Instance.InitializeTask();

           //await WebApiSender.Instance.BulkCopyAsync();

            //await WebApiSender.Instance.DeleteSaleOrdersAsync();

            //await WebApiSender.Instance.UpdateSaleOrdersAsync();

            //await WebApiSender.Instance.GetAllPagedSaleOrdersAsync();

            #region Dataflow Retry
            //10万
            //await WebApiSender.Instance.DataflowBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 5000, maxRetryCount: 2);
            //await WebApiSender.Instance.DataflowBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 10000, maxRetryCount: 1);
            //await WebApiSender.Instance.DataflowBulkInsertBlockAsync (orderCount: 100_000);
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:01:14.2061584,Count="5000",count/time(sec):68,ThreadId=9,now:"19:45:28.7684327"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:01:14.3126451,ThreadId = 12,now: 19:45:28.7702170"----
            //DataflowBulkInser - 1 - OrderCount:100000: Information: ----finish dataflow bulk insert 100000 orders,cost time:"00:02:31.9789927",transport time:00:02:29.9639361,count / time(sec):667,now: "19:45:29.7857405"----

            //DataflowBulkInser - 1:100000: Information: ----dataflow bulk insert 100000 orders,cost time:"00:03:21.5859265",transport time:00:03:21.4077372,count / time(sec):497,now: "18:46:49.9859865"----

            //20万
            //await WebApiSender.Instance.DataflowBulkInsertRetryTasksAsync(maxRetryCount: 200, orderCount: 1000);//每次发送1000
           // await WebApiSender.Instance.DataflowBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 10000, maxRetryCount: 2);
            //2.2
            //DataflowBulkInserter:Information: CreateBlockers's ExecutionTime=00:00:06.7524192,Count="1000",count/time(sec):149,ThreadId=10,now:"00:31:25.8603656"
            //SqlOrderRepository:Information: ----finish dataflow bulk insert 1000 orders,cost time:00:00:06.7581398,ThreadId=12,now:00:31:25.8632594"----
            //DataflowBulkInser-1:200000:Information: ----dataflow bulk insert 200000 orders,cost time:"00:14:38.1658521",transport time:00:14:37.9381377,count/time(sec):228,now:"00:31:25.8666704"----
            //3.0-10000
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:00:06.2953531,Count="1000",count/time(sec):159,ThreadId=12,now:"18:16:24.1424439"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 1000 orders,cost time:00:00:06.3002218,ThreadId = 8,now: 18:16:24.1452389"----
            //DataflowBulkInser - 1:200000: Information: ----dataflow bulk insert 200000 orders,cost time:"00:13:47.7472711",transport time:00:13:47.5008418,count / time(sec):242,now: "18:16:24.1476880"----
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:02:26.6749302,Count="10000",count/time(sec):69,ThreadId=29,now:"19:55:03.9337290"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 10000 orders,cost time:00:02:26.8486803,ThreadId = 5,now: 19:55:03.9370846"----
            //DataflowBulkInser - 1 - OrderCount:200000: Information: ----finish dataflow bulk insert 200000 orders,cost time:"00:05:01.1372182",transport time:00:04:59.1239372,count / time(sec):669,now: "19:55:04.9446089"----

            //50万
            //await WebApiSender.Instance.DataflowBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 5000, maxRetryCount: 10);
            await WebApiSender.Instance.DataflowBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount:10000, maxRetryCount: 5);
            //2.2
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:02:37.1447949,Count="5000",count/time(sec):32,ThreadId=41,now:"17:28:13.5685400"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:02:37.1579711,ThreadId = 39,now: 17:28:13.5720131"----
            //DataflowBulkInser - 1 - OrderCount:500000:Information: ----dataflow bulk insert 500000 orders,cost time:"00:17:59.0701464",transport time:00:17:48.9966265,count / time(sec):468,now: "17:28:14.5821800"----
            //3.0-10000
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:02:18.2332050,Count="5000",count/time(sec):37,ThreadId=7,now:"19:23:54.1069227"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:02:18.2782964,ThreadId = 5,now: 19:23:54.1091664"----
            //DataflowBulkInser - 1 - OrderCount:500000: Information: ----dataflow bulk insert 500000 orders,cost time:"00:16:31.7827309",transport time:00:16:21.6815672,count / time(sec):510,now: "19:23:55.1262967"----
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:02:23.8451716,Count="5000",count/time(sec):35,ThreadId=36,now:"22:41:31.9912668"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:02:23.9427723,ThreadId = 19,now: 22:41:31.9944313"----
            //DataflowBulkInser - 1 - OrderCount:500000: Information: ----dataflow bulk insert 500000 orders,cost time:"00:16:26.7847103",transport time:00:16:16.7461943,count / time(sec):512,now: "22:41:33.0002899"----

            //100万
            //await WebApiSender.Instance.DataflowBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 5000, maxRetryCount: 20);
            //await WebApiSender.Instance.DataflowBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 10000, maxRetryCount: 10);
            //2.2
            //3.0-5000
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:04:13.3481732,Count="5000",count/time(sec):20,ThreadId=44,now:"20:18:51.7942878"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:04:13.4384616,ThreadId=45,now:20:18:51.7969763"----
            //DataflowBulkInsert-1-OrderCount:1000000: Information: ----finish dataflow bulk insert 1000000 orders,cost time:"00:50:34.4764213",transport time:00:50:14.3726538,count/time(sec):332,now:"20:18:52.8017394"----
            //3.0-10000
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:04:22.2126087,Count="10000",count/time(sec):39,ThreadId=23,now:"18:51:42.9629980"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 10000 orders,cost time:00:04:22.2612813,ThreadId=10,now:18:51:42.9658788"----
            //DataflowBulkInsert-1-OrderCount:1000000: Information: ----finish dataflow bulk insert 1000000 orders,cost time:"00:30:18.8747302",transport time:00:30:08.7867855,count/time(sec):553,now:"18:51:43.9866505"----

            //200万
            //await WebApiSender.Instance.DataflowBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount:10_000, maxRetryCount: 20);
            //3.0-10000
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:08:04.2288147,Count="10000",count/time(sec):21,ThreadId=27,now:"18:05:42.3317410"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 10000 orders,cost time:00:08:04.2579174,ThreadId=32,now:18:05:42.3350377"----
            //DataflowBulkInsert-1-OrderCount:2000000: Information: ----finish dataflow bulk insert 2000000 orders,cost time:"01:38:11.3633650",transport time:01:37:51.2196517,count/time(sec):341,now:"18:05:43.3432905"----
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:07:45.8131086,Count="10000",count/time(sec):22,ThreadId=13,now:"15:22:35.6199925"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 10000 orders,cost time:00:07:45.8842442,ThreadId=17,now:15:22:35.6230174"----
            //DataflowBulkInsert-1-OrderCount:2000000: Information: ----finish dataflow bulk insert 2000000 orders,cost time:"01:33:49.7025191",transport time:01:33:39.5816103,count/time(sec):356,now:"15:22:36.1424853"----

            // await WebApiSender.Instance.DataflowBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 200_000, maxRetryCount: 1);
            //3.0-200_000
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=01:11:40.2360561,Count="10000",count/time(sec):3,ThreadId=26,now:"23:00:47.0563466"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 200000 orders,cost time:01:58:36.5344479,ThreadId=9,now:23:00:47.0611414"----
            //DataflowBulkInsert-1-OrderCount:2000000: Information: ----finish dataflow bulk insert 2000000 orders,cost time:"01:58:42.5855172",transport time:01:58:41.5696098,count/time(sec):281,now:"23:00:48.2724946"----

            //await WebApiSender.Instance.DataflowBulkInsertRetryTasksAsync(frequency: 1000, taskCount:4, count: 1, orderCount: 50_000, maxRetryCount: 10);
            //3.0-200_000
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 100000 orders,cost time:00:16:05.7022563,ThreadId = 9,now: 03:27:11.8789880"----
            //DataflowBulkInsert - 1 - OrderCount:2000000: Information: ----finish dataflow bulk insert 2000000 orders,cost time:"01:39:23.0445616",transport time:01:39:13.0248798,count / time(sec):336,now: "03:27:12.9855806"----

            //500万
            //100万*5=500万,打开10个App
            //await WebApiSender.Instance.DataflowBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 10000, maxRetryCount: 10);
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:04:53.9241210,Count="10000",count/time(sec):35,ThreadId=18,now:"14:13:34.5431397"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 10000 orders,cost time:00:04:53.9247730,ThreadId = 6,now: 14:13:34.5432688"----
            //DataflowBulkInsert - 1 - OrderCount:1000000: ----finish dataflow bulk insert 1000000 orders,cost time:"00:52:57.7362590",transport time:00:52:47.6924745,count / time(sec):316,now: "14:13:35.5483515"----
            #endregion

            #region DataflowPipe Retry
            //10万
            // await WebApiSender.Instance.DataflowPipeBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount:5000, maxRetryCount: 2);
            //await WebApiSender.Instance.DataflowPipeBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount:10000, maxRetryCount: 1);
            //3.0-5000
            //DataflowPipeBulkInserter: Information: CreateBlockers's ExecutionTime=00:01:10.4871698,Count="5000",ThreadId=4,now:"20:03:43.5996812"
            //SqlOrderRepository: Information: ----finish dataflow pipe bulk insert 5000 orders,cost time:00:01:10.5579993,ThreadId = 22,now: 20:03:43.6023361"----
            //DataflowPipeBulkInser - 1 - OrderCount:100000: Information: ----finish dataflow pipe bulk insert 100000 orders,cost time:"00:02:26.2729464,count/time(sec):684,now:"20:03:44.6073388"----
            //3.0-10000
            //DataflowPipeBulkInserter: Information: CreateBlockers's ExecutionTime=00:02:34.2850004,Count="10000",ThreadId=20,now:"19:47:06.1312225"
            //SqlOrderRepository: Information: ----finish dataflow pipe bulk insert 10000 orders,cost time:00:02:35.4497941,ThreadId = 11,now: 19:47:06.1339939"----
            //DataflowPipeInsert - 1 - OrderCount:100000: Information: ----finish dataflow pipe bulk insert 100000 orders,cost time:"00:02:36.9003015,count/time(sec):638,now:"19:47:07.1532840"----

            //20万
            //await WebApiSender.Instance.DataflowPipeBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount:5000, maxRetryCount: 4);
            //await WebApiSender.Instance.DataflowPipeBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount:10000, maxRetryCount:2);
            //2.2
            //DataflowBulkInserter:Information: CreateBlockers's ExecutionTime=00:00:06.7524192,Count="1000",count/time(sec):149,ThreadId=10,now:"00:31:25.8603656"
            //SqlOrderRepository:Information: ----finish dataflow bulk insert 1000 orders,cost time:00:00:06.7581398,ThreadId=12,now:00:31:25.8632594"----
            //DataflowBulkInser-1:200000:Information: ----dataflow bulk insert 200000 orders,cost time:"00:14:38.1658521",transport time:00:14:37.9381377,count/time(sec):228,now:"00:31:25.8666704"----
            //3.0-5000
            //DataflowPipeBulkInserter: Information: CreateBlockers's ExecutionTime=00:01:35.2958116,Count="5000",ThreadId=32,now:"19:56:57.0261902"
            //SqlOrderRepository: Information: ----finish dataflow pipe bulk insert 5000 orders,cost time:00:01:35.3896689,ThreadId = 5,now: 19:56:57.0285664"----
            //DataflowPipeInsert - 1 - OrderCount:200000: Information: ----finish dataflow pipe bulk insert 200000 orders,cost time:"00:05:29.7190364,count/time(sec):607,now:"19:56:58.0350524"----
            //3.0-10000
            //DataflowPipeBulkInserter: Information: CreateBlockers's ExecutionTime=00:02:41.2344704,Count="10000",ThreadId=16,now:"20:23:12.2925890"
            //SqlOrderRepository: Information: ----finish dataflow pipe bulk insert 10000 orders,cost time:00:02:41.8513507,ThreadId=11,now:20:23:12.2955776"----
            //DataflowPipeInsert-1-OrderCount:200000: Information: ----finish dataflow pipe bulk insert 200000 orders,cost time:"00:05:10.8769208,count/time(sec):644,now:"20:23:13.3021090"----

            //50万
            //await WebApiSender.Instance.DataflowPipeBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 5000, maxRetryCount: 10);
            //await WebApiSender.Instance.DataflowPipeBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 10000, maxRetryCount: 5);
            //2.2
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:02:37.1447949,Count="5000",count/time(sec):32,ThreadId=41,now:"17:28:13.5685400"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:02:37.1579711,ThreadId = 39,now: 17:28:13.5720131"----
            //DataflowBulkInser - 1 - OrderCount:500000:Information: ----dataflow bulk insert 500000 orders,cost time:"00:17:59.0701464",transport time:00:17:48.9966265,count / time(sec):468,now: "17:28:14.5821800"----
            //3.0-5000
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:02:23.8451716,Count="5000",count/time(sec):35,ThreadId=36,now:"22:41:31.9912668"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:02:23.9427723,ThreadId = 19,now: 22:41:31.9944313"----
            //DataflowBulkInser - 1 - OrderCount:500000: Information: ----dataflow bulk insert 500000 orders,cost time:"00:16:26.7847103",transport time:00:16:16.7461943,count / time(sec):512,now: "22:41:33.0002899"----
            //3.0-10000
            //DataflowPipeBulkInserter: Information: CreateBlockers's ExecutionTime=00:03:41.4230638,Count="10000",ThreadId=28,now:"23:01:18.1083364"
            //SqlOrderRepository: Information: ----finish dataflow pipe bulk insert 10000 orders,cost time:00:03:41.5565847,ThreadId=16,now:23:01:18.1114734"----
            //DataflowPipeInsert-1-OrderCount:500000: Information: ----finish dataflow pipe bulk insert 500000 orders,cost time:"00:15:53.5003774,count/time(sec):525,now:"23:01:19.1185488"----

            //100万
            //await WebApiSender.Instance.DataflowPipeBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 10000, maxRetryCount: 10);
            //2.2
            //3.0-10000
            //DataflowPipeBulkInserter: Information: CreateBlockers's ExecutionTime=00:04:36.0614553,Count="10000",ThreadId=18,now:"12:09:56.1339973"
            //SqlOrderRepository: Information: ----finish dataflow pipe bulk insert 10000 orders,cost time:00:04:36.1556860,ThreadId=27,now:12:09:56.1363813"----
            //DataflowPipeInsert-1-OrderCount:1000000: Information: ----finish dataflow pipe bulk insert 1000000 orders,cost time:"00:32:52.1448389,count/time(sec):508,now:"12:09:57.1428844"----
            //DataflowPipeBulkInserter: Information:CreateBlockers's ExecutionTime=00:04:44.7669355,Count="10000",ThreadId=6,now:"16:08:15.8399379"
            //SqlOrderRepository: Information:finish dataflow pipe bulk insert 10000 orders,cost time:00:04:45.3364395,ThreadId=29,now:16:08:15.8400573"----
            //DataflowPipeInsert-1-OrderCount:1000000: Information:----finish dataflow pipe bulk insert 1000000 orders,cost time:"00:29:56.0384273,count/time(sec):557,now:"16:08:16.8449744"----

            //200万
            //await WebApiSender.Instance.DataflowPipeBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 10_000, maxRetryCount: 20);
            //2.2
            //3.0-10000
            //DataflowPipeBulkInserter: Information: CreateBlockers's ExecutionTime=00:09:05.1752381,Count="10000",ThreadId=16,now:"15:08:41.2443104"
            //SqlOrderRepository: Information: ----finish dataflow pipe bulk insert 10000 orders,cost time:00:09:05.1854575,ThreadId=32,now:15:08:41.2470779"----
            //DataflowPipeInsert-1-OrderCount:2000000: Information: ----finish dataflow pipe bulk insert 2000000 orders,cost time:"01:48:27.5800672,count/time(sec):308,now:"15:08:42.2720037"----
            //500万
            //50万*10=500万,打开10个App
            //3.0-10000
            #endregion

            #region Pipe Retry
            //10万
            //await WebApiSender.Instance.PipeBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 5000, maxRetryCount: 2);
            //await WebApiSender.Instance.PipeBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 10000, maxRetryCount: 1);
            //await WebApiSender.Instance.PipeBulkInsertBlockTasksAsync(taskCount: 10,  orderCount: 5000);
            //3.0-5000
            //PipeBulkInserter: Information: PipeBulkInsertAsync's ExecutionTime=00:01:20.2006240,Count="5000",ThreadId=21,now:"19:19:35.5921555"
            //SqlOrderRepository: Information: ----finish bulk insert 5000 orders,cost time:00:01:20.2693023,ThreadId = 21,now: 19:19:35.5942272"----
            //PipeBulkInsert - 1 - OrderCount:100000: Information: ----finish pipe bulk insert 100000 orders,cost time:"00:02:30.6132330",transport time:00:02:28.5947647,count / time(sec):673,now: "19:19:36.6011488"----
            //3.0-10000
            //PipeBulkInserter: Information: PipeBulkInsertAsync's ExecutionTime=00:01:10.5641625,Count="5000",ThreadId=10,now:"19:24:30.7124793"
            //SqlOrderRepository: Information: ----finish bulk insert 5000 orders,cost time:00:01:10.5911085,ThreadId = 10,now: 19:24:30.7151379"----
            //PipeBulkInser - 1 - OrderCount:100000: Information: ----finish pipe bulk insert 100000 orders,cost time:"00:02:33.0497195",transport time:00:02:31.0345736,count / time(sec):663,now: "19:24:31.7196764"----

            //50万
            //await WebApiSender.Instance.PipeBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 10000, maxRetryCount: 5);
            //2.2
            //3.0-10000
            //PipeBulkInserter: Information: PipeBulkInsertAsync's ExecutionTime=00:03:43.7289347,Count="10000",ThreadId=22,now:"14:38:02.2431717"
            //SqlOrderRepository: Information: ----finish bulk insert 10000 orders,cost time:00:03:43.7362578,ThreadId = 22,now: 14:38:02.2449186"----
            //PipeBulkInsert - 1 - OrderCount:500000: Information: ----finish pipe bulk insert 500000 orders,cost time:"00:15:15.0185533",transport time:00:15:09.9910915,count / time(sec):550,now: "14:38:03.2552995"----

            //100万
            //await WebApiSender.Instance.PipeBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 10000, maxRetryCount: 10);
            //2.2
            //3.0-10000
            //PipeBulkInserter: Information: PipeBulkInsertAsync's ExecutionTime=00:04:48.0778430,Count="10000",ThreadId=8,now:"13:27:03.8014514"
            //SqlOrderRepository: Information: ----finish bulk insert 10000 orders,cost time:00:04:48.0871865,ThreadId=8,now:13:27:03.8039154"----
            //PipeBulkInsert-1-OrderCount:1000000: Information: ----finish pipe bulk insert 1000000 orders,cost time:"00:31:49.1033084",transport time:00:31:39.0368305,count/time(sec):527,now:"13:27:04.8129508"----

            //200万
            //await WebApiSender.Instance.PipeBulkInsertRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 10000, maxRetryCount: 20);
            //2.2
            //3.0-10000
            //PipeBulkInserter: Information: PipeBulkInsertAsync's ExecutionTime=00:09:09.5937365,Count="10000",ThreadId=33,now:"15:46:03.3006915"
            //SqlOrderRepository: Information: ----finish bulk insert 10000 orders,cost time:00:09:09.6069397,ThreadId=33,now:15:46:03.3036443"----
            //PipeBulkInsert-1-OrderCount:2000000: Information: ----finish pipe bulk insert 2000000 orders,cost time:"01:47:47.7916588",transport time:01:47:27.6397160,count/time(sec):311,now:"15:46:04.3125210"----
            #endregion

            // Console.WriteLine("Hello World!");

            Console.ReadLine();
        }
    }
}