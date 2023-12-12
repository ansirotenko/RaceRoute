SET configuration=Debug
SET dotnetVersion=net7.0
SET installDebugClr="cd ../ && apt-get update && apt-get install -y --no-install-recommends unzip && apt-get install -y curl && rm -rf var/lib/apt/lists/* && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg"
SET project=RaceRoute.Web
SET projectPath=./back/src/%project%

cd ../front

CMD /c npm ci
CMD /c npm run build

cd ../

dotnet publish %projectPath%/%project%.csproj -c %configuration%

xcopy /S /Q /Y /D /e /i "./front/dist" "%projectPath%/bin/%configuration%/%dotnetVersion%/publish/wwwroot/"

docker build %projectPath%/bin/%configuration%/%dotnetVersion%/publish -t race-route:debug -f %projectPath%/Dockerfile --build-arg INSTALL_CLRDBG=%installDebugClr%