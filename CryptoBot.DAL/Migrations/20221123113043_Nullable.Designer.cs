﻿// <auto-generated />
using System;
using CryptoBot.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CryptoBot.DAL.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20221123113043_Nullable")]
    partial class Nullable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CryptoBot.DAL.Models.User", b =>
                {
                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("TelegramId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CryptoBot.DAL.Models.UserPostInfo", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("CryptoSet")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Currency")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastPostTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Timer")
                        .HasColumnType("integer");

                    b.HasKey("UserId");

                    b.ToTable("UserPostsInfo");
                });

            modelBuilder.Entity("CryptoBot.DAL.Models.UserPostInfo", b =>
                {
                    b.HasOne("CryptoBot.DAL.Models.User", "User")
                        .WithOne("PostInfo")
                        .HasForeignKey("CryptoBot.DAL.Models.UserPostInfo", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CryptoBot.DAL.Models.User", b =>
                {
                    b.Navigation("PostInfo")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
