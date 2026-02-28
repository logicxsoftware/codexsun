export const liveBackendUrl = process.env.LIVE_BACKEND_URL ?? "http://localhost:7041"
export const liveFrontendUrl = process.env.LIVE_FRONTEND_URL ?? "http://localhost:7043"
export const livePrimaryDomain = process.env.LIVE_PRIMARY_DOMAIN ?? "localhost"
export const liveSecondaryDomain = process.env.LIVE_SECONDARY_DOMAIN ?? "127.0.0.1.nip.io"

export const tenantHeaders = (domain: string): Record<string, string> => ({
  Host: domain,
  "X-Forwarded-Host": domain,
})
