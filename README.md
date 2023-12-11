# RaceRoute

Todo
- UI_STATIC
- SpaFiles + DevProxy

# For developers
## migrations
- navigate to `scripts` folder
- execute `docker compose --profile db up`
### Create initial migration
- navigate to `back` directory
- `dotnet ef migrations --project src/RaceRoute.Core add InitialCreate -- "Server=127.0.0.1,1401;Database=RoadRoute;User Id=SA;Password=My&Strong1234567;Encrypt=False;"`
### Update database
- navigate to `back` directory
- `dotnet ef database update --project src/RaceRoute.Core -- "Server=127.0.0.1,1401;Database=RoadRoute;User Id=SA;Password=My&Strong1234567;Encrypt=False;"`
