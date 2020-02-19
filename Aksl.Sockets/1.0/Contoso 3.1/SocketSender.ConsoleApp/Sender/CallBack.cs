using System;
using System.Linq;
using System.Threading;

using Microsoft.Extensions.Logging;

using Aksl.Concurrency;

namespace Socket.Sender
{
    public partial class SocketSender
    {
        #region IDataflowPipeSocketSender CallBack Methods
        public void RegisterCallBackOnSendBatchJsonMessagesDataflowPipeLoop(Aksl.Sockets.Client.IDataflowPipeSocketSender sender, ILogger logger, AsyncCountdownEvent signals = null)
        {
            sender.OnSendCallBack = async (context) =>
             {
                 if (context.Exception != null)
                 {
                     logger.LogError($"exception: {context.Exception} when send {context.MessageConunt} messages");
                 }
                 else if (context.ExecutionTime != TimeSpan.Zero)
                 {
                     using (await _mutex.LockAsync())
                     {
                         _durationManage.MaxTime = _durationManage.MaxTime.Ticks < context.ExecutionTime.Ticks ? context.ExecutionTime : _durationManage.MaxTime;
                        //_durationManage.MaxTime = _durationManage.GetMaxTimeValue(_durationManage.MaxTime, context.ExecutionTime);
                        _durationManage.TotalTime += context.ExecutionTime;
                         _durationManage.TotalCount += context.Datas.Count();
                     }

                     logger
                         .LogInformation($"TotalCount={ _durationManage.TotalCount},ExecutionTime={context.ExecutionTime},ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay},OrderCount=\"{context.Datas?.Count()}\"");
                 }

                 signals?.Signal();
             };
        }
        #endregion

        #region IDataflowDuplexPipeSocketSender CallBack Methods
        public void RegisterCallBackOnSendBatchJsonMessagesDataflowDuplexPipeLoop(Aksl.Sockets.Client.IDataflowPipeSocketSender sender, ILogger logger, AsyncCountdownEvent signals = null)
        {
            sender.OnSendCallBack = async (context) =>
            {
                if (context.Exception != null)
                {
                    logger.LogError($"exception: {context.Exception} when send {context.MessageConunt} messages");
                }
                else if (context.ExecutionTime != TimeSpan.Zero)
                {
                    using (await _mutex.LockAsync())
                    {
                        _durationManage.MaxTime = _durationManage.MaxTime.Ticks < context.ExecutionTime.Ticks ? context.ExecutionTime : _durationManage.MaxTime;
                        //_durationManage.MaxTime = _durationManage.GetMaxTimeValue(_durationManage.MaxTime, context.ExecutionTime);
                        _durationManage.TotalTime += context.ExecutionTime;
                        _durationManage.TotalCount += context.Datas.Count();
                    }

                    logger
                        .LogInformation($"TotalCount={ _durationManage.TotalCount},ExecutionTime={context.ExecutionTime},ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay},OrderCount=\"{context.Datas?.Count()}\"");
                }

                signals?.Signal();
            };
        }
        #endregion
    }
}
