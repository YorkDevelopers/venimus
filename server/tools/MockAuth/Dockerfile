FROM mcr.microsoft.com/dotnet/core/aspnet:3.0

WORKDIR /app
COPY /mockauth /app
ENTRYPOINT ["dotnet", "MockAuth.dll"]