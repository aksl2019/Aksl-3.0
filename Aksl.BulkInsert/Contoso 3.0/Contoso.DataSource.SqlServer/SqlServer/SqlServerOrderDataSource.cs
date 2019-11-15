using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using AutoMapper;

using Aksl.Data;

using Contoso.Domain.Repository;
using Contoso.DataSource.Dtos;
using Contoso.Domain.Models;

namespace Contoso.DataSource.SqlServer
{
    public class SqlServerOrderDataSource : ISqlServerOrderDataSource
    {
        #region Members
        private readonly IMapper _mapper;
        private readonly ISqlOrderRepository _sqlOrderRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        #endregion

        #region Constructors
        public SqlServerOrderDataSource(IMapper mapper, ISqlOrderRepository sqlOrderRepository, ILoggerFactory loggerFactory)
        {
            _sqlOrderRepository = sqlOrderRepository ?? throw new ArgumentNullException(nameof(sqlOrderRepository));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

            _logger = _loggerFactory.CreateLogger(nameof(SqlServerOrderDataSource));
            _logger.LogInformation($"{nameof(SqlServerOrderDataSource)} 's Constructor");
            //_logger.LogInformation($"{this.GetType().FullName}'s Constructor");
        }
        #endregion

        #region Read Methods
        public async Task<IPagedList<OrderDto>> GetPagedOrdersAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                var pagedOrders = await _sqlOrderRepository.GetPagedOrdersAsync(pageIndex, pageSize);
                var pagedOrderDtos = await _mapper.Map<IEnumerable<OrderDto>>(pagedOrders.AsEnumerable()).AddPagedAsync(pageIndex, pageSize);
                return pagedOrderDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetPagedOrdersAsync)} Error: {ex.Message}");

                throw ex;
            }
        }
        #endregion

        #region Add Method
        public async Task<IEnumerable<OrderDto>> DataflowBulkInsertOrdersAsync(IEnumerable<OrderDto> orderDtos)
        {
            try
            {
                var orders = _mapper.Map<IEnumerable<Order>>(orderDtos);
                var newOrders = await _sqlOrderRepository.DataflowBulkInsertOrdersAsync(orders);

                var newOrderDtos = _mapper.Map<IEnumerable<OrderDto>>(newOrders);
                return newOrderDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DataflowBulkInsertOrdersAsync)} Error: {ex.Message}");
                throw ex;
            }
        }

        public async Task<IEnumerable<OrderDto>> DataflowPipeBulkInsertOrdersAsync(IEnumerable<OrderDto> orderDtos)
        {
            try
            {
                var orders = _mapper.Map<IEnumerable<Order>>(orderDtos);
                var newOrders = await _sqlOrderRepository.DataflowPipeBulkInsertOrdersAsync(orders);

                var newOrderDtos = _mapper.Map<IEnumerable<OrderDto>>(newOrders);
                return newOrderDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DataflowBulkInsertOrdersAsync)} Error: {ex.Message}");
                throw ex;
            }
        }

        public  async Task<IEnumerable<OrderDto>> PipeBulkInsertOrdersAsync(IEnumerable<OrderDto> orderDtos)
        {
            try
            {
                var orders = _mapper.Map<IEnumerable<Order>>(orderDtos);
                var newOrders = await _sqlOrderRepository.PipeBulkInsertOrdersAsync(orders);

                var newOrderDtos = _mapper.Map<IEnumerable<OrderDto>>(newOrders);
                return newOrderDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DataflowBulkInsertOrdersAsync)} Error: {ex.Message}");
                throw ex;
            }
        }
        #endregion
    }
}
