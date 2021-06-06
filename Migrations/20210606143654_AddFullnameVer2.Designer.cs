﻿// <auto-generated />
using System;
using Angeloid.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Angeloid.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20210606143654_AddFullnameVer2")]
    partial class AddFullnameVer2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Angeloid.Models.Anime", b =>
                {
                    b.Property<int>("AnimeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AnimeName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Episode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EpisodeDuration")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SeasonId")
                        .HasColumnType("int");

                    b.Property<string>("StartDay")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StudioId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Thumbnail")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Trailer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("View")
                        .HasColumnType("int");

                    b.Property<byte[]>("Wallpaper")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Web")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AnimeId");

                    b.HasIndex("SeasonId");

                    b.HasIndex("StudioId");

                    b.ToTable("Animes");
                });

            modelBuilder.Entity("Angeloid.Models.AnimeTag", b =>
                {
                    b.Property<int>("AnimeId")
                        .HasColumnType("int");

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.HasKey("AnimeId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("AnimeTags");
                });

            modelBuilder.Entity("Angeloid.Models.Character", b =>
                {
                    b.Property<int>("CharacterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AnimeId")
                        .HasColumnType("int");

                    b.Property<byte[]>("CharacterImage")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("CharacterName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CharacterRole")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SeiyuuId")
                        .HasColumnType("int");

                    b.HasKey("CharacterId");

                    b.HasIndex("AnimeId");

                    b.HasIndex("SeiyuuId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Angeloid.Models.Favorite", b =>
                {
                    b.Property<int>("AnimeId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("AnimeId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("Angeloid.Models.Review", b =>
                {
                    b.Property<int>("AnimeId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RateScore")
                        .HasColumnType("int");

                    b.HasKey("AnimeId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("Angeloid.Models.Season", b =>
                {
                    b.Property<int>("SeasonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("SeasonName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Year")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SeasonId");

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("Angeloid.Models.Seiyuu", b =>
                {
                    b.Property<int>("SeiyuuId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("SeiyuuImage")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("SeiyuuName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SeiyuuId");

                    b.ToTable("Seiyuus");
                });

            modelBuilder.Entity("Angeloid.Models.Studio", b =>
                {
                    b.Property<int>("StudioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("StudioName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StudioId");

                    b.ToTable("Studios");
                });

            modelBuilder.Entity("Angeloid.Models.Tag", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TagDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TagName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TagId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Angeloid.Models.Thread", b =>
                {
                    b.Property<int>("ThreadId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Image")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ThreadId");

                    b.HasIndex("UserId");

                    b.ToTable("Threads");
                });

            modelBuilder.Entity("Angeloid.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Avatar")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FacebookId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fullname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Angeloid.Models.Anime", b =>
                {
                    b.HasOne("Angeloid.Models.Season", "Season")
                        .WithMany("Animes")
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Angeloid.Models.Studio", "Studio")
                        .WithMany("Animes")
                        .HasForeignKey("StudioId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Season");

                    b.Navigation("Studio");
                });

            modelBuilder.Entity("Angeloid.Models.AnimeTag", b =>
                {
                    b.HasOne("Angeloid.Models.Anime", "Anime")
                        .WithMany("AnimeTags")
                        .HasForeignKey("AnimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Angeloid.Models.Tag", "Tag")
                        .WithMany("AnimeTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Anime");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Angeloid.Models.Character", b =>
                {
                    b.HasOne("Angeloid.Models.Anime", "Anime")
                        .WithMany("Characters")
                        .HasForeignKey("AnimeId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Angeloid.Models.Seiyuu", "Seiyuu")
                        .WithMany("Characters")
                        .HasForeignKey("SeiyuuId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Anime");

                    b.Navigation("Seiyuu");
                });

            modelBuilder.Entity("Angeloid.Models.Favorite", b =>
                {
                    b.HasOne("Angeloid.Models.Anime", "Anime")
                        .WithMany("Favorites")
                        .HasForeignKey("AnimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Angeloid.Models.User", "User")
                        .WithMany("Favorites")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Anime");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Angeloid.Models.Review", b =>
                {
                    b.HasOne("Angeloid.Models.Anime", "Anime")
                        .WithMany("Reviews")
                        .HasForeignKey("AnimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Angeloid.Models.User", "User")
                        .WithMany("Reviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Anime");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Angeloid.Models.Thread", b =>
                {
                    b.HasOne("Angeloid.Models.User", "User")
                        .WithMany("Threads")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Angeloid.Models.Anime", b =>
                {
                    b.Navigation("AnimeTags");

                    b.Navigation("Characters");

                    b.Navigation("Favorites");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("Angeloid.Models.Season", b =>
                {
                    b.Navigation("Animes");
                });

            modelBuilder.Entity("Angeloid.Models.Seiyuu", b =>
                {
                    b.Navigation("Characters");
                });

            modelBuilder.Entity("Angeloid.Models.Studio", b =>
                {
                    b.Navigation("Animes");
                });

            modelBuilder.Entity("Angeloid.Models.Tag", b =>
                {
                    b.Navigation("AnimeTags");
                });

            modelBuilder.Entity("Angeloid.Models.User", b =>
                {
                    b.Navigation("Favorites");

                    b.Navigation("Reviews");

                    b.Navigation("Threads");
                });
#pragma warning restore 612, 618
        }
    }
}
