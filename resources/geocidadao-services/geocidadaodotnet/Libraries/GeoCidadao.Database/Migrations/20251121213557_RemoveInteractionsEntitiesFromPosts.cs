using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoCidadao.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInteractionsEntitiesFromPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "post_comments");

            migrationBuilder.DropTable(
                name: "post_likes");

            migrationBuilder.DropIndex(
                name: "IX_posts_relevance_score",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "comments_count",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "likes_count",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "relevance_score",
                table: "posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "comments_count",
                table: "posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "likes_count",
                table: "posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "relevance_score",
                table: "posts",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "post_comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_comments_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_likes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_likes", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_likes_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_posts_relevance_score",
                table: "posts",
                column: "relevance_score");

            migrationBuilder.CreateIndex(
                name: "IX_post_comments_created_at",
                table: "post_comments",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_post_comments_post_id",
                table: "post_comments",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_comments_updated_at",
                table: "post_comments",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_post_comments_user_id",
                table: "post_comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_created_at",
                table: "post_likes",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_post_id",
                table: "post_likes",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_post_id_user_id",
                table: "post_likes",
                columns: new[] { "post_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_updated_at",
                table: "post_likes",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_user_id",
                table: "post_likes",
                column: "user_id");
        }
    }
}
