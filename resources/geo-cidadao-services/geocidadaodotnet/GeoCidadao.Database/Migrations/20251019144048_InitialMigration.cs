using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoCidadao.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_profile",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profile", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_picture",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileHash = table.Column<string>(type: "text", nullable: false),
                    FileExtension = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_picture", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_picture_user_profile_id",
                        column: x => x.id,
                        principalTable: "user_profile",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_picture_created_at",
                table: "user_picture",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_user_picture_FileHash",
                table: "user_picture",
                column: "FileHash");

            migrationBuilder.CreateIndex(
                name: "IX_user_picture_updated_at",
                table: "user_picture",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_user_profile_created_at",
                table: "user_profile",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_user_profile_email",
                table: "user_profile",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_user_profile_updated_at",
                table: "user_profile",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_user_profile_username",
                table: "user_profile",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_picture");

            migrationBuilder.DropTable(
                name: "user_profile");
        }
    }
}
