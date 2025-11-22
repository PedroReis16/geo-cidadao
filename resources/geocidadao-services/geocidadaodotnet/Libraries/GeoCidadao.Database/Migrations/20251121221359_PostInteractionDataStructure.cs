using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoCidadao.Database.Migrations
{
    /// <inheritdoc />
    public partial class PostInteractionDataStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "post_comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_comments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "post_likes",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_likes", x => new { x.user_id, x.PostId });
                });

            migrationBuilder.CreateTable(
                name: "comment_likes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment_likes", x => x.id);
                    table.ForeignKey(
                        name: "FK_comment_likes_post_comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "post_comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comment_likes_CommentId",
                table: "comment_likes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_comment_likes_created_at",
                table: "comment_likes",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_comment_likes_id_user_id",
                table: "comment_likes",
                columns: new[] { "id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_comment_likes_updated_at",
                table: "comment_likes",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_comment_likes_user_id",
                table: "comment_likes",
                column: "user_id");

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
                name: "IX_post_likes_id_user_id",
                table: "post_likes",
                columns: new[] { "id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_PostId",
                table: "post_likes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_updated_at",
                table: "post_likes",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_user_id",
                table: "post_likes",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comment_likes");

            migrationBuilder.DropTable(
                name: "post_likes");

            migrationBuilder.DropTable(
                name: "post_comments");
        }
    }
}
