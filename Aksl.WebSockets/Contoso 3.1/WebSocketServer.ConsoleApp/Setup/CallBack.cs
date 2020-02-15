using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Aksl.Concurrency;
using Aksl.BulkInsert;

using Contoso.DataSource.Dtos;

namespace WebSocketServer.ConsoleApp 
{
    public partial class WebSocketListener
    {
        #region Dataflow CallBack Methods   
        public void RegisterCallBackOnDataflowBulkInsert(IDataflowBulkInserter<SaleOrderDto, SaleOrderDto> sender, ILogger logger, AsyncCountdownEvent signals = null)
        {
            sender.OnInsertCallBack = async (context) =>
            {
                if (context.Exception != null)
                {
                    logger.LogError($"exception: {context.Exception} when send {context.MessageConunt} messages");
                }
                else if (context.ExecutionTime != TimeSpan.Zero)
                {
                    using (await _mutex.LockAsync())
                    {
                        _durationManage.MaxTime = _durationManage.GetMaxTimeValue(_durationManage.MaxTime, context.ExecutionTime);
                        _durationManage.TotalTime += context.ExecutionTime;
                        _durationManage.TotalCount += context.MessageConunt;
                    }

                    logger
                        .LogInformation($"TotalCount={ _durationManage.TotalCount},ExecutionTime={context.ExecutionTime},ThreadId={Thread.CurrentThread.ManagedThreadId},OrderCount=\"{context.MessageConunt}\"");
                }

                signals?.Signal();
            };
        }
        #endregion
    }
}
