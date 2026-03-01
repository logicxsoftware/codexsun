import type { ReactElement } from "react"
import { useState } from "react"
import { Link, useParams } from "react-router"

import CommentForm from "@/features/blog/components/CommentForm"
import ImageGallery from "@/features/blog/components/ImageGallery"
import LikeButton from "@/features/blog/components/LikeButton"
import { useBlogPost } from "@/features/blog/hooks/useBlogPost"
import { blogApi } from "@/features/blog/services/blog-api"
import { showToast } from "@/shared/components/ui/toast/showToast"

const renderBody = (value: string): ReactElement => {
  const looksLikeHtml = value.includes("<") && value.includes(">")
  if (looksLikeHtml) {
    return <div className="prose prose-sm max-w-none text-foreground" dangerouslySetInnerHTML={{ __html: value }} />
  }

  return <p className="whitespace-pre-wrap text-sm leading-7 text-foreground">{value}</p>
}

export default function BlogPostPage() {
  const params = useParams()
  const { data, loading, error, setData } = useBlogPost(params.slug)
  const [commentBusy, setCommentBusy] = useState<boolean>(false)

  const submitComment = async (body: string) => {
    if (!data) {
      return
    }

    setCommentBusy(true)
    try {
      const created = await blogApi.submitComment(data.id, body)
      setData({
        ...data,
        comments: [created, ...data.comments],
      })
    } finally {
      setCommentBusy(false)
    }
  }

  const toggleLike = async (liked: boolean) => {
    if (!data) {
      return
    }

    try {
      await blogApi.setLike(data.id, liked)
      setData({
        ...data,
        likeCount: Math.max(0, data.likeCount + (liked ? 1 : -1)),
      })
    } catch {
      showToast({ title: "Login required for likes", variant: "warning" })
      throw new Error("Like failed")
    }
  }

  if (loading) {
    return <section className="mx-auto max-w-5xl px-5 py-12"><div className="h-72 animate-pulse rounded-xl border border-border bg-card" /></section>
  }

  if (error || !data) {
    return <section className="mx-auto max-w-5xl px-5 py-12"><p className="rounded-xl border border-border bg-card p-6 text-sm text-muted-foreground">{error ?? "Post not available."}</p></section>
  }

  return (
    <section className="bg-background py-12">
      <div className="mx-auto grid max-w-6xl gap-8 px-5 lg:grid-cols-[minmax(0,1fr)_320px]">
        <article className="space-y-6">
          <header className="space-y-3">
            <p className="text-xs text-muted-foreground">{data.categoryName}</p>
            <h1 className="text-3xl font-semibold text-foreground">{data.title}</h1>
            {data.excerpt ? <p className="text-sm text-muted-foreground">{data.excerpt}</p> : null}
          </header>

          {data.featuredImage ? <img src={data.featuredImage} alt={data.title} className="w-full rounded-xl border border-border object-cover" /> : null}

          {renderBody(data.body)}

          <ImageGallery images={data.images} />

          <div className="flex items-center gap-3">
            <LikeButton initialCount={data.likeCount} onToggle={toggleLike} />
            <span className="text-sm text-muted-foreground">{data.comments.length} comments</span>
          </div>

          <CommentForm
            onSubmit={async (value) => {
              if (commentBusy) {
                return
              }

              await submitComment(value)
            }}
          />

          <div className="space-y-3">
            {data.comments.map((comment) => (
              <div key={comment.id} className="rounded-xl border border-border bg-card p-4">
                <p className="text-sm text-card-foreground">{comment.body}</p>
                <p className="mt-2 text-xs text-muted-foreground">{new Date(comment.createdAtUtc).toLocaleString()}</p>
              </div>
            ))}
          </div>
        </article>

        <aside className="space-y-4">
          <div className="rounded-xl border border-border bg-card p-4">
            <h2 className="mb-3 text-sm font-semibold text-card-foreground">Tags</h2>
            <div className="flex flex-wrap gap-2">
              {data.tags.map((tag) => (
                <Link key={tag.id} to={`/blog?tag=${encodeURIComponent(tag.slug)}`} className="rounded-full border border-border px-2 py-1 text-xs text-muted-foreground">
                  {tag.name}
                </Link>
              ))}
            </div>
          </div>

          <div className="rounded-xl border border-border bg-card p-4">
            <h2 className="mb-3 text-sm font-semibold text-card-foreground">Related posts</h2>
            <div className="space-y-3">
              {data.relatedPosts.map((post) => (
                <Link key={post.id} to={`/blog/${post.slug}`} className="block rounded-lg border border-border px-3 py-2 text-sm text-card-foreground transition hover:bg-muted">
                  {post.title}
                </Link>
              ))}
            </div>
          </div>
        </aside>
      </div>
    </section>
  )
}
