export type BlogPagination = {
  page: number
  pageSize: number
  totalItems: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
}

export type BlogPostListItem = {
  id: string
  title: string
  slug: string
  excerpt?: string | null
  featuredImage?: string | null
  categoryName: string
  categorySlug: string
  createdAtUtc: string
  likeCount: number
  commentCount: number
  tags: string[]
}

export type BlogPostListResponse = {
  data: BlogPostListItem[]
  pagination: BlogPagination
}

export type BlogSearchItem = {
  id: string
  title: string
  slug: string
  excerpt?: string | null
  featuredImage?: string | null
  categorySlug: string
  createdAtUtc: string
  rank: number
  headline?: string | null
}

export type BlogSearchResponse = {
  data: BlogSearchItem[]
  pagination: BlogPagination
}

export type BlogCategoryItem = {
  id: string
  name: string
  slug: string
  active: boolean
}

export type BlogTagItem = {
  id: string
  name: string
  slug: string
  active: boolean
}

export type BlogCommentItem = {
  id: string
  postId: string
  userId: string
  body: string
  approved: boolean
  createdAtUtc: string
  updatedAtUtc: string
}

export type BlogLikeItem = {
  postId: string
  userId: string
  liked: boolean
  updatedAtUtc: string
}

export type BlogPostImageItem = {
  id: string
  imagePath: string
  altText?: string | null
  caption?: string | null
  sortOrder: number
}

export type BlogPostDetailResponse = {
  id: string
  title: string
  slug: string
  excerpt?: string | null
  body: string
  featuredImage?: string | null
  published: boolean
  active: boolean
  createdAtUtc: string
  updatedAtUtc: string
  categoryName: string
  categorySlug: string
  userId: string
  metaKeywords?: unknown
  tags: BlogTagItem[]
  comments: BlogCommentItem[]
  images: BlogPostImageItem[]
  likeCount: number
  relatedPosts: BlogPostListItem[]
}

export type BlogPostImageWriteRequest = {
  id?: string
  imagePath: string
  altText?: string
  caption?: string
  sortOrder: number
}

export type BlogPostWriteRequest = {
  title: string
  slug: string
  excerpt?: string
  body: string
  featuredImage?: string
  categoryId: string
  metaKeywordsJson?: string
  published: boolean
  active: boolean
  tagIds: string[]
  images: BlogPostImageWriteRequest[]
}
