using System;
using System.Threading.Tasks;

namespace Contoso.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await WebApiSender.Instance.InitializeTask();

            #region Dataflow Retry
            //20万
            //await WebApiSender.Instance.DataflowBulkInsertBlockRetryAsync(maxRetryCount: 200, orderCount: 1000);//每次发送1000
            //2.2
            //DataflowBulkInserter:Information: CreateBlockers's ExecutionTime=00:00:06.7524192,Count="1000",count/time(sec):149,ThreadId=10,now:"00:31:25.8603656"
            //SqlOrderRepository:Information: ----finish dataflow bulk insert 1000 orders,cost time:00:00:06.7581398,ThreadId=12,now:00:31:25.8632594"----
            //DataflowBulkInser-1:200000:Information: ----dataflow bulk insert 200000 orders,cost time:"00:14:38.1658521",transport time:00:14:37.9381377,count/time(sec):228,now:"00:31:25.8666704"----
            //3.0
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:00:06.2953531,Count="1000",count/time(sec):159,ThreadId=12,now:"18:16:24.1424439"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 1000 orders,cost time:00:00:06.3002218,ThreadId = 8,now: 18:16:24.1452389"----
            //DataflowBulkInser - 1:200000: Information: ----dataflow bulk insert 200000 orders,cost time:"00:13:47.7472711",transport time:00:13:47.5008418,count / time(sec):242,now: "18:16:24.1476880"----

            //50万
            // await WebApiSender.Instance.DataflowBulkInsertBlockRetryTasksAsync(frequency: 1000, taskCount: 10, count: 1, orderCount: 5000, maxRetryCount: 10);
            //2.2
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:02:37.1447949,Count="5000",count/time(sec):32,ThreadId=41,now:"17:28:13.5685400"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:02:37.1579711,ThreadId = 39,now: 17:28:13.5720131"----
            //DataflowBulkInser - 1 - OrderCount:500000:Information: ----dataflow bulk insert 500000 orders,cost time:"00:17:59.0701464",transport time:00:17:48.9966265,count / time(sec):468,now: "17:28:14.5821800"----
            //3.0
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:02:18.2332050,Count="5000",count/time(sec):37,ThreadId=7,now:"19:23:54.1069227"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:02:18.2782964,ThreadId = 5,now: 19:23:54.1091664"----
            //DataflowBulkInser - 1 - OrderCount:500000: Information: ----dataflow bulk insert 500000 orders,cost time:"00:16:31.7827309",transport time:00:16:21.6815672,count / time(sec):510,now: "19:23:55.1262967"----

            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:02:23.8451716,Count="5000",count/time(sec):35,ThreadId=36,now:"22:41:31.9912668"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:02:23.9427723,ThreadId = 19,now: 22:41:31.9944313"----
            //DataflowBulkInser - 1 - OrderCount:500000: Information: ----dataflow bulk insert 500000 orders,cost time:"00:16:26.7847103",transport time:00:16:16.7461943,count / time(sec):512,now: "22:41:33.0002899"----

            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:02:15.5663905,Count="5000",count/time(sec):24,ThreadId=13,now:"22:21:07.5611370"
            //SqlOrderRepository: Information:   ----finish dataflow bulk insert 5000 orders,cost time:00:03:36.8189504,ThreadId=32,now:17:12:15.1385123"----
            //DataflowBulkInser - 1 - OrderCount:500000: Information: ----dataflow bulk insert 500000 orders,cost time:"00:16:47.5788941",transport time:00:16:37.5592814,count/time(sec):502,now:"22:21:08.5646312"----

            //50万*10=500万,打开10个App
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:03:36.8129181,Count="5000",count/time(sec):24,ThreadId=15,now:"17:12:15.1383978"
            //SqlOrderRepository: Information:  ----finish dataflow bulk insert 5000 orders,cost time:00:02:15.5670321,ThreadId=10,now:22:21:07.5612604"----
            //DataflowBulkInser - 1 - OrderCount:500000: Information:  ----dataflow bulk insert 500000 orders,cost time:"00:44:15.9471106",transport time:00:44:05.8675905,count/time(sec):189,now:"17:12:16.1577166"----

            //DataflowBulkInserter:CreateBlockers's ExecutionTime=00:03:46.2609539,Count="5000",count/time(sec):23,ThreadId=19,now:"17:05:58.0896806"
            //SqlOrderRepository: Information:----finish dataflow bulk insert 5000 orders,cost time:00:03:46.2612042,ThreadId = 8,now: 17:05:58.0897568"----
            //DataflowBulkInser - 1 - OrderCount:500000: Information: ----dataflow bulk insert 500000 orders,cost time:"00:37:57.5336317",transport time:00:37:47.4870501,count / time(sec):221,now: "17:05:59.0971739"----

            //60万
            //await WebApiSender.Instance.DataflowBulkInsertBlockRetryTasksAsync(frequency: 500, taskCount: 12, count: 1, orderCount: 5000, maxRetryCount: 10);
            //2.2
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:03:36.5273079,Count="5000",ThreadId=46,now:"14:35:39.4126236"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:03:36.5871983,ThreadId = 37,now: 14:35:39.4153365"----
            //DataflowBulkInser - 1 - OrderCount:600000:Information: ----dataflow bulk insert 600000 orders,cost time:"00:25:02.3612023",transport time:00:24:57.2844565,count / time(sec):401,now: "14:35:39.9326504"----
            //3.0
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:03:27.3077146,Count="5000",count/time(sec):25,ThreadId=42,now:"15:27:44.2959500"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:03:27.3128876,ThreadId = 43,now: 15:27:44.2985671"----
            //DataflowBulkInser - 1 - OrderCount:600000: Information: ----dataflow bulk insert 600000 orders,cost time:"00:24:15.3156198",transport time:00:24:10.2843955,count / time(sec):414,now: "15:27:44.8030233"----
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:03:21.5647718,Count="5000",count/time(sec):25,ThreadId=36,now:"18:50:10.3160684"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:03:21.7250028,ThreadId = 26,now: 18:50:10.3188177"----
            //DataflowBulkInser - 1 - OrderCount:600000: Information: ----dataflow bulk insert 600000 orders,cost time:"00:22:25.0276993",transport time:00:22:19.9992913,count / time(sec):448,now: "18:50:10.8242062"----

            //120万
            //await WebApiSender.Instance.DataflowBulkInsertBlockRetryTasksAsync(frequency: 500, taskCount: 12, count: 1, orderCount: 5000, maxRetryCount: 20);
            //2.2
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:06:38.4120889,Count="5000",ThreadId=14,now:"00:59:03.6299909"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:06:38.4171721,ThreadId = 10,now: 00:59:03.6325897"----
            //DataflowBulkInser - 1 - OrderCount:1200000:Information: ----dataflow bulk insert 1200000 orders,cost time:"01:18:47.1240764",transport time:01:18:37.0959011,count / time(sec):255,now: "00:59:04.1426769"----
            //3.0
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:06:09.0826140,Count="5000",count/time(sec):14,ThreadId=59,now:"00:51:48.0596555"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 5000 orders,cost time:00:06:09.1570174,ThreadId = 51,now: 00:51:48.0621999"----
            //DataflowBulkInser - 1 - OrderCount:1200000: Information: ----dataflow bulk insert 1200000 orders,cost time:"01:11:20.2321429",transport time:01:11:10.2128485,count / time(sec):282,now: "00:51:48.5679747"----

            //240万
            //await WebApiSender.Instance.DataflowBulkInsertBlockRetryTasksAsync(frequency: 500, taskCount: 12, count: 1, orderCount: 20000, maxRetryCount: 10);
            //2.2
            //DataflowBulkInserter:Information: CreateBlockers's ExecutionTime=00:26:44.2319721,Count="10000",count/time(sec):7,ThreadId=35,now:"16:03:59.0437152"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 20000 orders,cost time:00:26:47.2547429,ThreadId = 34,now: 16:03:59.0465232"----
            //DataflowBulkInser - 1 - OrderCount:2400000:Information: ----dataflow bulk insert 2400000 orders,cost time:"02:41:44.4752474",transport time:02:41:39.4640383,count / time(sec):248,now: "16:03:59.5769668"----
            //3.0
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:22:52.3633072,Count="10000",count/time(sec):8,ThreadId=13,now:"13:34:43.6738752"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 20000 orders,cost time:00:22:52.5097500,ThreadId = 27,now: 13:34:43.6769341"----
            //DataflowBulkInser - 1 - OrderCount:2400000: Information: ----dataflow bulk insert 2400000 orders,cost time:"02:13:51.3458936",transport time:02:13:46.2987059,count / time(sec):300,now: "13:34:44.1885920"----

            //500万
            //await WebApiSender.Instance.DataflowBulkInsertBlockRetryTasksAsync(frequency: 500, taskCount: 10, count: 1, orderCount: 50000, maxRetryCount: 10);
            //2.2

            //3.0
            #endregion

            //  Console.WriteLine("Hello World!");

            Console.ReadLine();
        }
    }
}
