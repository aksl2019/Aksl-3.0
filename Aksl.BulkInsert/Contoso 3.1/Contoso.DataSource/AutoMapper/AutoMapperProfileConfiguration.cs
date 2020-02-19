using System;

using AutoMapper;

using Contoso.Domain.Models;
using Contoso.DataSource.Dtos;

//https://www.cnblogs.com/jiguixin/archive/2011/09/19/2181521.html
//https://github.com/AutoMapper

namespace Contoso.DataSource.AutoMapper
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            //SaleOrder => SaleOrderDto
            CreateMap<SaleOrder, SaleOrderDto>()
                .ForMember(dto => dto.CustomerId, (map) => map.MapFrom(m => m.CustomerName))
                .ForMember(dto => dto.Status, (map) => map.MapFrom(m =>(Contoso.DataSource.Dtos.OrderStatus)Enum.Parse( typeof(Contoso.DataSource.Dtos.OrderStatus), m.Status.ToString())))
                .ForMember(dto => dto.RowVersion, (map) => map.MapFrom(m => m.RowVersion))
                .ForMember(dto => dto.OrderLineItems, (map) => map.MapFrom(m => m.OrderItems))
                .ForMember(dto => dto.TotalCostExcludeTax, m => m.Ignore())
                .ForMember(dto => dto.TotalCostTax, m => m.Ignore())
                .ForMember(dto => dto.TotalCostIncludeTax, m => m.Ignore());

            //SaleOrderItem => SaleOrderLineItemDto
            CreateMap<SaleOrderItem, SaleOrderLineItemDto>()
               .ForMember(dto => dto.UnitPriceIncludeTax, m => m.Ignore())
               .ForMember(dto => dto.TotalCostExcludeTax, m => m.Ignore())
               .ForMember(dto => dto.TotalCostTax, m => m.Ignore())
               .ForMember(dto => dto.TotalCostIncludeTax, m => m.Ignore());

            //SaleOrderDto => SaleOrder
            CreateMap<SaleOrderDto, SaleOrder>()
                .ForMember(m => m.CustomerName, (map) => map.MapFrom(dto => dto.CustomerId))
                .ForMember(m => m.Status, (map) => map.MapFrom(dto => (Contoso.Domain.Models.OrderStatus)Enum.Parse(typeof(Contoso.Domain.Models.OrderStatus), dto.Status.ToString())))
                .ForMember(m => m.RowVersion, (map) => map.MapFrom(dto => dto.RowVersion))
                .ForMember(m => m.OrderItems, (map) => map.MapFrom(dto => dto.OrderLineItems))
                .ForMember(m => m.TotalCostExcludeTax, dto => dto.Ignore())
                .ForMember(m => m.TotalCostTax, dto => dto.Ignore())
                .ForMember(m => m.TotalCostIncludeTax, dto => dto.Ignore());

            //SaleOrderLineItemDto => SaleOrderItem
            CreateMap<SaleOrderLineItemDto, SaleOrderItem>()
                .ForMember(m => m.UnitPriceIncludeTax, dto => dto.Ignore())
                .ForMember(m => m.TotalCostExcludeTax, dto => dto.Ignore())
                .ForMember(m => m.TotalCostTax, dto => dto.Ignore())
                .ForMember(m => m.TotalCostIncludeTax, dto => dto.Ignore());
        }
    }
}
