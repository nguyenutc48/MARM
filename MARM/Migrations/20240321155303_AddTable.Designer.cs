﻿// <auto-generated />
using System;
using MARM.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MARM.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240321155303_AddTable")]
    partial class AddTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.12");

            modelBuilder.Entity("MARM.Data.AppConfig", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.Property<int>("Baudrate")
                        .HasColumnType("INT")
                        .HasColumnName("Baudrate");

                    b.Property<int>("Light1Mode")
                        .HasColumnType("INT")
                        .HasColumnName("Light1Mode");

                    b.Property<int>("Light2Mode")
                        .HasColumnType("INT")
                        .HasColumnName("Light2Mode");

                    b.Property<int>("Light3Mode")
                        .HasColumnType("INT")
                        .HasColumnName("Light3Mode");

                    b.Property<int>("Light4Mode")
                        .HasColumnType("INT")
                        .HasColumnName("Light4Mode");

                    b.Property<string>("Port")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Port");

                    b.Property<int>("TimerInterval")
                        .HasColumnType("INT")
                        .HasColumnName("TimerInterval");

                    b.Property<bool>("Transmit1")
                        .HasColumnType("BOOL")
                        .HasColumnName("Transmit1");

                    b.Property<bool>("Transmit2")
                        .HasColumnType("BOOL")
                        .HasColumnName("Transmit2");

                    b.Property<bool>("Transmit3")
                        .HasColumnType("BOOL")
                        .HasColumnName("Transmit3");

                    b.Property<bool>("Transmit4")
                        .HasColumnType("BOOL")
                        .HasColumnName("Transmit4");

                    b.HasKey("Id");

                    b.ToTable("AppConfigs");
                });

            modelBuilder.Entity("MARM.Data.BoatUnitMission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.Property<Guid>("MissionId")
                        .HasColumnType("TEXT")
                        .HasColumnName("MissionId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Name");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Note");

                    b.HasKey("Id");

                    b.ToTable("BoatUnitMissions");
                });

            modelBuilder.Entity("MARM.Data.BoatUnitShot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.Property<Guid>("BoatUnitId")
                        .HasColumnType("TEXT")
                        .HasColumnName("BoatUnitId");

                    b.Property<int>("Position")
                        .HasColumnType("INT")
                        .HasColumnName("Position");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT")
                        .HasColumnName("Time");

                    b.HasKey("Id");

                    b.ToTable("BoatUnitShots");
                });

            modelBuilder.Entity("MARM.Data.Mission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("TEXT")
                        .HasColumnName("CreateAt");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("TEXT")
                        .HasColumnName("ModifiedAt");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Name");

                    b.Property<Guid>("NavalUnitId")
                        .HasColumnType("TEXT")
                        .HasColumnName("NavalUnitId");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Note");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("TEXT")
                        .HasColumnName("StartAt");

                    b.Property<int>("State")
                        .HasColumnType("INT")
                        .HasColumnName("State");

                    b.HasKey("Id");

                    b.ToTable("Missions");
                });

            modelBuilder.Entity("MARM.Data.NavalUnit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("Name");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("TEXT")
                        .HasColumnName("ParentId");

                    b.HasKey("Id");

                    b.ToTable("NavalUnits");
                });
#pragma warning restore 612, 618
        }
    }
}
