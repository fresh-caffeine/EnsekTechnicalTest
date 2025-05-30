﻿// <auto-generated />
using System;
using Ensek.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Ensek.Api.Migrations
{
    [DbContext(typeof(MeterReadingsDbContext))]
    [Migration("20250528163434_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.5");

            modelBuilder.Entity("Ensek.Api.Models.Account", b =>
                {
                    b.Property<int>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AccountId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Ensek.Api.Models.MeterReading", b =>
                {
                    b.Property<int>("MeterReadingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccountId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ReadingDate")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ReadingValue")
                        .HasColumnType("TEXT");

                    b.HasKey("MeterReadingId");

                    b.HasIndex("AccountId");

                    b.ToTable("MeterReadings");
                });

            modelBuilder.Entity("Ensek.Api.Models.MeterReading", b =>
                {
                    b.HasOne("Ensek.Api.Models.Account", "Account")
                        .WithMany("MeterReadings")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Ensek.Api.Models.Account", b =>
                {
                    b.Navigation("MeterReadings");
                });
#pragma warning restore 612, 618
        }
    }
}
