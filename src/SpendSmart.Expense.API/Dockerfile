FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["SpendSmart.Contracts/SpendSmart.Contracts.csproj", "SpendSmart.Contracts/"]
COPY ["SpendSmart.Expense.API/SpendSmart.Expense.API.csproj", "SpendSmart.Expense.API/"]
RUN dotnet restore "SpendSmart.Expense.API/SpendSmart.Expense.API.csproj"
COPY SpendSmart.Contracts/ SpendSmart.Contracts/
COPY SpendSmart.Expense.API/ SpendSmart.Expense.API/
WORKDIR /src/SpendSmart.Expense.API
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpendSmart.Expense.API.dll"]