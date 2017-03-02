using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Senparc.Weixin.MP.Sample.MySQL.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    AddTime = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Address = table.Column<string>(maxLength: 250, nullable: true),
                    City = table.Column<string>(maxLength: 20, nullable: true),
                    Country = table.Column<string>(maxLength: 20, nullable: true),
                    District = table.Column<string>(maxLength: 20, nullable: true),
                    Email = table.Column<string>(maxLength: 150, nullable: true),
                    EmailChecked = table.Column<bool>(nullable: false),
                    HeadUrl = table.Column<string>(maxLength: 200, nullable: true),
                    LastLoginIp = table.Column<string>(maxLength: 50, nullable: true),
                    LastLoginTime = table.Column<DateTime>(nullable: false),
                    LastWeixinSignInTime = table.Column<DateTime>(nullable: true),
                    NickName = table.Column<string>(maxLength: 50, nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Password = table.Column<string>(maxLength: 50, nullable: false),
                    PasswordSalt = table.Column<string>(maxLength: 50, nullable: false),
                    PicUrl = table.Column<string>(maxLength: 200, nullable: true),
                    QQ = table.Column<string>(maxLength: 50, nullable: true),
                    RealName = table.Column<string>(maxLength: 50, nullable: true),
                    Sex = table.Column<int>(nullable: false),
                    Tel = table.Column<string>(maxLength: 50, nullable: true),
                    ThisLoginIp = table.Column<string>(maxLength: 50, nullable: true),
                    ThisLoginTime = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    WeixinOpenId = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CognitiveEmotions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    AccountId = table.Column<long>(nullable: false),
                    AddTime = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    PicUrl = table.Column<string>(maxLength: 250, nullable: true),
                    ResultJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CognitiveEmotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CognitiveEmotions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Sex",
                table: "Accounts",
                column: "Sex");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserName",
                table: "Accounts",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_CognitiveEmotions_AccountId",
                table: "CognitiveEmotions",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CognitiveEmotions");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
