using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductManagementAss2.Migrations
{
    /// <inheritdoc />
    public partial class initi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "601ee63f-246d-4aa3-bed3-fd31afcd1587", "5395f69c-c49b-4ae7-b876-cb8970aec53a" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1", "5395f69c-c49b-4ae7-b876-cb8970aec53a" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1", "b49eda27-8b9b-4bdf-9a89-940be8a5f71d" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "59e05f62-2239-41fe-adcb-172b84703060", "df2a0a3a-9b6b-4861-b636-f219e16e08a9" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "601ee63f-246d-4aa3-bed3-fd31afcd1587", "df2a0a3a-9b6b-4861-b636-f219e16e08a9" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1", "df2a0a3a-9b6b-4861-b636-f219e16e08a9" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "59e05f62-2239-41fe-adcb-172b84703060");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "601ee63f-246d-4aa3-bed3-fd31afcd1587");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5395f69c-c49b-4ae7-b876-cb8970aec53a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b49eda27-8b9b-4bdf-9a89-940be8a5f71d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "df2a0a3a-9b6b-4861-b636-f219e16e08a9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1",
                column: "ConcurrencyStamp",
                value: "82e87ace-202b-4eec-87f3-780feaf9a00c");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3ed1a283-41e8-458b-9983-69a9ed7695b9", "463e593c-e20c-4d28-87e6-f3fefad7b6d8", "SuperAdmin", "SUPERADMIN" },
                    { "c0f91524-4a7e-4a34-a122-19b869435d7e", "e3ba1b38-dd82-4d20-8b6e-b42b11daa1c4", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "18962e75-0bb1-48d7-b1d8-2f18a340e753", 0, "80fe1b77-2eb9-438b-88c7-75b424db0ce0", "ApplicationUser", "user@example.com", true, "Demo", "User", false, null, "USER@EXAMPLE.COM", "USER@EXAMPLE.COM", "AQAAAAEAACcQAAAAEOoVRkFXYkP98VFgYVKp+Dg4BadICLrgcZOjhNkSBfGvLF79nXOdnNLiWBbkXu/JTg==", null, false, "e848ae8b-01f7-4fed-a12b-836511b56b14", false, "user@example.com" },
                    { "396a8a3e-b366-412b-968e-62a73dbf9423", 0, "87fe3cda-d177-47b4-b2f2-5fd81ed2f459", "ApplicationUser", "superadmin@example.com", true, "Steve", "Roy", false, null, "SUPERADMIN@EXAMPLE.COM", "SUPERADMIN@EXAMPLE.COM", "AQAAAAEAACcQAAAAED0TSGkkHcNZVOYlyqQkzO1srRW0KKvepdL+1IdhokkZwG/0h+gl938dPjjDYkFQBQ==", null, false, "f678e129-a952-4356-b5eb-2913b605fc0d", false, "superadmin@example.com" },
                    { "df0a6550-5ab6-4ca2-a113-0b282af1c14f", 0, "830543f5-78ec-4c87-9d7e-2528ca6f02a7", "ApplicationUser", "admin@example.com", true, "John", "Mellus", false, null, "ADMIN@EXAMPLE.COM", "ADMIN@EXAMPLE.COM", "AQAAAAEAACcQAAAAELB6Y02IwdhgDF++45Z3CpshzacAP7aCB4gAOL2vjhxtDid/lM9YzVX16wsJ4O5UVQ==", null, false, "e2fdea42-dfc3-4e37-986e-949f0dc6d865", false, "admin@example.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1", "18962e75-0bb1-48d7-b1d8-2f18a340e753" },
                    { "3ed1a283-41e8-458b-9983-69a9ed7695b9", "396a8a3e-b366-412b-968e-62a73dbf9423" },
                    { "c0f91524-4a7e-4a34-a122-19b869435d7e", "396a8a3e-b366-412b-968e-62a73dbf9423" },
                    { "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1", "396a8a3e-b366-412b-968e-62a73dbf9423" },
                    { "c0f91524-4a7e-4a34-a122-19b869435d7e", "df0a6550-5ab6-4ca2-a113-0b282af1c14f" },
                    { "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1", "df0a6550-5ab6-4ca2-a113-0b282af1c14f" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1", "18962e75-0bb1-48d7-b1d8-2f18a340e753" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3ed1a283-41e8-458b-9983-69a9ed7695b9", "396a8a3e-b366-412b-968e-62a73dbf9423" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c0f91524-4a7e-4a34-a122-19b869435d7e", "396a8a3e-b366-412b-968e-62a73dbf9423" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1", "396a8a3e-b366-412b-968e-62a73dbf9423" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c0f91524-4a7e-4a34-a122-19b869435d7e", "df0a6550-5ab6-4ca2-a113-0b282af1c14f" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1", "df0a6550-5ab6-4ca2-a113-0b282af1c14f" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3ed1a283-41e8-458b-9983-69a9ed7695b9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c0f91524-4a7e-4a34-a122-19b869435d7e");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "18962e75-0bb1-48d7-b1d8-2f18a340e753");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "396a8a3e-b366-412b-968e-62a73dbf9423");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "df0a6550-5ab6-4ca2-a113-0b282af1c14f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1",
                column: "ConcurrencyStamp",
                value: "99a92639-8bea-4f7f-b97d-28a372aee335");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "59e05f62-2239-41fe-adcb-172b84703060", "9cad48cf-13ec-4a1b-b09a-9936a651a1d6", "SuperAdmin", "SUPERADMIN" },
                    { "601ee63f-246d-4aa3-bed3-fd31afcd1587", "07d718ec-b419-4a22-9569-80f396f40f49", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "5395f69c-c49b-4ae7-b876-cb8970aec53a", 0, "db2516ce-23c4-449d-b9a0-6085c6a8df2e", "ApplicationUser", "admin@example.com", true, "John", "Mellus", false, null, "ADMIN@EXAMPLE.COM", "ADMIN@EXAMPLE.COM", "AQAAAAEAACcQAAAAEPdB1yeX0XTdzzfUKsHNQJUFnNrNYvysIeHfVSKvHznO57XnzFv495PjhQEZtBJ9MA==", null, false, "9152b3da-2627-4d7a-b447-0afa902101de", false, "admin@example.com" },
                    { "b49eda27-8b9b-4bdf-9a89-940be8a5f71d", 0, "11df2b51-f769-41fc-877f-95dcb617952b", "ApplicationUser", "user@example.com", true, "Demo", "User", false, null, "USER@EXAMPLE.COM", "USER@EXAMPLE.COM", "AQAAAAEAACcQAAAAEMiUo1MWYWUBqazydZQVxkx2WrWvPCkO1N6l/63ZEkInTlE3yQmjD1LtOabQHiEMCQ==", null, false, "7fabd9ea-dfbc-4023-982f-def5fdcaac0e", false, "user@example.com" },
                    { "df2a0a3a-9b6b-4861-b636-f219e16e08a9", 0, "350e9170-9cc2-491b-a292-898243e7809c", "ApplicationUser", "superadmin@example.com", true, "Steve", "Roy", false, null, "SUPERADMIN@EXAMPLE.COM", "SUPERADMIN@EXAMPLE.COM", "AQAAAAEAACcQAAAAEJ1qpfLLsQW1MY8xaxV/GjVtE883BvtONbWheVkvfoZnn4diHtYNmZDB/bvsH0dyBQ==", null, false, "78bdf0cd-9f08-41fb-b91a-a361a7b4cf53", false, "superadmin@example.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "601ee63f-246d-4aa3-bed3-fd31afcd1587", "5395f69c-c49b-4ae7-b876-cb8970aec53a" },
                    { "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1", "5395f69c-c49b-4ae7-b876-cb8970aec53a" },
                    { "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1", "b49eda27-8b9b-4bdf-9a89-940be8a5f71d" },
                    { "59e05f62-2239-41fe-adcb-172b84703060", "df2a0a3a-9b6b-4861-b636-f219e16e08a9" },
                    { "601ee63f-246d-4aa3-bed3-fd31afcd1587", "df2a0a3a-9b6b-4861-b636-f219e16e08a9" },
                    { "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1", "df2a0a3a-9b6b-4861-b636-f219e16e08a9" }
                });
        }
    }
}
