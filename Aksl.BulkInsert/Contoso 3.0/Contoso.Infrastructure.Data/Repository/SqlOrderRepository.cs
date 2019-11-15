using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Contoso.Domain.Repository;
using Contoso.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Aksl.BulkInsert;
using Aksl.Data;

using Contoso.Domain.Models;

namespace Contoso.Infrastructure.Data.Repository
{
    public class SqlOrderRepository : EfRepository<Order>, ISqlOrderRepository
    {
        #region Members
        private readonly IServiceProvider _serviceProvider;
        private readonly ContosoContext _contosoContext;
        private readonly IDbContextFactory<ContosoContext> _dbContextFactory;

        private readonly IDataflowBulkInserter<Order, Order> _dataflowBulkInserter;
        private readonly IDataflowPipeBulkInserter<Order, Order> _dataflowPipeBulkInserter;
        private readonly IPipeBulkInserter<Order, Order> _pipeBulkInserter;
        //  private readonly INoResultBulkInserter<Models.PurchaseOrder> _noResulBulkInserter;

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
            _dataflowBulkInserter = _serviceProvider.GetRequiredService<IDataflowBulkInserter<Order, Order>>();
            _dataflowBulkInserter.InsertHandler = (async (orders) =>
            {
               // return await this.BulkInsertAsync(orders);
                return await this.InsertAsync(orders);
            });

            _dataflowPipeBulkInserter = _serviceProvider.GetRequiredService<IDataflowPipeBulkInserter<Order, Order>>();
            _dataflowPipeBulkInserter.InsertHandler = (async (orders) =>
            {
                // return await this.BulkInsertAsync(orders);
                return await this.InsertAsync(orders);
            });

            // _pipeBulkInserter = new PipeBulkInserter<Order, Order>(InsertOrdersAsync, PipeSettings.Default, BlockSettings.Default, _loggerFactory);
            //_pipeBulkInserter = new PipeBulkInserter<Order, Order>(InsertOrdersAsync);
            _pipeBulkInserter = _serviceProvider.GetRequiredService<IPipeBulkInserter<Order, Order>>();
            _pipeBulkInserter.InsertHandler = (async (orders) =>
            {
                // return await this.BulkInsertAsync(orders);
                return await this.InsertAsync(orders);
            });

            //_noResulBulkInserter = new NoResultBulkInserter<Models.PurchaseOrder>(InsertOrdersAsync, _loggerFactory);

            _logger = loggerFactory.CreateLogger(nameof(SqlOrderRepository));
            _logger.LogInformation($"{nameof(SqlOrderRepository)}'s Constructor");
        }
        #endregion

        public async Task<IPagedList<Order>> GetPagedOrdersAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var pagedOrders = await _contosoContext.Orders.Include(po => po.OrderItems)
                                                          .AddPagedAsync(pageIndex, pageSize);

            return pagedOrders;
        }

        public async Task<IEnumerable<Order>> DataflowBulkInsertOrdersAsync(IEnumerable<Order> orders)
        {
            Stopwatch sw = Stopwatch.StartNew();
            _logger.LogInformation($"----begin dataflow bulk insert {  orders.Count()} orders,ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}----");

            //    EntityFrameworkManager.ContextFactory = context => _purchaseOrderContext;
            //   await _purchaseOrderContext.BulkInsertAsync(purchaseOrders);
            // return purchaseOrders;

            var newOrders = await _dataflowBulkInserter.BulkInsertAsync(orders);

            _logger.LogInformation($"----finish dataflow bulk insert {orders.Count()} orders,cost time:{sw.Elapsed},ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}\"----");

            return newOrders;
        }

        public async Task<IEnumerable<Order>> DataflowPipeBulkInsertOrdersAsync(IEnumerable<Order> orders)
        {
            Stopwatch sw = Stopwatch.StartNew();
            _logger.LogInformation($"----begin dataflow pipe bulk insert {  orders.Count()} orders,ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}----");

            //    EntityFrameworkManager.ContextFactory = context => _purchaseOrderContext;
            //   await _purchaseOrderContext.BulkInsertAsync(purchaseOrders);
            // return purchaseOrders;

            var newOrders = await _dataflowPipeBulkInserter.BulkInsertAsync(orders);

            _logger.LogInformation($"----finish dataflow pipe bulk insert {orders.Count()} orders,cost time:{sw.Elapsed},ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}\"----");

            return newOrders;
        }

        public async Task<IEnumerable<Order>> PipeBulkInsertOrdersAsync(IEnumerable<Order> orders)
        {
            Stopwatch sw = Stopwatch.StartNew();
            _logger.LogInformation($"----begin bulk insert {  orders.Count()} orders,ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}----");

            //    EntityFrameworkManager.ContextFactory = context => _purchaseOrderContext;
            //   await _purchaseOrderContext.BulkInsertAsync(purchaseOrders);
            // return purchaseOrders;

            var newOrders = await _pipeBulkInserter.BulkInsertAsync(orders);

            _logger.LogInformation($"----finish bulk insert {orders.Count()} orders,cost time:{sw.Elapsed},ThreadId={Thread.CurrentThread.ManagedThreadId},now:{DateTime.Now.TimeOfDay}\"----");

            return newOrders;
        }
    }
}
