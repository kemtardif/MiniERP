﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MiniERP.InventoryService.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiniERP.InventoryService.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230423141919_viewrightjoin")]
    partial class viewrightjoin
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MiniERP.InventoryService.Models.AvailableInventoryView", b =>
                {
                    b.Property<int>("ArticleId")
                        .HasColumnType("integer");

                    b.Property<double>("Quantity")
                        .HasColumnType("double precision");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("ArticleId");

                    b.ToTable((string)null);

                    b.ToView("AvailableInventoryView", (string)null);
                });

            modelBuilder.Entity("MiniERP.InventoryService.Models.InventoryItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ArticleId")
                        .HasColumnType("integer");

                    b.Property<bool>("AutoOrder")
                        .HasColumnType("boolean");

                    b.Property<double>("AutoQuantity")
                        .HasColumnType("double precision");

                    b.Property<double>("AutoTreshold")
                        .HasColumnType("double precision");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId")
                        .IsUnique();

                    b.ToTable("InventoryItems");
                });

            modelBuilder.Entity("MiniERP.InventoryService.Models.InventoryMovement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ExpectedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("InventoryId")
                        .HasColumnType("integer");

                    b.Property<int>("MovementStatus")
                        .HasColumnType("integer");

                    b.Property<int>("MovementType")
                        .HasColumnType("integer");

                    b.Property<double>("Quantity")
                        .HasColumnType("double precision");

                    b.Property<int>("RelatedOrderId")
                        .HasColumnType("integer");

                    b.Property<int>("RelatedOrderType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("InventoryId");

                    b.ToTable("StockMovements");
                });

            modelBuilder.Entity("MiniERP.InventoryService.Models.PendingInventoryView", b =>
                {
                    b.Property<int>("ArticleId")
                        .HasColumnType("integer");

                    b.Property<double>("Quantity")
                        .HasColumnType("double precision");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("ArticleId");

                    b.ToTable((string)null);

                    b.ToView("PendingInventoryView", (string)null);
                });

            modelBuilder.Entity("MiniERP.InventoryService.Models.InventoryMovement", b =>
                {
                    b.HasOne("MiniERP.InventoryService.Models.InventoryItem", "InventoryItem")
                        .WithMany()
                        .HasForeignKey("InventoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InventoryItem");
                });
#pragma warning restore 612, 618
        }
    }
}
