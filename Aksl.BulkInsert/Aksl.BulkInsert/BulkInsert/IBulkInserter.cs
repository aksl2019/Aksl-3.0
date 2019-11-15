using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aksl.BulkInsert
{
    #region Dataflow IBulkInserter
    /// <summary>
    /// Bulk Inserter Interface
    /// </summary>
    public interface IDataflowBulkInserter<TMessage, TResult>
    {
        #region Properties
        Action<BulkInsertContextContext<TResult>> OnInsertCallBack { get; set; }

        Func<IEnumerable<TMessage>, Task<IEnumerable<TResult>>> InsertHandler { get; set; }
        #endregion

        #region Bulk Insert Method
        Task<IEnumerable<TResult>> BulkInsertAsync(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default);
        #endregion
    }
    #endregion

    #region DataflowPipe IBulkInserter
    /// <summary>
    /// Bulk Inserter Interface
    /// </summary>
    public interface IDataflowPipeBulkInserter<TMessage, TResult>
    {
        #region Properties
        Action<BulkInsertContextContext<TResult>> OnInsertCallBack { get; set; }

        Func<IEnumerable<TMessage>, Task<IEnumerable<TResult>>> InsertHandler { get; set; }
        #endregion

        #region Bulk Insert Method
        Task<IEnumerable<TResult>> BulkInsertAsync(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default);
        #endregion
    }
    #endregion

    #region INoResultBulkInserter
    /// <summary>
    /// Bulk Inserter Interface
    /// </summary>
    public interface IDataflowNoResultHandler<TMessage>
    {
        #region Properties
        Func<IEnumerable<TMessage>, Task> Handle { get; set; }

        Action<BulkInsertContextContext> OnHandleCallBack { get; set; }
        #endregion

        #region Method
        ValueTask HandleAsync(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default);
        #endregion
    }
    #endregion

    #region Pipe IBulkInserter
    /// <summary>
    /// Bulk Inserter Interface
    /// </summary>
    public interface IPipeBulkInserter<TMessage, TResult> : IDisposable
    {
        #region Properties
        Func<IEnumerable<TMessage>, Task<IEnumerable<TResult>>> InsertHandler { get; set; }

        Action<BulkInsertContextContext<TResult>> OnInsertCallBack { get; set; }
        #endregion

        #region Bulk Insert Method
        Task<TResult[]> BulkInsertAsync(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default);

        // Task BulkInserterAsync<TMessage>(IEnumerable<TMessage> messages, Func<IEnumerable<TMessage>> insertAction, CancellationToken cancellationToken = default);
        #endregion
    }
    #endregion
}