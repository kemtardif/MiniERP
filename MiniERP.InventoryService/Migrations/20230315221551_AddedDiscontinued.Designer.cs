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
    [Migration("20230315221551_AddedDiscontinued")]
    partial class AddedDiscontinued
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MiniERP.InventoryService.Models.Stock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("AutoOrder")
                        .HasColumnType("boolean");

                    b.Property<double>("AutoQuantity")
                        .HasColumnType("double precision");

                    b.Property<double>("AutoTreshold")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Discontinued")
                        .HasColumnType("boolean");

                    b.Property<double>("MaxQuantity")
                        .HasColumnType("double precision");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<double>("Quantity")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ProductId")
                        .IsUnique();

                    b.ToTable("InventoryItems");
                });
#pragma warning restore 612, 618
        }
    }
}
