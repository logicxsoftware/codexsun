import { httpClient } from "@/shared/services/http-client"
import type {
  BlogCategoryItem,
  BlogCommentItem,
  BlogLikeItem,
  BlogPostDetailResponse,
  BlogPostListResponse,
  BlogPostWriteRequest,
  BlogSearchResponse,
  BlogTagItem,
} from "@/features/blog/types/blog-types"

export type BlogListQuery = {
  category?: string
  tag?: string
  sort?: "newest" | "oldest"
  page?: number
  pageSize?: number
}

export type BlogSearchQuery = {
  q: string
  category?: string
  tag?: string
  sort?: "relevance" | "newest" | "oldest"
  page?: number
  pageSize?: number
}

const queryString = (pairs: Record<string, string | number | undefined>): string => {
  const params = new URLSearchParams()
  for (const [key, value] of Object.entries(pairs)) {
    if (typeof value === "undefined") {
      continue
    }

    const normalized = String(value).trim()
    if (normalized.length === 0) {
      continue
    }

    params.set(key, normalized)
  }

  const output = params.toString()
  return output.length > 0 ? `?${output}` : ""
}

export const blogApi = {
  getCategories(): Promise<BlogCategoryItem[]> {
    return httpClient.get<BlogCategoryItem[]>("/blog/categories")
  },

  getTags(): Promise<BlogTagItem[]> {
    return httpClient.get<BlogTagItem[]>("/blog/tags")
  },

  getPosts(query: BlogListQuery): Promise<BlogPostListResponse> {
    const suffix = queryString({
      category: query.category,
      tag: query.tag,
      sort: query.sort,
      page: query.page,
      pageSize: query.pageSize,
    })
    return httpClient.get<BlogPostListResponse>(`/blog/posts${suffix}`)
  },

  getPostBySlug(slug: string): Promise<BlogPostDetailResponse> {
    return httpClient.get<BlogPostDetailResponse>(`/blog/posts/${encodeURIComponent(slug)}`)
  },

  search(query: BlogSearchQuery): Promise<BlogSearchResponse> {
    const suffix = queryString({
      q: query.q,
      category: query.category,
      tag: query.tag,
      sort: query.sort,
      page: query.page,
      pageSize: Math.min(50, query.pageSize ?? 12),
    })
    return httpClient.get<BlogSearchResponse>(`/blog/search${suffix}`)
  },

  upsertPost(body: BlogPostWriteRequest, id?: string): Promise<BlogPostDetailResponse> {
    if (id) {
      return httpClient.put<BlogPostDetailResponse>(`/blog/posts/${encodeURIComponent(id)}`, body)
    }

    return httpClient.post<BlogPostDetailResponse>("/blog/posts", body)
  },

  deletePost(id: string): Promise<void> {
    return httpClient.delete<void>(`/blog/posts/${encodeURIComponent(id)}`)
  },

  submitComment(postId: string, body: string): Promise<BlogCommentItem> {
    return httpClient.post<BlogCommentItem>("/blog/comments", { postId, body, approved: true })
  },

  setLike(postId: string, liked: boolean): Promise<BlogLikeItem> {
    return httpClient.post<BlogLikeItem>("/blog/likes", { postId, liked })
  },
}
