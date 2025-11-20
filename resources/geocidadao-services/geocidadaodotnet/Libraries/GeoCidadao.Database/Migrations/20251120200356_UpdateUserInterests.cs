using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoCidadao.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserInterests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_interests_user_profiles_user_id",
                table: "user_interests");

            migrationBuilder.DropIndex(
                name: "IX_user_interests_user_id",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "categories",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "city",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "region",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "state",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "user_interests");

            migrationBuilder.AddColumn<int[]>(
                name: "followed_categories",
                table: "user_interests",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<List<string>>(
                name: "followed_cities",
                table: "user_interests",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "followed_districts",
                table: "user_interests",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "followed_users",
                table: "user_interests",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "interest_range",
                table: "user_interests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_user_interests_user_profiles_id",
                table: "user_interests",
                column: "id",
                principalTable: "user_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_interests_user_profiles_id",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "followed_categories",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "followed_cities",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "followed_districts",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "followed_users",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "interest_range",
                table: "user_interests");

            migrationBuilder.AddColumn<int[]>(
                name: "categories",
                table: "user_interests",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "user_interests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "region",
                table: "user_interests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "user_interests",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "user_interests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_user_interests_user_id",
                table: "user_interests",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_user_interests_user_profiles_user_id",
                table: "user_interests",
                column: "user_id",
                principalTable: "user_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
