using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace GeoCidadao.Database.Migrations
{
    /// <inheritdoc />
    public partial class PostLocationRelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_post_location",
                table: "post_location");

            migrationBuilder.DropIndex(
                name: "IX_post_location_category",
                table: "post_location");

            migrationBuilder.DropIndex(
                name: "IX_post_location_post_id",
                table: "post_location");

            migrationBuilder.DropColumn(
                name: "category",
                table: "post_location");

            migrationBuilder.DropColumn(
                name: "post_id",
                table: "post_location");

            migrationBuilder.RenameTable(
                name: "post_location",
                newName: "post_locations");

            migrationBuilder.RenameIndex(
                name: "IX_post_location_updated_at",
                table: "post_locations",
                newName: "IX_post_locations_updated_at");

            migrationBuilder.RenameIndex(
                name: "IX_post_location_position",
                table: "post_locations",
                newName: "IX_post_locations_position");

            migrationBuilder.RenameIndex(
                name: "IX_post_location_created_at",
                table: "post_locations",
                newName: "IX_post_locations_created_at");

            migrationBuilder.AlterColumn<Point>(
                name: "position",
                table: "post_locations",
                type: "geometry (Point)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "post_locations",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "post_locations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "post_locations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "post_locations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "suburb",
                table: "post_locations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_post_locations",
                table: "post_locations",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_post_locations_city",
                table: "post_locations",
                column: "city");

            migrationBuilder.CreateIndex(
                name: "IX_post_locations_country",
                table: "post_locations",
                column: "country");

            migrationBuilder.CreateIndex(
                name: "IX_post_locations_state",
                table: "post_locations",
                column: "state");

            migrationBuilder.CreateIndex(
                name: "IX_post_locations_suburb",
                table: "post_locations",
                column: "suburb");

            migrationBuilder.AddForeignKey(
                name: "FK_post_locations_posts_id",
                table: "post_locations",
                column: "id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_post_locations_posts_id",
                table: "post_locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_post_locations",
                table: "post_locations");

            migrationBuilder.DropIndex(
                name: "IX_post_locations_city",
                table: "post_locations");

            migrationBuilder.DropIndex(
                name: "IX_post_locations_country",
                table: "post_locations");

            migrationBuilder.DropIndex(
                name: "IX_post_locations_state",
                table: "post_locations");

            migrationBuilder.DropIndex(
                name: "IX_post_locations_suburb",
                table: "post_locations");

            migrationBuilder.DropColumn(
                name: "address",
                table: "post_locations");

            migrationBuilder.DropColumn(
                name: "city",
                table: "post_locations");

            migrationBuilder.DropColumn(
                name: "country",
                table: "post_locations");

            migrationBuilder.DropColumn(
                name: "state",
                table: "post_locations");

            migrationBuilder.DropColumn(
                name: "suburb",
                table: "post_locations");

            migrationBuilder.RenameTable(
                name: "post_locations",
                newName: "post_location");

            migrationBuilder.RenameIndex(
                name: "IX_post_locations_updated_at",
                table: "post_location",
                newName: "IX_post_location_updated_at");

            migrationBuilder.RenameIndex(
                name: "IX_post_locations_position",
                table: "post_location",
                newName: "IX_post_location_position");

            migrationBuilder.RenameIndex(
                name: "IX_post_locations_created_at",
                table: "post_location",
                newName: "IX_post_location_created_at");

            migrationBuilder.AlterColumn<Point>(
                name: "position",
                table: "post_location",
                type: "geometry",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (Point)");

            migrationBuilder.AddColumn<int>(
                name: "category",
                table: "post_location",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "post_id",
                table: "post_location",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_post_location",
                table: "post_location",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_post_location_category",
                table: "post_location",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_post_location_post_id",
                table: "post_location",
                column: "post_id");
        }
    }
}
