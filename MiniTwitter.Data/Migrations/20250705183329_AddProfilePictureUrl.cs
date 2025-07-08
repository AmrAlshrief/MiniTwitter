using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniTwitter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tweets_UserId",
                table: "Tweets");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_FollowingId",
                table: "Follows",
                newName: "IX_FollowingId");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_FollowerId",
                table: "Follows",
                newName: "IX_FollowerId");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_UserId_CreatedAt",
                table: "Tweets",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FollowerId_FollowingId",
                table: "Follows",
                columns: new[] { "FollowerId", "FollowingId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tweets_UserId_CreatedAt",
                table: "Tweets");

            migrationBuilder.DropIndex(
                name: "IX_FollowerId_FollowingId",
                table: "Follows");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_FollowingId",
                table: "Follows",
                newName: "IX_Follows_FollowingId");

            migrationBuilder.RenameIndex(
                name: "IX_FollowerId",
                table: "Follows",
                newName: "IX_Follows_FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tweets_UserId",
                table: "Tweets",
                column: "UserId");
        }
    }
}
