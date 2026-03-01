import { useEffect, useState } from "react"

import { blogApi } from "@/features/blog/services/blog-api"
import type { BlogPostDetailResponse } from "@/features/blog/types/blog-types"
import { HttpError } from "@/shared/services/http-client"

export const useBlogPost = (slug: string | undefined) => {
  const [data, setData] = useState<BlogPostDetailResponse | null>(null)
  const [loading, setLoading] = useState<boolean>(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!slug) {
      setLoading(false)
      setError("Post slug is required.")
      return
    }

    let mounted = true

    const load = async () => {
      try {
        setLoading(true)
        const response = await blogApi.getPostBySlug(slug)
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
          setError("Blog post not found.")
        } else {
          setError("Failed to load blog post.")
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
  }, [slug])

  return { data, loading, error, setData }
}
