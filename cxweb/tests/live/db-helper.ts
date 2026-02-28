import fs from "node:fs/promises"
import path from "node:path"
import mysql, { type Pool, type PoolOptions, type RowDataPacket } from "mysql2/promise"

type BackendAppSettings = {
  Environment: string
  AppEnv: Record<string, { Database: { Provider: string; Master: { ConnectionString: string } } }>
}

type ParsedConnectionString = {
  host: string
  port: number
  database: string
  user: string
  password: string
}

export type TenantRecord = {
  id: string
  identifier: string
  domain: string
  databaseName: string
  connectionString: string
}

const parseConnectionString = (connectionString: string): ParsedConnectionString => {
  const entries = connectionString
    .split(";")
    .map((item) => item.trim())
    .filter((item) => item.length > 0)
    .map((item) => {
      const index = item.indexOf("=")
      const key = index >= 0 ? item.slice(0, index).trim().toLowerCase() : item.trim().toLowerCase()
      const value = index >= 0 ? item.slice(index + 1).trim() : ""
      return [key, value] as const
    })

  const map = new Map<string, string>(entries)
  const host = map.get("server") ?? map.get("host") ?? "localhost"
  const portRaw = map.get("port") ?? "3306"
  const database = map.get("database") ?? ""
  const user = map.get("user") ?? map.get("uid") ?? map.get("username") ?? "root"
  const password = map.get("password") ?? map.get("pwd") ?? ""

  const port = Number.parseInt(portRaw, 10)
  if (!database) {
    throw new Error("Connection string database is empty")
  }
  if (Number.isNaN(port)) {
    throw new Error("Connection string port is invalid")
  }

  return { host, port, database, user, password }
}

const toMysqlOptions = (connectionString: string): PoolOptions => {
  const parsed = parseConnectionString(connectionString)
  return {
    host: parsed.host,
    port: parsed.port,
    database: parsed.database,
    user: parsed.user,
    password: parsed.password,
    waitForConnections: true,
    connectionLimit: 10,
    namedPlaceholders: true,
  }
}

const readBackendSettings = async (): Promise<BackendAppSettings> => {
  const settingsPath = path.resolve(process.cwd(), "..", "cxserver", "appsettings.json")
  const content = await fs.readFile(settingsPath, "utf-8")
  return JSON.parse(content) as BackendAppSettings
}

export const createMasterPool = async (): Promise<Pool> => {
  const settings = await readBackendSettings()
  const selectedEnvironment = settings.Environment
  const envSettings = settings.AppEnv[selectedEnvironment]
  if (!envSettings) {
    throw new Error(`Environment ${selectedEnvironment} is missing from appsettings.json`)
  }
  const provider = envSettings.Database.Provider.toLowerCase()
  if (!provider.includes("maria") && !provider.includes("mysql")) {
    throw new Error(`Live DB helper currently supports MariaDB/MySQL only. Found provider: ${envSettings.Database.Provider}`)
  }

  const options = toMysqlOptions(envSettings.Database.Master.ConnectionString)
  return mysql.createPool(options)
}

export const createTenantPool = (connectionString: string): Pool => {
  const options = toMysqlOptions(connectionString)
  return mysql.createPool(options)
}

export const getActiveTenants = async (): Promise<TenantRecord[]> => {
  const pool = await createMasterPool()
  try {
    const [rows] = await pool.query<RowDataPacket[]>(
      `select id, identifier, domain, database_name as databaseName, connection_string as connectionString
       from tenants
       where is_deleted = 0 and status = 1
       order by created_at_utc asc`,
    )

    return rows.map((row) => ({
      id: String(row.id),
      identifier: String(row.identifier),
      domain: String(row.domain),
      databaseName: String(row.databaseName),
      connectionString: String(row.connectionString),
    }))
  } finally {
    await pool.end()
  }
}

export const getSingleNumber = async (pool: Pool, sql: string, values: unknown[] = []): Promise<number> => {
  const [rows] = await pool.query<RowDataPacket[]>(sql, values)
  const row = rows[0]
  if (!row) {
    return 0
  }
  const firstValue = Object.values(row)[0]
  return typeof firstValue === "number" ? firstValue : Number(firstValue)
}
