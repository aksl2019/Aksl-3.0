using System;
using System.Threading.Tasks;

namespace SocketServer.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await SocketListener.Instance.StartAsync();

            #region DataflowProducerConsumer
            //--DataflowProducerConsumer--
            //DataflowBulkInserter-935926-Orders: Information: ----dataflow bulk insert 1000000 orders,cost time:"00:49:45.3879817",transport time:00:49:45.3836498,count/time(sec):314,now:"22:27:46.4543322"----

            //DataflowBulkInserter - 343339 - Orders: Information: ----dataflow bulk insert 1000000 orders,cost time:"00:22:53.3610670",transport time:00:22:53.3585643,count / time(sec):250,now: "00:40:42.9397523"----
            //DataflowBulkInserter - 343339 - Orders: Information: ----dataflow bulk insert 3000000 orders,cost time:"03:14:18.7672947",transport time:03:14:18.7669448,count/time(sec):172,now:"07:04:00.5787653"----

            //SqlOrderRepository: Information: ----finish dataflow bulk insert 29411 orders,cost time:00:02:27.8342337,ThreadId=29,now:15:37:58.8292521"----
            //DataflowBulkInserter - 1000000 - Orders: Information: ----dataflow bulk insert 2000000 orders,cost time:"01:09:28.6282096",transport time:01:09:28.6253065,count / time(sec):240,now: "15:37:58.8805557"----
            #endregion

            #region PipeProducerConsumer
            //--PipeProducerConsumer--
            #region 5
            //DataflowPipe-5
            //DataflowBulkInserter-22514-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.90mb","34.76mb"),total cost time:"00:03:32.2172819",mb/second:"0.61",now:"01:40:58.6450173"----

            //DataflowDuplexPipe-5
            //DataflowBulkInserter-11430-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.89mb","34.76mb"),total cost time:"00:03:39.8349207",mb/second:"0.57",now:"01:34:14.2053212"----

            //Pipe-5
            //DataflowBulkInserter-60010-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.89mb","34.75mb"),total cost time:"00:03:33.7545298",mb/second:"0.63",now:"01:28:48.8849746"----

            //BytesDuplexPipe-5
            //DataflowBulkInserter-24668-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.89mb","34.75mb"),total cost time:"00:03:37.4517221",mb/second:"0.60",now:"01:23:06.6143939"----

            //StringsDuplexPipe-5
            //DataflowBulkInserter-37978-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.90mb","34.76mb"),total cost time:"00:03:31.6950621",mb/second:"0.63",now:"01:17:05.0180462"----
            #endregion

            #region 500000=50万
            //500000=(25*20000)+vs
            //DataflowBulkInserter-490838-Orders: Information: ----dataflow bulk insert 500000 total orders=("159.49mb","174.26mb"),total cost time:"00:21:47.8334631",mb/second:"0.40",now:"23:13:05.3039566"----


            //500000=(25*20000)+exe
            #endregion

            #region 100000=100万
            //100000=50*20000+vs
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:00:54.3017454,Count="14576",count/time(sec):269,ThreadId=25,now:"17:01:09.5295808"
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:01:47.7530114,Count="14575",count/time(sec):136,ThreadId=68,now:"17:02:02.9808551"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 29151 orders,cost time:00:01:47.7616681,ThreadId=72,now:17:02:02.9841456"----
            //DataflowBulkInserter-932844-Orders: Information: ----dataflow bulk insert 932844 orders=("297.57mb","322.48mb"),cost time:"00:44:58.0576334",transport time:"00:44:54.0487445",mb/second:"0.23",now:"17:02:03.0800221"---
            //DataflowBulkInserter-932844-Orders: Information: ----dataflow bulk insert 1000000 total orders=("318.93mb","345.64mb"),total cost time:"00:49:33.4233818",mb/second:"0.39",now:"17:02:03.0826895"----

            //100000=50*20000+Duplex+vs
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:00:48.8471604,Count="12948",count/time(sec):266,ThreadId=15,now:"09:06:35.1027941"
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:01:37.7035935,Count="12948",count/time(sec):133,ThreadId=34,now:"09:07:23.9592337"
            //DataflowBulkInserter: Information: CreateBlockers's ExecutionTime=00:02:26.5260849,Count="12948",count/time(sec):89,ThreadId=32,now:"09:08:12.7817276"
            //SqlOrderRepository: Information: ----finish dataflow bulk insert 38844 orders,cost time:00:02:26.5331748,ThreadId=31,now:09:08:12.7846764"----
            //DataflowBulkInserter-971105-Orders: Information: ----dataflow bulk insert 971105 orders=("309.77mb","338.47mb"),cost time:"00:47:43.5946805",transport time:"00:47:39.7324634",mb/second:"0.23",now:"09:08:12.9040929"----
            //DataflowBulkInserter-971105-Orders: Information: ----dataflow bulk insert 1000000 total orders=("318.99mb","348.54mb"),total cost time:"00:49:02.3714885",mb/second:"0.48",now:"09:08:12.9069567"----

            //100000=(25*20000)*2+exe
            //DataflowBulkInserter:  CreateBlockers's ExecutionTime=00:00:49.5269773,Count="14537",count/time(sec):294,ThreadId=54,now:"22:16:18.2382574"
            //DataflowBulkInserter:  CreateBlockers's ExecutionTime=00:01:39.0970713,Count="14537",count/time(sec):147,ThreadId=60,now:"22:17:07.8084248"
            //SqlOrderRepository: Information:   ----finish dataflow bulk insert 29074 orders,cost time:00:01:39.0998580,ThreadId=52,now:22:17:07.8085343"----
            //DataflowBulkInserter - 872228- Orders:  ----dataflow bulk insert 1000000 orders,cost time:"00:39:59.4681371",transport time:00:39:59.4676751,count/time(sec):364,now:"22:17:07.8247135"----
            #endregion

            #region 5000000=5百万
            //500000=(50*100_000)+exe
            //DataflowBulkInserter: CreateBlockers's ExecutionTime=00:02:10.1657908,Count="13174",count/time(sec):102,ThreadId=19,now:"10:35:28.7769213"
            //DataflowBulkInserter: CreateBlockers's ExecutionTime=00:04:24.8509410,Count="13174",count/time(sec):50,ThreadId=19,now:"10:37:43.4620554"
            //DataflowBulkInserter: CreateBlockers's ExecutionTime=00:06:37.9781349,Count="13173",count/time(sec):34,ThreadId=41,now:"10:39:56.5892719"
            //SqlOrderRepository: Information:  ----finish dataflow bulk insert 39521 orders,cost time:00:06:37.9795253,ThreadId=23,now:10:39:56.5894457"----
            //DataflowBulkInserter - 5000000- Orders: ----dataflow bulk insert 5000000 orders,cost time:"04:16:14.8875320",transport time:04:16:14.8873928,count/time(sec):108,now:"10:39:56.6370960"----

            //500000=(50*100_000)+exe
            #endregion
            #endregion

            #region SubTasks
            #region Json.Pipe
            //300000=(1000*30*10)+vs
            //DataflowProducerConsumer
            //  DataflowBulkInserter-10010-Orders: Information: ----dataflow bulk insert 300000 total orders=("93.3354mb","102.2043mb"),total cost time:"00:11:22.9656435",mb/second:"5.3586",now:"20:41:47.8429738"----

            //500000=(2000*25*10)+vs
            //DataflowProducerConsumer
            //  DataflowBulkInserter-335475-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.4878mb","170.2700mb"),total cost time:"00:17:48.8008082",mb/second:"0.6065",now:"15:29:44.8960057"----

            //560000=(2000*28*10)+vs
            //DataflowProducerConsumer
            #endregion

            #region Json.DuplexPipe.Byte
            //300000=(1000*30*10)+vs
            //DataflowProducerConsumer
            //  DataflowBulkInserter-10005-Orders: Information: ----dataflow bulk insert 300000 total orders=("93.4675mb","102.3368mb"),total cost time:"00:14:25.2849651",mb/second:"5.1137",now:"22:17:51.3841829"----

            //500000=(2000*25*10)+vs
            //DataflowProducerConsumer
            //  DataflowBulkInserter-354445-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.5122mb","170.2942mb"),total cost time:"00:19:16.9658263",mb/second:"0.5504",now:"14:48:08.5141294"----

            //560000=(2000*28*10)+vs
            //DataflowProducerConsumer
            //  DataflowBulkInserter-432920-Orders: Information: ----dataflow bulk insert 560000 total orders=("174.1759mb","190.7315mb"),total cost time:"00:23:31.2094501",mb/second:"0.5056",now:"16:44:20.5709406"----
            //PipeProducerConsumer
            //  DataflowBulkInserter - 282160 - Orders: Information: ----dataflow bulk insert 560000 total orders = ("174.1799mb", "190.7355mb"), total cost time:"00:22:56.3447282",mb/second:"0.6017",now: "00:09:30.3655288"----

            //1120000=(2000*28*20)+vs
            //DataflowProducerConsumer
            //  DataflowBulkInserter-6340-Orders: Information: ----dataflow bulk insert 1120000 total orders=("348.7225mb","381.8345mb"),total cost time:"01:05:07.2578033",mb/second:"17.4235",now:"22:22:24.5631233"----
            #endregion

            #region Json.DuplexPipe.Generic
            //300000=(1000*30*10)+vs
            //DataflowProducerConsumer
            //  DataflowBulkInserter - 178775 - Orders: Information: ----dataflow bulk insert 300000 total orders = ("93.3318mb", "102.2012mb"), total cost time:"00:09:55.3480038",mb/second:"0.6546",now: "17:38:06.4315927"----
            //PipeProducerConsumer
            //  DataflowBulkInserter-18705-Orders: Information: ----dataflow bulk insert 300000 total orders=("93.3404mb","102.2095mb"),total cost time:"00:11:19.8508948",mb/second:"0.5553",now:"19:19:47.0468079"----

            //500000=(2000*25*10)+vs
            //DataflowProducerConsumer
            //  DataflowBulkInserter - 390455 - Orders: Information: ----dataflow bulk insert 500000 total orders = ("155.4730mb", "170.2550mb"), total cost time:"00:19:22.8101917",mb/ econd:"0.5338",now: "18:04:09.8643308"----
            //PipeProducerConsumer
            //  DataflowBulkInserter-489195-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.5152mb","170.2971mb"),total cost time:"00:18:34.8963993",mb/second:"0.3802",now:"20:04:58.4217466"----

            //560000=(2000*28*10)+vs
            //DataflowProducerConsumer
            //  DataflowBulkInserter-382690-Orders: Information: ----dataflow bulk insert 560000 total orders=("174.1489mb","190.7046mb"),total cost time:"00:21:34.9857010",mb/second:"0.5639",now:"18:44:22.6841393"----
            //  DataflowBulkInserter-12800-Orders: Information: ----dataflow bulk insert 560000 total orders=("174.1587mb","190.7147mb"),total cost time:"00:21:52.6884739",mb/second:"8.7006",now:"18:29:20.8076173"----
            //  DataflowBulkInserter-1895-Orders: Information: ----dataflow bulk insert 560000 total orders=("174.1270mb","190.6829mb"),total cost time:"00:24:12.8144991",mb/second:"11.4102",now:"01:32:23.2909482"----
            //  DataflowBulkInserter-12290-Orders: Information: ----dataflow bulk insert 560000 total orders=("174.1895mb","190.7454mb"),total cost time:"00:30:14.8732040",mb/second:"8.9189",now:"13:45:47.4113197"----
            //PipeProducerConsumer
            //  DataflowBulkInserter-157140-Orders: Information: ----dataflow bulk insert 560000 total orders=("174.1915mb","190.7473mb"),total cost time:"00:21:29.6336593",mb/second:"0.6212",now:"20:38:42.9462351"----

            //580000=(2000*28*10)+exe
            //DataflowProducerConsumer
            //  DataflowBulkInserter-240000-Orders: Information:  ----dataflow bulk insert 560000 total orders=("174.1377mb","190.6934mb"),total cost time:"00:15:59.3375172",mb/second:"1.0412",now:"22:19:12.3182090"----
            #endregion
            #endregion

            #region Tasks
            #region Json.DataflowPipe 
            //100000=(1000*100)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    DataflowBulkInserter-37466-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.6563mb","34.6128mb"),total cost time:"00:03:45.0329760",mb/second:"0.5923",now:"23:26:14.8013935"----(2016)
            //    DataflowBulkInserter-40282-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.6551mb","33.8275mb"),total cost time:"00:04:23.0155311",mb/second:"0.5157",now:"19:43:41.4246063"----(2019)
            //    DataflowBulkInserter-18745-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.6554mb","34.0397mb"),total cost time:"00:04:10.8614606",mb/second:"0.5249",now:"00:30:33.7907011"----
            //----TaskChannelProducerConsumer----
            //
            #endregion

            #region Json.DataflowDuplexPipe
            //100000=(1000*100)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    DataflowBulkInserter-42510-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.6552mb","34.6115mb"),total cost time:"00:03:41.9516010",mb/second:"0.6030",now:"22:55:58.2669608"----
            //----TaskChannelProducerConsumer----
            //    DataflowBulkInserter-39914-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.6551mb","34.6118mb"),total cost time:"00:03:42.0563620",mb/second:"0.6083",now:"22:25:50.2346318"----
            #endregion

            #region Json.Pipe
            #region 100000=(5000*20)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    DataflowBulkInserter-65970-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.1568mb","34.1131mb"),total cost time:"00:03:11.6263312",mb/second:"0.6740",now:"10:52:51.7256960"----
            //    DataflowBulkInserter-25385-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.2603mb","33.9306mb"),total cost time:"00:04:19.5109276",mb/second:"0.5089",now:"13:26:20.4119072"----(2019)
            //----TaskChannelProducerConsumer----
            #endregion

            #region 5000000=(5000*100)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    DataflowBulkInserter-408529-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.8481mb","170.6300mb"),total cost time:"00:19:17.5402012",mb/second:"0.5471",now:"00:55:43.8259923"----
            //----TaskChannelProducerConsumer----
            //
            #endregion

            #region 10000000=(5000*200)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    DataflowBulkInserter-948160-Orders: Information: ----dataflow bulk insert 1000000 total orders=("312.1982mb","341.7620mb"),total cost time:"00:46:10.8397856",mb/second:"0.4002",now:"12:25:18.6409590"----
            //----TaskChannelProducerConsumer----
            //
            #endregion

            #region 500000=(10000*50)+vs
            //----DataflowProducerConsumer----
            //    DataflowBulkInserter-476010-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.8394mb","170.6215mb"),total cost time:"00:19:28.7422048",mb/second:"0.4131",now:"13:25:33.1597203"----
            //----PipeProducerConsumer----
            //    DataflowBulkInserter-288470-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.8155mb","170.5971mb"),total cost time:"00:20:49.0404286",mb/second:"0.6052",now:"13:12:56.7062288"----
            //    DataflowBulkInserter-266720-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.8768mb","170.6583mb"),total cost time:"00:20:40.0993652",mb/second:"0.6425",now:"01:31:00.7860924"----
            //    DataflowBulkInserter-456360-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.8785mb","170.6607mb"),total cost time:"00:15:56.4051527",mb/second:"0.5519",now:"11:15:07.0698693"----
            //----PipeProducerConsumerEx----
            //----TaskChannelProducerConsumer----
            //    DataflowBulkInserter-412110-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.8599mb","170.6414mb"),total cost time:"00:21:11.4109814",mb/second:"0.4811",now:"12:26:29.7229215"----
            #endregion

            #region  10000000=(10000*100)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    DataflowBulkInserter-285835-Orders: Information: ----dataflow bulk insert 1000000 total orders=("311.6851mb","341.2492mb"),total cost time:"00:49:05.1786251",mb/second:"0.5169",now:"14:16:11.3037430"----
            //    DataflowBulkInserter-949876-Orders: Information: ----dataflow bulk insert 1000000 total orders=("311.7251mb","341.2889mb"),total cost time:"00:46:57.0842774",mb/second:"0.3723",now:"02:34:13.5557685"----
            //----TaskChannelProducerConsumer----
            #endregion

            #region 10000000=(10000*100)+exe
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    
            //----TaskChannelProducerConsumer----
            //
            #endregion
            #endregion

            #region Json.DuplexPipe.Byte
            #region 100000=(5000*20)+vs
            //----DataflowProducerConsumer----
            //    DataflowBulkInserter-28530-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.1964mb","33.8668mb"),total cost time:"00:04:02.7228607",mb/second:"0.5400",now:"16:22:01.2281553"----
            //----PipeProducerConsumer----
            //    DataflowBulkInserter-27065-Orders: Information: ----dataflow bulk insert 100000 total orders=("31.1815mb","33.8518mb"),total cost time:"00:03:52.5918533",mb/second:"0.5622",now:"16:11:53.8670172"----(2019)
            //----TaskChannelProducerConsumer----
            //
            #endregion

            #region 5000000=(5000*100)+vs
            //----DataflowProducerConsumer----
            //    DataflowBulkInserter-381440-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.8724mb","170.6543mb"),total cost time:"00:18:24.9300274",mb/second:"0.5718",now:"17:13:56.9711667"----
            //----PipeProducerConsumer----
            //    
            //----TaskChannelProducerConsumer----
            //
            #endregion

            #region 0000000=(5000*200)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    
            //----TaskChannelProducerConsumer----
            //
            #endregion

            #region 500000=(10000*50)+vs
            //----DataflowProducerConsumer----
            //    DataflowBulkInserter-356264-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.8346mb","170.6167mb"),total cost time:"00:20:18.7427838",mb/second:"0.5467",now:"20:19:41.4191985"----
            //----PipeProducerConsumer----
            //  
            //----PipeProducerConsumerEx---- 
            //----TaskChannelProducerConsumer----
            //  
            #endregion

            #region 10000000=(10000*100)+vs 
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    DataflowBulkInserter-909100-Orders: Information: ----dataflow bulk insert 1000000 total orders=("311.6773mb","341.2413mb"),total cost time:"00:59:56.0768052",mb/second:"0.3350",now:"00:14:27.4582639"----
            //----TaskChannelProducerConsumer----
            //
            #endregion

            #region 50000000=(10000*500)+exe
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    DataflowBulkInserter-1609016-Orders: Information: ----dataflow bulk insert 5000000 total orders=("1.5259gb","1.6702gb"),total cost time:"08:22:27.4859758",mb/second:"0.5421",now:"10:11:05.7074858"----  
            //----TaskChannelProducerConsumer----
            //
            #endregion
            #endregion

            #region Json.DuplexPipe.Generic
            #region 100000=(5000*20)+vs
            //----DataflowProducerConsumer----
            //    
            //----PipeProducerConsumer----
            //   
            //----TaskChannelProducerConsumer----
            //
            #endregion

            #region 5000000=(5000*100)+vs
            //----DataflowProducerConsumer----
            //    

            //----PipeProducerConsumer----
            //    
            //----TaskChannelProducerConsumer----
            //
            #endregion

            #region 10000000=(5000*200)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    DataflowBulkInserter-116115-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.9314mb","170.7134mb"),total cost time:"00:20:06.6307231",mb/second:"0.6185",now:"20:32:21.4704006"----
            //----TaskChannelProducerConsumer----
            //
            #endregion

            #region 500000=(10000*50)+vs
            //----DataflowProducerConsumer----
            //    DataflowBulkInserter-391670-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.8097mb","169.1613mb"),total cost time:"00:19:03.0362523",mb/second:"0.5369",now:"16:56:20.6718473"----(2019)
            //----PipeProducerConsumer----
            //    DataflowBulkInserter-116115-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.9314mb","170.7134mb"),total cost time:"00:20:06.6307231",mb/second:"0.6185",now:"20:32:21.4704006"----
            //----PipeProducerConsumerEx---- 
            //----TaskChannelProducerConsumer----
            //  
            #endregion

            #region 10000000=(10000*100)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    DataflowBulkInserter-961600-Orders: Information: ----dataflow bulk insert 1000000 total orders=("311.7111mb","341.2748mb"),total cost time:"00:43:58.3334936",mb/second:"0.3463",now:"23:07:57.9941353"----
            //----TaskChannelProducerConsumer----
            //
            #endregion

            #region 5000000=(10000*500)+exe
            //----DataflowProducerConsumer----
            //   
            //----PipeProducerConsumer----
            //   DataflowBulkInserter-1290938-Orders: Information:----dataflow bulk insert 5000000 total orders=("1.5259gb","1.6542gb"),total cost time:"06:49:45.0087472",mb/second:"0.7434",now:"07:37:35.8405469"----
            //----TaskChannelProducerConsumer----
            //
            #endregion
            #endregion

            #endregion

            Console.ReadLine();
        }
    }
}