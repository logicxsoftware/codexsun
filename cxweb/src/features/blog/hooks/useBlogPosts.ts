import { useEffect, useState } from "react"

import { blogApi, type BlogListQuery } from "@/features/blog/services/blog-api"
import type { BlogPostListResponse } from "@/features/blog/types/blog-types"
import { HttpError } from "@/shared/services/http-client"

const empty: BlogPostListResponse = {
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

export const useBlogPosts = (query: BlogListQuery) => {
  const [data, setData] = useState<BlogPostListResponse>(empty)
  const [loading, setLoading] = useState<boolean>(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let mounted = true

    const load = async () => {
      try {
        setLoading(true)
        const response = await blogApi.getPosts(query)
        if (!mounted) {
          return
        }

        setData(response)
        setError(null)
      } catch (err) {
        if (!mounted) {
          return
        }

        if (err instanceof HttpError && err.status === 404) {
          setError("Blog posts were not found.")
        } else {
          setError("Failed to load blog posts.")
        }
      } finally {
        if (mounted) {
          setLoading(false)
        }
      }
    }

    void load()

    return () => {
      mounted = false
    }
  }, [query])

  return { data, loading, error }
}
