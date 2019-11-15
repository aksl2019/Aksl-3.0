﻿// <auto-generated />
using System;
using Contoso.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Contoso.Api.Migrations
{
    [DbContext(typeof(ContosoContext))]
    [Migration("20191110145034_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Contoso.Domain.Models.SaleOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOnUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<int>("OrderNumber")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(16)")
                        .HasMaxLength(16);

                    b.HasKey("Id");

                    b.ToTable("SaleOrder");
                });

            modelBuilder.Entity("Contoso.Domain.Models.SaleOrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int?>("SaleOrderId")
                        .HasColumnType("int");

                    b.Property<decimal>("TaxRate")
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("UnitPriceExcludeTax")
                        .HasColumnType("money");

                    b.HasKey("Id");

                    b.HasIndex("SaleOrderId");

                    b.ToTable("SaleOrderItem");
                });

            modelBuilder.Entity("Contoso.Domain.Models.SaleOrderItem", b =>
                {
                    b.HasOne("Contoso.Domain.Models.SaleOrder", null)
                        .WithMany("OrderItems")
                        .HasForeignKey("SaleOrderId");
                });
#pragma warning restore 612, 618
        }
    }
}
