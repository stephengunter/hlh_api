# hlh_api
dotnet build ./hlh_api.sln && dotnet run --project ./Web 

dotnet ef migrations add Event-test -s ../Web -o PostgreSqlMigrations/

dotnet ef migrations add init -s ../Web
dotnet ef migrations remove -s ../Web
dotnet ef database update -s ../Web

