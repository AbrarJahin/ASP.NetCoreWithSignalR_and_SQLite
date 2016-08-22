
## Publishing the app-

	# Remove the publish directory files before start
	dotnet restore
	dotnet build
	dotnet publish -r ubuntu.14.04-x64

### Before publish-

Run Migration-

	dotnet ef migrations add testMigration
	dotnet ef database update

Detail-

https://damienbod.com/2015/08/30/asp-net-5-with-sqlite-and-entity-framework-7/

See the result directory with a platform specific dotnet command able to run the app.

## Server Config-

https://docs.asp.net/en/latest/publishing/linuxproduction.html
