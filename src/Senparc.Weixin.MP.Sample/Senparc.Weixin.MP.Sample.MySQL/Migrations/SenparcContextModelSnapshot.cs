using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Senparc.Weixin.MP.Sample.MySQL.Models;

namespace Senparc.Weixin.MP.Sample.MySQL.Migrations
{
    [DbContext(typeof(SenparcContext))]
    partial class SenparcContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("Senparc.Weixin.MP.Sample.MySQL.Models.Account", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddTime")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .HasMaxLength(250);

                    b.Property<string>("City")
                        .HasMaxLength(20);

                    b.Property<string>("Country")
                        .HasMaxLength(20);

                    b.Property<string>("District")
                        .HasMaxLength(20);

                    b.Property<string>("Email")
                        .HasMaxLength(150);

                    b.Property<bool>("EmailChecked");

                    b.Property<string>("HeadUrl")
                        .HasMaxLength(200);

                    b.Property<string>("LastLoginIp")
                        .HasMaxLength(50);

                    b.Property<DateTime>("LastLoginTime");

                    b.Property<DateTime?>("LastWeixinSignInTime");

                    b.Property<string>("NickName")
                        .HasMaxLength(50);

                    b.Property<string>("Note");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("PicUrl")
                        .HasMaxLength(200);

                    b.Property<string>("QQ")
                        .HasMaxLength(50);

                    b.Property<string>("RealName")
                        .HasMaxLength(50);

                    b.Property<int>("Sex");

                    b.Property<string>("Tel")
                        .HasMaxLength(50);

                    b.Property<string>("ThisLoginIp")
                        .HasMaxLength(50);

                    b.Property<DateTime>("ThisLoginTime");

                    b.Property<int>("Type");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("WeixinOpenId")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("Sex");

                    b.HasIndex("UserName");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Senparc.Weixin.MP.Sample.MySQL.Models.CognitiveEmotion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AccountId");

                    b.Property<DateTime>("AddTime")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PicUrl")
                        .HasMaxLength(250);

                    b.Property<string>("ResultJson");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("CognitiveEmotions");
                });

            modelBuilder.Entity("Senparc.Weixin.MP.Sample.MySQL.Models.CognitiveEmotion", b =>
                {
                    b.HasOne("Senparc.Weixin.MP.Sample.MySQL.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
