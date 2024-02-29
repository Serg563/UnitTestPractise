﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderApi.SignalR.Services.Data;

#nullable disable

namespace OrderApi.Migrations.SignalR
{
    [DbContext(typeof(SignalRContext))]
    [Migration("20240227170608_UpdatedMessagesEntity")]
    partial class UpdatedMessagesEntity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("OrderApi.SignalR.Services.Data.Connections", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConnectionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "ConnectionId");

                    b.ToTable("Connections");
                });

            modelBuilder.Entity("OrderApi.SignalR.Services.Data.Messages", b =>
                {
                    b.Property<int>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Key"));

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Time")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Key");

                    b.HasIndex("UserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("OrderApi.SignalR.Services.Data.Users", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("OrderApi.SignalR.Services.Data.Connections", b =>
                {
                    b.HasOne("OrderApi.SignalR.Services.Data.Users", "User")
                        .WithMany("Connections")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("OrderApi.SignalR.Services.Data.Messages", b =>
                {
                    b.HasOne("OrderApi.SignalR.Services.Data.Users", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("OrderApi.SignalR.Services.Data.Users", b =>
                {
                    b.Navigation("Connections");

                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
