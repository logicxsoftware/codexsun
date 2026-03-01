import { useEffect, useState } from "react"

import { blogApi, type BlogSearchQuery } from "@/features/blog/services/blog-api"
import type { BlogSearchResponse } from "@/features/blog/types/blog-types"

const empty: BlogSearchResponse = {
  data: [],
  pagination: {
    page: 1,
    pageSize: 12,
    totalItems: 0,
    totalPages: 1,
    hasPrevious: false,
    hasNext: false,
  },
}

export const useBlogSearch = (query: BlogSearchQuery) => {
  const [data, setData] = useState<BlogSearchResponse>(empty)
  const [loading, setLoading] = useState<boolean>(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const normalized = query.q.trim()
    if (normalized.length === 0) {
      setData(empty)
      setLoading(false)
      setError(null)
      return
    }

    let mounted = true
    const timer = window.setTimeout(() => {
      const run = async () => {
        try {
          setLoading(true)
          const response = await blogApi.search(query)
          if (!mounted) {
            return
          }
          setData(response)
          setError(null)
        } catch {
          if (!mounted) {
            return
          }
          setError("Search request failed.")
        } finally {
          if (mounted) {
            setLoading(false)
          }
        }
      }

      void run()
    }, 300)

    return () => {
      mounted = false
      window.clearTimeout(timer)
    }
  }, [query])

  return { data, loading, error }
}
