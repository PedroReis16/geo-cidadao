using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoCidadao.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPostInteractionsAndMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add metrics columns to post table
            migrationBuilder.AddColumn<int>(
                name: "likes_count",
                table: "post",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "comments_count",
                table: "post",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "relevance_score",
                table: "post",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            // Create post_like table
            migrationBuilder.CreateTable(
                name: "post_like",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_like", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_like_post_post_id",
                        column: x => x.post_id,
                        principalTable: "post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create post_comment table
            migrationBuilder.CreateTable(
                name: "post_comment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_comment", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_comment_post_post_id",
                        column: x => x.post_id,
                        principalTable: "post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes for post table
            migrationBuilder.CreateIndex(
                name: "IX_post_relevance_score",
                table: "post",
                column: "relevance_score");

            // Create indexes for post_like table
            migrationBuilder.CreateIndex(
                name: "IX_post_like_created_at",
                table: "post_like",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_post_like_post_id",
                table: "post_like",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_like_user_id",
                table: "post_like",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_like_updated_at",
                table: "post_like",
                column: "updated_at");

            // Create unique index for post_id + user_id combination
            migrationBuilder.CreateIndex(
                name: "IX_post_like_post_id_user_id",
                table: "post_like",
                columns: new[] { "post_id", "user_id" },
                unique: true);

            // Create indexes for post_comment table
            migrationBuilder.CreateIndex(
                name: "IX_post_comment_created_at",
                table: "post_comment",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_post_comment_post_id",
                table: "post_comment",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_comment_user_id",
                table: "post_comment",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_comment_updated_at",
                table: "post_comment",
                column: "updated_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "post_like");

            migrationBuilder.DropTable(
                name: "post_comment");

            migrationBuilder.DropIndex(
                name: "IX_post_relevance_score",
                table: "post");

            migrationBuilder.DropColumn(
                name: "likes_count",
                table: "post");

            migrationBuilder.DropColumn(
                name: "comments_count",
                table: "post");

            migrationBuilder.DropColumn(
                name: "relevance_score",
                table: "post");
        }
    }
}
