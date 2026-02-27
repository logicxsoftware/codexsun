FROM mcr.microsoft.com/dotnet/sdk:10.0

# Install Node.js 22.x for Aspire Vite app execution.
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates curl gnupg \
    && mkdir -p /etc/apt/keyrings \
    && curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
    && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_22.x nodistro main" > /etc/apt/sources.list.d/nodesource.list \
    && apt-get update \
    && apt-get install -y --no-install-recommends nodejs \
    && npm install -g npm@latest \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY . .

WORKDIR /src/cxweb
RUN npm ci

WORKDIR /src
RUN dotnet restore codexsun.sln
RUN dotnet build codexsun.AppHost/codexsun.AppHost.csproj -c Debug --no-restore

EXPOSE 15258 19046 18011 20098

WORKDIR /src/codexsun.AppHost
CMD ["dotnet", "run", "--no-launch-profile", "--no-build", "--no-restore"]
