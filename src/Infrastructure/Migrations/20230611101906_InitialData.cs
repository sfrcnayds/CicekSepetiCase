using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class InitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .Sql($"INSERT INTO \"Users\" (\"Id\",\"Name\",\"Surname\",\"Username\",\"EmailAddress\",\"Password\") Values "+
                     "('d9efde00-bb0e-4f30-8f4f-ed7ba7ac8599','Sefercan','Aydaş','sfrcnayds','sfrcnayds@gmail.com','$2b$10$Ms4Iu726Qy388kq2FzwQ/eBlMBnnJ80tj8wVPvy5W/A8jxhcHANIa')");
            migrationBuilder
                .Sql($"INSERT INTO \"Users\" (\"Id\",\"Name\",\"Surname\",\"Username\",\"EmailAddress\",\"Password\") Values "+
                     "('535fbd99-f400-4c1d-8a12-90bb82eabb53','Test','User','testuser','testuser@test.com','AQAAAAEAACcQAAAAEGPPk/6+oMzXIgHQH1PIuWtmcpK4IYYqS38rNxlgMWws4Qgl0Eo8gRuPmetmqBEX6g==')");
            
            migrationBuilder
                .Sql("INSERT INTO \"Products\" (\"Id\",\"Name\",\"Sku\",\"Price\",\"StockQuantity\") Values " +
                     "('ca4e4352-85ea-4abb-aa98-bf843a57310a','Kırmızı Gül','KG727',100,5)");
            migrationBuilder
                .Sql("INSERT INTO \"Products\" (\"Id\",\"Name\",\"Sku\",\"Price\",\"StockQuantity\") Values " +
                     "('83dc5fd3-e294-40bb-94d8-d22087160383','Beyaz Gül','BG123',200,10)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .Sql("DELETE FROM \"Users\" WHERE Id='d9efde00-bb0e-4f30-8f4f-ed7ba7ac8599'");
            migrationBuilder
                .Sql("DELETE FROM \"Users\" WHERE Id='535fbd99-f400-4c1d-8a12-90bb82eabb53'");
            
            migrationBuilder
                .Sql("DELETE FROM \"Products\" WHERE Id='ca4e4352-85ea-4abb-aa98-bf843a57310a'");
            migrationBuilder
                .Sql("DELETE FROM \"Products\" WHERE Id='83dc5fd3-e294-40bb-94d8-d22087160383'");
        }
    }
}
