﻿// <auto-generated />
using System;
using AirportAPI.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AirportApi.Migrations
{
    [DbContext(typeof(AirportContext))]
    partial class AirportContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("AirportAPI.Models.Flight", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Brand")
                        .HasColumnType("longtext");

                    b.Property<int?>("LegId")
                        .HasColumnType("int");

                    b.Property<string>("Number")
                        .HasColumnType("longtext");

                    b.Property<int?>("PassengersCount")
                        .HasColumnType("int");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LegId");

                    b.ToTable("Flights", (string)null);
                });

            modelBuilder.Entity("AirportAPI.Models.FlightLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("FlightId")
                        .HasColumnType("int");

                    b.Property<DateTime>("In")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("LegId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Out")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("FlightId");

                    b.HasIndex("LegId");

                    b.ToTable("FlightLogs", (string)null);
                });

            modelBuilder.Entity("AirportAPI.Models.Leg", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("Capacity")
                        .HasColumnType("int");

                    b.Property<int>("CrossingTime")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<int?>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Legs", (string)null);

                    b.HasData(
                        new
                        {
                            Id = -1,
                            Capacity = 2147483647,
                            CrossingTime = 0,
                            Name = "Wait for Landing",
                            Type = 0
                        },
                        new
                        {
                            Id = 1,
                            Capacity = 1,
                            CrossingTime = 60,
                            Name = "Point 1",
                            Type = 1
                        },
                        new
                        {
                            Id = 2,
                            Capacity = 1,
                            CrossingTime = 60,
                            Name = "Point 2",
                            Type = 1
                        },
                        new
                        {
                            Id = 3,
                            Capacity = 1,
                            CrossingTime = 60,
                            Name = "Point 3",
                            Type = 1
                        },
                        new
                        {
                            Id = 4,
                            Capacity = 1,
                            CrossingTime = 60,
                            Name = "Point 4",
                            Type = 6
                        },
                        new
                        {
                            Id = 5,
                            Capacity = 1,
                            CrossingTime = 180,
                            Name = "Point 5",
                            Type = 2
                        },
                        new
                        {
                            Id = 6,
                            Capacity = 1,
                            CrossingTime = 60,
                            Name = "Point 6",
                            Type = 2
                        },
                        new
                        {
                            Id = 7,
                            Capacity = 1,
                            CrossingTime = 60,
                            Name = "Point 7",
                            Type = 2
                        },
                        new
                        {
                            Id = 8,
                            Capacity = 1,
                            CrossingTime = 180,
                            Name = "Point 8",
                            Type = 4
                        },
                        new
                        {
                            Id = 9,
                            Capacity = 1,
                            CrossingTime = 60,
                            Name = "Point 9",
                            Type = 4
                        },
                        new
                        {
                            Id = 10,
                            Capacity = 2147483647,
                            CrossingTime = 0,
                            Name = "Departure to other Place",
                            Type = 4
                        });
                });

            modelBuilder.Entity("AirportAPI.Models.Flight", b =>
                {
                    b.HasOne("AirportAPI.Models.Leg", "CurrentLeg")
                        .WithMany("CurrectFlights")
                        .HasForeignKey("LegId");

                    b.Navigation("CurrentLeg");
                });

            modelBuilder.Entity("AirportAPI.Models.FlightLog", b =>
                {
                    b.HasOne("AirportAPI.Models.Flight", "Flight")
                        .WithMany()
                        .HasForeignKey("FlightId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AirportAPI.Models.Leg", "Leg")
                        .WithMany()
                        .HasForeignKey("LegId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Flight");

                    b.Navigation("Leg");
                });

            modelBuilder.Entity("AirportAPI.Models.Leg", b =>
                {
                    b.Navigation("CurrectFlights");
                });
#pragma warning restore 612, 618
        }
    }
}
