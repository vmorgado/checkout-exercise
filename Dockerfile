FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster

COPY . /App
WORKDIR /App


RUN dotnet user-secrets set "Kestrel:Certificates:Development:Password" "defaultpassword" --project "/App/src/dotnetexample.csproj"

CMD ["dotnet", "run", "--project", "/App/src/dotnetexample.csproj"]