using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System.Linq.Dynamic;

using Aksl.BulkInsert;
using Aksl.Data;

using Contoso.Domain.Repository;
using Contoso.Domain.Models;
using Contoso.Infrastructure.Data.Context;

namespace Contoso.Infrastructure.Data.Repository
{
    public class SqlOrderRepository : EfRepository<SaleOrder>, ISqlOrderRepository
    {
        #region Members
        private readonly IServiceProvider _serviceProvider;
        private readonly ContosoContext _contosoContext;
        private readonly IDbContextFactory<ContosoContext> _dbContextFactory;

        private readonly IDataflowBulkInserter<SaleOrder, SaleOrder> _dataflowBulkInserter;
        private readonly IDataflowPipeBulkInserter<SaleOrder, SaleOrder> _dataflowPipeBulkInserter;
        private readonly IPipeBulkInserter<SaleOrder, SaleOrder> _pipeBulkInserter;
        private readonly IDataflowNoResultHandler<SaleOrder> _dataflowBulkDeleter;
        private readonly IDataflowNoResultHandler<SaleOrder> _dataflowBulkUpdater;

        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        #endregion

        #region Constructors
        public SqlOrderRepository(IServiceProvider serviceProvider, IDbContextFactory<ContosoContext> dbContextFactory, ILoggerFactory loggerFactory) : base(dbContextFactory.CreateDbContext())
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _contosoContext = this._context as ContosoContext ?? throw new ArgumentNullException(nameof(ContosoContext));
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

            // _bulkInserter = new DataflowBulkInserter<Models.PurchaseOrder, Models.PurchaseOrder>(InsertOrdersAsync, _loggerFactory);
            // _pipeBulkInserter=new PipeBulkInserter<Models.PurchaseOrder, Models.PurchaseOrder>(InsertOrdersAsync, pipeSettings: PipeSettings.Default,blockSettings: BlockSettings.Default, loggerFactory: _loggerFactory);
            _dataflowBulkInserter = _serviceProvider.GetRequiredService<IDataflowBulkInserter<SaleOrder, SaleOrder>>();
            _dataflowBulkInserter.InsertHandler = (async (orders) =>
            {
                 // await _contosoContext.ExecuteSqlBulkCopyAync<SaleOrder>(orders.ToList());
                 //return await this.BulkInsertAsync(orders);
                return await this.InsertAsync(orders);
            });

            _dataflowPipeBulkInserter = _serviceProvider.GetRequiredService<IDataflowPipeBulkInserter<SaleOrder, SaleOrder>>();
            _dataflowPipeBulkInserter.InsertHandler = (async (orders) =>
            {
                // return await this.BulkInsertAsync(orders);
                return await this.InsertAsync(orders);
            });

            // _pipeBulkInserter = new PipeBulkInserter<Order, Order>(InsertOrdersAsync, PipeSettings.Default, BlockSettings.Default, _loggerFactory);
            //_pipeBulkInserter = new PipeBulkInserter<Order, Order>(InsertOrdersAsync);
            _pipeBulkInserter = _serviceProvider.GetRequiredService<IPipeBulkInserter<SaleOrder, SaleOrder>>();
            _pipeBulkInserter.InsertHandler = (async (orders) =>
            {
                // return await this.BulkInsertAsync(orders);
                return await this.InsertAsync(orders);
            });

            _dataflowBulkDeleter = _serviceProvider.GetRequiredService<IDataflowNoResultHandler<SaleOrder>>();
            _dataflowBulkDeleter.Handle = (async (saleOrders) =>
            {
                var saleOrderIds = saleOrders.Select(o => o.Id).ToArray();
                var deleteSaleOrder = await (from so in _contosoContext.SaleOrders
                                             where saleOrderIds.Contains(so.Id)
                                             select so).ToListAsync();

                //  var deleteSaleOrder = _contosoContext.SaleOrders.WhereIn<SaleOrder, int>(so => so.Id, saleOrderIds).ToList();
                await this.DeleteAsync(deleteSaleOrder);
            });

            _dataflowBulkUpdater = _serviceProvider.GetRequiredService<IDataflowNoResultHandler<SaleOrder>>();
            _dataflowBulkUpdater.Handle = (async (saleOrders) =>
            {
                var saleOrderIds = saleOrders.Select(o => o.Id).ToArray();
                var dbSaleOrders = await (from so in _contosoContext.SaleOrders
                                          where saleOrderIds.Contains(so.Id)
                                          select so).ToListAsync();

                foreach (var dbSaleOrder in dbSaleOrders)
                {
                    var updateSaleOrder = saleOrders.FirstOrDefault(o => o.Id == dbSaleOrder.Id);
                    dbSaleOrder.Status = updateSaleOrder.Status;
                    dbSaleOrder.RowVersion = updateSaleOrder.RowVersion;
                    //var dbSaleOrder = await _contosoContext.SaleOrders.FirstOrDefaultAsync(o => o.Id == saleOrder.Id);
                    //await EntityHelper.TryUpdateEntryAsync<SaleOrder>(_contosoContext, dbSaleOrder, so => so.Status, so => so.RowVersion);
                }

                //  var deleteSaleOrder = _contosoContext.SaleOrders.WhereIn<SaleOrder, int>(so => so.Id, saleOrderIds).ToList();
                await this.UpdateAsync(dbSaleOrders);
            });

            _logger = loggerFactory.CreateLogger(nameof(SqlOrderRepository));
            _logger.LogInformation($"{nameof(SqlOrderRepository)}'s Constructor");
        }
        #endregion

        public async ValueTask<(IQueryable<SaleOrder> PagedQuery, int TotalCount)> GetPagedSaleOrdersAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var pagedOrders = await _contosoContext.SaleOrders.Include(po => po.OrderItems)
                                                              .PageByAsync(pageIndex, pageSize);

            return pagedOrders;
        }

        public async ValueTask<IEnumerable<SaleOrder>> DataflowBulkInsertSaleOrdersAsync(IEnumerable<SaleOrder> saleOrders)
        {
            Stopwatch sw = Stopwatch.StartNew();
            _logger.LogInformation($"----begin dataflow bulk insert { saleOrders.Count()} orders,ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}----");

            //    EntityFrameworkManager.ContextFactory = context => _purchaseOrderContext;
            //   await _purchaseOrderContext.BulkInsertAsync(purchaseOrders);
            // return purchaseOrders;

            var newOrders = await _dataflowBulkInserter.BulkInsertAsync(saleOrders);

            _logger.LogInformation($"----finish dataflow bulk insert {saleOrders.Count()} orders,cost time:{sw.Elapsed},ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}\"----");

            return newOrders;
        }

        public async ValueTask<IEnumerable<SaleOrder>> DataflowPipeBulkInsertSaleOrdersAsync(IEnumerable<SaleOrder> saleOrders)
        {
            Stopwatch sw = Stopwatch.StartNew();
            _logger.LogInformation($"----begin dataflow pipe bulk insert {saleOrders.Count()} orders,ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}----");

            //    EntityFrameworkManager.ContextFactory = context => _purchaseOrderContext;
            //   await _purchaseOrderContext.BulkInsertAsync(purchaseOrders);
            // return purchaseOrders;

            var newOrders = await _dataflowPipeBulkInserter.BulkInsertAsync(saleOrders);

            _logger.LogInformation($"----finish dataflow pipe bulk insert {saleOrders.Count()} orders,cost time:{sw.Elapsed},ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}\"----");

            return newOrders;
        }

        public async ValueTask<IEnumerable<SaleOrder>> PipeBulkInsertSaleOrdersAsync(IEnumerable<SaleOrder> saleOrders)
        {
            Stopwatch sw = Stopwatch.StartNew();
            _logger.LogInformation($"----begin bulk insert { saleOrders.Count()} orders,ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}----");

            //    EntityFrameworkManager.ContextFactory = context => _purchaseOrderContext;
            //   await _purchaseOrderContext.BulkInsertAsync(purchaseOrders);
            // return purchaseOrders;

            var newOrders = await _pipeBulkInserter.BulkInsertAsync(saleOrders);

            _logger.LogInformation($"----finish bulk insert {saleOrders.Count()} orders,cost time:{sw.Elapsed},ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}\"----");

            return newOrders;
        }

        public async ValueTask DeleteSaleOrdersAsync(IEnumerable<SaleOrder> saleOrders)
        {
            Stopwatch sw = Stopwatch.StartNew();
            _logger.LogInformation($"----begin bulk delete { saleOrders.Count()} sale orders,ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}----");

            await _dataflowBulkDeleter.HandleAsync(saleOrders);

            _logger.LogInformation($"----finish bulk delete {saleOrders.Count()} sale orders,cost time:{sw.Elapsed},ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}\"----");
        }

        public async ValueTask UpdateSaleOrdersAsync(IEnumerable<SaleOrder> saleOrders)
        {
            Stopwatch sw = Stopwatch.StartNew();
            _logger.LogInformation($"----begin bulk update { saleOrders.Count()} sale orders,ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}----");

            await _dataflowBulkUpdater.HandleAsync(saleOrders);

            _logger.LogInformation($"----finish bulk update {saleOrders.Count()} sale orders,cost time:{sw.Elapsed},ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}\"----");
        }
    }
}
