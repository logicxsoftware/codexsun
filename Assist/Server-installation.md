# Server Installation (Ubuntu)

This guide deploys the current project on Ubuntu using Docker with a dedicated production compose file.

## 1. Install Docker

```bash
sudo apt update
sudo apt install -y docker.io docker-compose-plugin git
sudo systemctl enable --now docker
sudo usermod -aG docker "$USER"
```

Log out and log back in once after adding your user to the `docker` group.

## 2. Clone the Project

```bash
git clone <your-repository-url> codexsun
cd codexsun
```

## 3. Prepare Production App Settings

Create or update:

- `cxserver/appsettings.Production.json`

Set production DB values there (do not keep dev passwords).

Recommended minimal structure:

```json
{
  "Database": {
    "Provider": "MariaDb",
    "MariaDbServerVersion": "11.5.2",
    "Master": {
      "ConnectionString": "server=mariadb;port=3306;database=codexsun_prod;user=<db_user>;password=<strong_password>;"
    },
    "Tenant": {
      "ConnectionStringTemplate": "server=mariadb;port=3306;database={database};user=<db_user>;password=<strong_password>;"
    }
  }
}
```

## 4. Create Shared Docker Network

```bash
docker network create codexion-network || true
```

## 5. Run MariaDB Separately (Dedicated DB Container)

```bash
docker run -d \
  --name mariadb \
  --network codexion-network \
  -e MARIADB_ROOT_PASSWORD='<strong_password>' \
  -e MARIADB_DATABASE='codexsun_prod' \
  -v mariadb-data:/var/lib/mysql \
  --restart unless-stopped \
  mariadb:11.5
```

If you already have an external MariaDB server/container, keep the hostname in your app settings aligned (`mariadb` or your DB host).

## 6. Build and Start Application (Production)

Use the dedicated production compose file:

```bash
docker compose -f docker-compose.prod.yml up -d --build
```

## 7. Verify Runtime

```bash
docker compose -f docker-compose.prod.yml ps
docker compose -f docker-compose.prod.yml logs -f apphost
```

Expected exposed ports:

- AppHost dashboard: `7040`
- API HTTP: `7041`
- API HTTPS: `7042`
- Frontend: `7043`

## 8. Update Deployment

```bash
git pull
docker compose -f docker-compose.prod.yml up -d --build
```

## 9. Optional: Auto-Start on Reboot

The compose service already uses:

- `restart: unless-stopped`

So containers auto-restart with Docker daemon after reboot.

## Production Files Created

- `Dockerfile.prod`
- `docker-compose.prod.yml`
