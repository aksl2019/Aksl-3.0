using System;
using System.Threading.Tasks;

namespace Socket.Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await SocketSender.Instance.InitializeTask();

            #region Tasks
            //----(2K+3.0)------  
            #region Json.DataflowPipe 
            #region 100000=(1000*100)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    SocketSender:100000-Orders: Information:  ----Json finish send 100000 orders=("31.7517mb","31.7517mb"),total cost time:"00:17:24.5179779",max cost time:"00:17:20.6784085",max transport time:"00:16:50.7792505",mb/second:"0.1000",now:"19:39:35.4143170"---- (2019)                                      
            //----TaskChannelProducerConsumer----
            //
            // await SocketSender.Instance.SendBatchJsonMessagesDataflowPipeLoopTasksAsync(taskCount: 1000, count: 1, orderCount: 100);
            #endregion
            #endregion

            #region Json.DataflowDuplexPipe
            #region 100000=(1000*100)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    SocketSender:100000-Orders: Information: ----Json finish send 100000 orders=("31.7524mb","31.6635mb"),total cost time:"00:17:30.1134310",max cost time:"00:17:25.9379140",max transport time:"00:16:56.4475245",mb/second:"0.1000",now:"00:26:35.8119416"----
            //----TaskChannelProducerConsumer----
            //    SocketSender:100000-Orders: Information: ----Json finish send 100000 orders=("31.7514mb","31.6614mb"),max cost time:"00:17:25.9116601",max transport time:"00:16:55.0081400",mb/second:"0.1000",now:"21:55:26.5880544"----
            //    SocketSender:100000-Orders: Information: ----Json finish send 100000 orders=("31.7520mb","31.6632mb"),max cost time:"00:17:31.6423932",max transport time:"00:17:00.3580684",mb/second:"0.1000",now:"22:22:28.4972157"----
            //await SocketSender.Instance.SendBatchJsonMessagesDataflowDuplexPipeLoopTasksAsync(taskCount:1000,  count: 1, orderCount: 100);
            #endregion
            #endregion

            #region Json.Pipe
            #region 100000=(5000*20)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    SocketSender:100000-Orders: Information: ----Json finish send 100000 orders=("31.0307mb","31.0307mb"),total cost time:"00:00:55.6011392",max cost time:"00:00:55.2366808",max transport time:"00:00:27.9721107",mb/second:"3.3197",now:"10:50:09.2020822"----
            //    SocketSender:100000-Orders: Information: ----Json finish send 100000 orders=("31.0976mb","31.0976mb"),total cost time:"00:01:20.1409032",max cost time:"00:01:18.7904744",max transport time:"00:00:44.2838141",mb/second:"1.7621",now:"13:14:31.3865308"----
            //    SocketSender:100000-Orders: Information: ----Json finish send 100000 orders=("31.1566mb","31.1566mb"),total cost time:"00:01:14.2917006",max cost time:"00:01:13.5969291",max transport time:"00:00:36.3226429",mb/second:"2.2967",now:"13:22:10.9055404"----
            //----TaskChannelProducerConsumer----
            //
            // await SocketSender.Instance.SendBatchJsonBytesPipeLoopTasksAsync(taskCount: 5000, count: 1, orderCount: 20);
            #endregion

            #region 5000000=(5000*100)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    SocketSender:500000-Orders: Information: ----Json finish send 500000 orders=("156.5200mb","156.5200mb"),total cost time:"00:03:39.1116815",max cost time:"00:03:38.5440894",max transport time:"00:03:07.6621920",mb/second:"1.9904",now:"14:57:52.8535006"----
            //----TaskChannelProducerConsumer----
            //
            // await SocketSender.Instance.SendBatchJsonBytesPipeLoopTasksAsync(taskCount: 5000, count: 1, orderCount: 100);
            #endregion

            #region 10000000=(5000*200)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    SocketSender:1000000-Orders: Information: ----Json finish send 1000000 orders=("313.0145mb","313.0145mb"),max cost time:"00:03:20.0033728",max transport time:"00:02:44.2027881",mb/second:"4.0006"----
            //----TaskChannelProducerConsumer----
            //
            //await SocketSender.Instance.SendBatchJsonBytesPipeLoopTasksAsync(taskCount: 5000, count: 1, orderCount: 200);
            #endregion

            #region 500000=(10000*50)+vs
            //----DataflowProducerConsumer----
            //    SocketSender:500000-Orders: Information: ----Json finish send 500000 orders=("156.0634mb","156.0634mb"),total cost time:"00:02:30.4618760",max cost time:"00:02:28.8425035",max transport time:"00:01:41.7095691",mb/second:"3.6964",now:"13:07:52.6619943"----
            //----PipeProducerConsumer----
            //    SocketSender:500000-Orders: Information: ----Json finish send 500000 orders=("156.1103mb","156.1103mb"),total cost time:"00:02:44.3239510",max cost time:"00:02:43.7166556",max transport time:"00:01:46.5278263",mb/second:"3.5194",now:"11:01:08.3531577"---
            //    SocketSender:500000-Orders: Information: ----Json finish send 500000 orders=("156.1698mb","156.1698mb"),total cost time:"00:04:25.3826911",max cost time:"00:04:24.3385735",max transport time:"00:03:23.4653217",mb/second:"2.0000",now:"15:47:17.0036372"----
            //----PipeProducerConsumerEx---- 
            //----TaskChannelProducerConsumer----
            //    SocketSender:500000-Orders: Information: ----Json finish send 500000 orders=("156.0877mb","156.0877mb"),total cost time:"00:03:00.9960926",max cost time:"00:03:00.0087459",max transport time:"00:01:55.6946288",mb/second:"3.1195",now:"12:07:24.4109703"----
            // await SocketSender.Instance.SendBatchJsonBytesPipeLoopTasksAsync(taskCount: 10000, count: 1, orderCount: 50);
            #endregion

            #region 10000000=(10000*100)+vs
            //----DataflowProducerConsumer----
            //    SocketSender:1000000-Orders: Information: ----Json finish send 1000000 orders=("313.0596mb","313.0596mb"),total cost time:"00:06:44.1663761",max cost time:"00:06:42.7765172",max transport time:"00:05:55.5180297",mb/second:"2.0000",now:"00:57:36.5013086"----
            //----PipeProducerConsumer----
            //    SocketSender:1000000-Orders: Information: ----Json finish send 1000000 orders=("313.0508mb","313.0508mb"),total cost time:"00:04:00.9713350",max cost time:"00:03:59.7790686",max transport time:"00:03:05.7386534",mb/second:"3.9565",now:"01:50:23.1179963"----
            //----TaskChannelProducerConsumer----
            //
            // await SocketSender.Instance.SendBatchJsonBytesPipeLoopTasksAsync(taskCount: 10000, count: 1, orderCount: 100);
            #endregion
            #endregion

            #region Json.DuplexPipe.Byte
            #region  100000=(5000*20)+vs
            //----DataflowProducerConsumer----
            //    SocketSender:100000-Orders: Information: ----Json finish send 100000 orders=("31.0937mb","31.0784mb"),total cost time:"00:01:15.4661226",max cost time:"00:01:15.0545990",max transport time:"00:00:39.8198866",mb/second:"2.1352"----
            //----PipeProducerConsumer----
            //   
            //----TaskChannelProducerConsumer----
            //
            // await SocketSender.Instance.SendBatchJsonBytesDuplexPipeLoopTasksAsync(taskCount: 5000, count: 1, orderCount: 20);
            #endregion

            #region 5000000=(5000*100)+vs
            //----DataflowProducerConsumer----
            //    SocketSender:500000-Orders: Information: ----Json finish send 500000 orders=("156.5345mb","156.0355mb"),total cost time:"00:03:06.6135234",max cost time:"00:03:05.9255288",max transport time:"00:02:33.8278772",mb/second:"2.0772"----
            //----PipeProducerConsumer----
            //    
            //----TaskChannelProducerConsumer----
            //
            await SocketSender.Instance.SendBatchJsonBytesDuplexPipeLoopTasksAsync(taskCount: 5000, count: 1, orderCount: 100);
            #endregion

            #region 10000000=(5000*200)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    
            //----TaskChannelProducerConsumer----
            //
            //await SocketSender.Instance.SendBatchJsonBytesDuplexPipeLoopTasksAsync(taskCount: 5000, count: 1, orderCount: 200);
            #endregion

            #region 500000=(10000*50)+vs
            //----DataflowProducerConsumer----
            //  
            //----PipeProducerConsumer----
            //    SocketSender:500000-Orders: Information: ----Json finish send 500000 orders=("156.0855mb","156.0584mb"),total cost time:"00:04:11.8083333",max cost time:"00:04:11.3327506",max transport time:"00:02:51.1340901",mb/second:"2.0000",now:"15:10:43.8429652"----
            //----PipeProducerConsumerEx---- 
            //----TaskChannelProducerConsumer----
            //  
            //await SocketSender.Instance.SendBatchJsonBytesDuplexPipeLoopTasksAsync(taskCount: 10000, count: 1, orderCount: 50);
            #endregion

            #region 10000000=(10000*100)+vs
            //----DataflowProducerConsumer----
            //    SocketSender:1000000-Orders: Information: ----Json finish send 1000000 orders=("313.0247mb","312.0269mb"),total cost time:"00:06:57.2099403",max cost time:"00:06:54.5716039",max transport time:"00:06:54.5622947",mb/second:"2.0000",now:"01:25:28.2650006"----
            //----PipeProducerConsumer----
            //    SocketSender:1000000-Orders: Information: ----Json finish send 1000000 orders=("313.0268mb","312.0284mb"),total cost time:"00:06:14.8496355",max cost time:"00:06:14.3765285",max transport time:"00:05:15.2467263",mb/second:"2.0000",now:"01:35:24.9436252"----  
            //----TaskChannelProducerConsumer----
            //
            //await SocketSender.Instance.SendBatchJsonBytesDuplexPipeLoopTasksAsync(taskCount: 10000, count: 1, orderCount: 100);
            #endregion

            #region 5000000=(10000*500)+exe
            //----DataflowProducerConsumer----
            //   
            //----PipeProducerConsumer----
            //    SocketSender:5000000-Orders: Information: ----Json finish send 5000000 orders=("1.5303gb","1.5257gb"),total cost time:"00:06:10.4008287",max cost time:"00:06:04.4866802",max transport time:"00:05:44.1343327",mb/second:"9.0000",now:"01:53:47.0466277"----  
            //----TaskChannelProducerConsumer----
            //
            //await SocketSender.Instance.SendBatchJsonBytesDuplexPipeLoopTasksAsync(taskCount: 10000, count: 1, orderCount: 500);
            #endregion

            #region 6000000=(12000*500)+exe
            //----DataflowProducerConsumer----
            //   
            //----PipeProducerConsumer----
            //    SocketSender:6000000-Orders: Information:----Json finish send 6000000 orders=("1.8363gb","1.8308gb"),total cost time:"00:16:07.6463817",max cost time:"00:16:04.7565293",max transport time:"00:15:34.4191982",mb/second:"3.6000",now:"01:22:28.8009368"----
            //----TaskChannelProducerConsumer----
            //
            // await SocketSender.Instance.SendBatchJsonBytesDuplexPipeLoopTasksAsync(taskCount: 12000, count: 1, orderCount: 500);
            #endregion
            #endregion

            #region Json.DuplexPipe.Generic
            #region  100000=(5000*20)+vs
            //----DataflowProducerConsumer----
            //    SocketSender:100000-Orders: Information: ----Json finish send 100000 orders=("31.0937mb","31.0784mb"),total cost time:"00:01:15.4661226",max cost time:"00:01:15.0545990",max transport time:"00:00:39.8198866",mb/second:"2.1352"----
            //----PipeProducerConsumer----
            //    
            //----TaskChannelProducerConsumer----
            //
            await SocketSender.Instance.SendBatchJsonStringsDuplexPipeLoopTasksAsync(taskCount: 5000, count: 1, orderCount: 20);
            #endregion

            #region 5000000=(5000*100)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    SocketSender:500000-Orders: Information: ----Json finish send 500000 orders=("156.5359mb","156.0375mb"),total cost time:"00:03:29.7155899",max cost time:"00:03:28.5287643",max transport time:"00:02:50.2587675",mb/second:"2.0000",now:"15:18:54.5356848"----
            //----TaskChannelProducerConsumer----
            await SocketSender.Instance.SendBatchJsonStringsDuplexPipeLoopTasksAsync(taskCount: 5000, count: 1, orderCount: 100);
            #endregion

            #region 10000000=(5000*200)+vs
            //----DataflowProducerConsumer----
            //----PipeProducerConsumer----
            //    
            //----TaskChannelProducerConsumer----
            //
            //await SocketSender.Instance.Instance.SendBatchJsonStringsDuplexPipeLoopTasksAsync(taskCount: 5000, count: 1, orderCount: 200);
            #endregion

            #region 10000000=(10000*100)+vs
            //----DataflowProducerConsumer----
            //    SocketSender:1000000-Orders: Information: ----Json finish send 1000000 orders=("313.0247mb","312.0269mb"),total cost time:"00:06:57.2099403",max cost time:"00:06:54.5716039",max transport time:"00:06:54.5622947",mb/second:"2.0000",now:"01:25:28.2650006"----
            //----PipeProducerConsumer----
            //    SocketSender:1000000-Orders: Information: ----Json finish send 1000000 orders=("313.0268mb","312.0284mb"),total cost time:"00:06:14.8496355",max cost time:"00:06:14.3765285",max transport time:"00:05:15.2467263",mb/second:"2.0000",now:"01:35:24.9436252"----  
            //----TaskChannelProducerConsumer----
            //
            //await SocketSender.Instance.Instance.SendBatchJsonStringsDuplexPipeLoopTasksAsync(taskCount: 10000, count: 1, orderCount: 100);
            #endregion

            #region 5000000=(10000*500)+exe
            //----DataflowProducerConsumer----
            //   
            //----PipeProducerConsumer----
            //    SocketSender:5000000-Orders: Information: ----Json finish send 5000000 orders=("1.5303gb","1.5257gb"),total cost time:"00:06:10.4008287",max cost time:"00:06:04.4866802",max transport time:"00:05:44.1343327",mb/second:"9.0000",now:"01:53:47.0466277"----  
            //----TaskChannelProducerConsumer----
            //
            //await SocketSender.Instance.Instance.SendBatchJsonStringsDuplexPipeLoopTasksAsync(taskCount: 10000, count: 1, orderCount: 500);
            #endregion

            #region 6000000=(12000*500)+exe
            //----DataflowProducerConsumer----
            //   
            //----PipeProducerConsumer----
            //    SocketSender:6000000-Orders: Information:----Json finish send 6000000 orders=("1.8363gb","1.8308gb"),total cost time:"00:16:07.6463817",max cost time:"00:16:04.7565293",max transport time:"00:15:34.4191982",mb/second:"3.6000",now:"01:22:28.8009368"----
            //----TaskChannelProducerConsumer----
            //
            // await SocketSender.Instance.Instance.SendBatchJsonStringsDuplexPipeLoopTasksAsync(taskCount: 12000, count: 1, orderCount: 500);
            #endregion
            #endregion

            #endregion

            Console.ReadLine();

            //Console.WriteLine("Hello World!");
        }
    }
}
