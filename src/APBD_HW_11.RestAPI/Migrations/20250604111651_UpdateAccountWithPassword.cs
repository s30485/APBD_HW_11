﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APBD_HW_11.RestAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountWithPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Accounts");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Accounts",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Accounts",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
