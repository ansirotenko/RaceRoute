# RaceRoute

## Description

- [En description](./docs/en.md)
- [Ru description](https://gist.github.com/Eflarus/7bae02da72d49ed6991e1680af79006d)

## start docker
- navigate to `scripts` folder
- execute 'docker-build.cmd'
- execute 'docker compose --profile all up'

## start local dev
- navigate to `scripts` folder
- execute `docker compose --profile db up`
- navigate to `front` folder
- execute `npm ci`
- execute `npm run dev`
- navigate to `back` folder
- execute `dotnet run --project src/RaceRoute.Web/RaceRoute.Web.csproj`

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
