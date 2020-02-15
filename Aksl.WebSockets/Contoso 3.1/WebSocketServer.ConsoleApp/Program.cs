using System;
using System.Threading.Tasks;

namespace WebSocketServer.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await WebSocketListener.Instance.StartAsync();

            #region SubTasks

            #region Json.Pipe
            //DataflowProducerConsumer
            //  DataflowBulkInserter-19115-Orders: Information: ----dataflow bulk insert 20000 total orders=("6.2584mb","6.7924mb"),total cost time:"00:01:03.5106852",mb/second:"0.2926",now:"21:20:44.5138098"----
            //PipeProducerConsumer
            //  DataflowBulkInserter-8855-Orders: Information: ---- dataflow bulk insert 30000 total orders=("9.4730mb","10.3313mb"),total cost time:"00:01:21.7417945",mb/second:"0.4674",now:"16:43:13.2783511"----
            //  DataflowBulkInserter-28770-Orders: Information: ----dataflow bulk insert 30000 total orders=("9.4657mb","10.3241mb"),total cost time:"00:01:18.7317296",mb/second:"0.3737",now:"17:07:20.6664817"----
            //  DataflowBulkInserter-23670-Orders: Information: ----dataflow bulk insert 32000 total orders=("10.0510mb","10.9665mb"),total cost time:"00:01:32.6468925",mb/second:"0.4401",now:"16:53:57.7690601"----
            #endregion

            #region Json.DuplexPipe.Byte
            //PipeProducerConsumer
            //  DataflowBulkInserter-3350-Orders: Information: ---- dataflow bulk insert 20000 total orders=("6.2923mb","6.8645mb"), total cost time:"00:00:52.8677240",mb/second:"0.8639",now:"15:50:23.2773842"----
            //  DataflowBulkInserter-4280-Orders: Information: ---- dataflow bulk insert 30000 total orders=("9.3795mb","10.2378mb"),total cost time:"00:01:17.0403734",mb/second:"0.7428",now:"16:09:41.1664391"----
            //  DataflowBulkInserter-15085-Orders: Information: ----dataflow bulk insert 32000 total orders=("10.0493mb","10.9649mb"),total cost time:"00:01:36.5596875",mb/second:"0.4370",now:"16:32:38.8620010"----

            //----500000=(2000 * 25)* 10----
            //DataflowProducerConsumer
            //  DataflowBulkInserter-15085-Orders: Information: ----dataflow bulk insert 500000 total orders=("155.5270mb","169.8323mb"),total cost time:"00:23:09.3062452",mb/second:"4.5487",now:"23:00:32.9989157"----

            //----600000=(2000 * 30)* 10----
            //DataflowProducerConsumer
            //  DataflowBulkInserter-15085-Orders: Information:  ----dataflow bulk insert 600000 total orders=("186.7802mb","203.9463mb"),total cost time:"00:28:45.2592271",mb/second:"4.2267",now:"00:21:07.2591882"----
            #endregion

            #region Json.DuplexPipe.Generic
            #endregion

            #endregion

            #region 50万

            #region Json.DataflowPipe
            //50万
            //DataflowProducerConsumer
            // DataflowBulkInserter-99706-Orders: Information: ----dataflow bulk insert 500000 total orders=("160.1168mb","173.4685mb"),total cost time:"00:26:56.4826291",mb/second:"0.6107",now:"15:46:57.7016442"----
            #endregion

            #region Json.DataflowDuplexPipe
            //50万
            //DataflowProducerConsumer
            // DataflowBulkInserter-132768-Orders: Information: ----dataflow bulk insert 500000 total orders=("160.1166mb","173.4681mb"),total cost time:"00:25:02.6069744",mb/second:"0.6528",now:"17:37:10.0031434"----
            #endregion

            #region Json.Pipe
            //--DataflowProducerConsumer--
            #endregion

            #region Json.DuplexPipe.Byte
            //--DataflowProducerConsumer--
            //PipeProducerConsumer
            //  DataflowBulkInserter-470492-Orders: Information: ----dataflow bulk insert 500000 total orders=("160.1174mb","174.4222mb"),total cost time:"00:24:22.1055650",mb/second:"0.4603",now:"10:06:58.0207269"----
            //  DataflowBulkInserter-20354-Orders: Information:  ----dataflow bulk insert 500000 total orders=("160.1173mb","174.4224mb"),total cost time:"00:24:25.6830768",mb/second:"0.6122",now:"15:36:40.6212452"----
            #endregion

            #region Json.DuplexPipe.String
            //--DataflowProducerConsumer--
            #endregion

            #endregion

            Console.ReadLine();
        }
    }
}