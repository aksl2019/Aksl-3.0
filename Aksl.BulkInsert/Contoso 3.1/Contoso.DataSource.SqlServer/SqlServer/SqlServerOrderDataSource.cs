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
        public async IAsyncEnumerable<SaleOrderDto> GetPagedSaleOrderListAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            //try
            //{
            var pageddSaleOrderList = await _sqlOrderRepository.GetPagedSaleOrdersAsync(pageIndex, pageSize);
            var saleOrderDtos = _mapper.Map<IEnumerable<SaleOrderDto>>(pageddSaleOrderList.PagedQuery.AsEnumerable());

            var pagedSaleOrderDtos = new PagedList<SaleOrderDto>(pageIndex, pageSize, pageddSaleOrderList.TotalCount, saleOrderDtos);
            foreach (var orderDto in pagedSaleOrderDtos)
            {
                yield return orderDto;
            }

            // var pagedOrderDtos = await _mapper.Map<IEnumerable<SaleOrderDto>>(pagedSaleOrders.AsEnumerable()).AddPagedAsync(pageIndex, pageSize);
            // return pagedOrderDtos;
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"{nameof(GetPagedOrdersAsync)} Error: {ex.Message}");

            //    throw ex;
            //}
        }
        #endregion

        #region Add Methods
        public async ValueTask<IEnumerable<SaleOrderDto>> DataflowBulkInsertSaleOrdersAsync(IEnumerable<SaleOrderDto> saleOrderDtos)
        {
            try
            {
                var saleOrderModels = _mapper.Map<IEnumerable<SaleOrder>>(saleOrderDtos);
                var newSaleOrderModels = await _sqlOrderRepository.DataflowBulkInsertSaleOrdersAsync(saleOrderModels);

                var newSaleOrderDtos = _mapper.Map<IEnumerable<SaleOrderDto>>(newSaleOrderModels);
                return newSaleOrderDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DataflowBulkInsertSaleOrdersAsync)} Error: {ex.Message}");
                throw ex;
            }
        }

        public async ValueTask<IEnumerable<SaleOrderDto>> DataflowPipeBulkInsertSaleOrdersAsync(IEnumerable<SaleOrderDto> saleOrderDtos)
        {
            try
            {
                var saleOrderModels = _mapper.Map<IEnumerable<SaleOrder>>(saleOrderDtos);
                var newSaleOrderModels = await _sqlOrderRepository.DataflowPipeBulkInsertSaleOrdersAsync(saleOrderModels);

                var newSaleOrderDtos = _mapper.Map<IEnumerable<SaleOrderDto>>(newSaleOrderModels);
                return newSaleOrderDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DataflowPipeBulkInsertSaleOrdersAsync)} Error: {ex.Message}");
                throw ex;
            }
        }

        public async ValueTask<IEnumerable<SaleOrderDto>> PipeBulkInsertSaleOrdersAsync(IEnumerable<SaleOrderDto> saleOrderDtos)
        {
            try
            {
                var saleOrderModels = _mapper.Map<IEnumerable<SaleOrder>>(saleOrderDtos);
                var newSaleOrderModels = await _sqlOrderRepository.PipeBulkInsertSaleOrdersAsync(saleOrderModels);

                var newSaleOrderDtos = _mapper.Map<IEnumerable<SaleOrderDto>>(newSaleOrderModels);
                return newSaleOrderDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(PipeBulkInsertSaleOrdersAsync)} Error: {ex.Message}");
                throw ex;
            }
        }
        #endregion

        #region Delete Methods
        public async ValueTask DeleteSaleOrdersAsync(IEnumerable<SaleOrderDto> saleOrderDtos)
        {
            try
            {
                var saleOrderModels = _mapper.Map<IEnumerable<SaleOrder>>(saleOrderDtos);
                await _sqlOrderRepository.DeleteSaleOrdersAsync(saleOrderModels);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteSaleOrdersAsync)} Error: {ex.Message}");
                throw ex;
            }
        }

        public async  ValueTask UpdateSaleOrdersAsync(IEnumerable<SaleOrderDto> saleOrderDtos)
        {
            try
            {
                var saleOrderModels = _mapper.Map<IEnumerable<SaleOrder>>(saleOrderDtos);
                await _sqlOrderRepository.UpdateSaleOrdersAsync(saleOrderModels);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteSaleOrdersAsync)} Error: {ex.Message}");
                throw ex;
            }
        }
        #endregion
    }
}
