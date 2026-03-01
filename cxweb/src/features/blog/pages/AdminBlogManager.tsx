import { useEffect, useMemo, useState } from "react"

import { blogApi } from "@/features/blog/services/blog-api"
import type { BlogCategoryItem, BlogPostListItem, BlogTagItem } from "@/features/blog/types/blog-types"
import { showToast } from "@/shared/components/ui/toast/showToast"

const toSlug = (value: string): string =>
  value
    .trim()
    .toLowerCase()
    .replace(/[^a-z0-9\s-]/g, "")
    .replace(/\s+/g, "-")

export default function AdminBlogManagerPage() {
  const [posts, setPosts] = useState<BlogPostListItem[]>([])
  const [categories, setCategories] = useState<BlogCategoryItem[]>([])
  const [tags, setTags] = useState<BlogTagItem[]>([])

  const [title, setTitle] = useState<string>("")
  const [excerpt, setExcerpt] = useState<string>("")
  const [body, setBody] = useState<string>("")
  const [featuredImage, setFeaturedImage] = useState<string>("")
  const [categoryId, setCategoryId] = useState<string>("")
  const [selectedTagIds, setSelectedTagIds] = useState<string[]>([])

  useEffect(() => {
    let mounted = true

    const load = async () => {
      const [postData, categoryData, tagData] = await Promise.all([
        blogApi.getPosts({ page: 1, pageSize: 20, sort: "newest" }),
        blogApi.getCategories(),
        blogApi.getTags(),
      ])

      if (!mounted) {
        return
      }

      setPosts(postData.data)
      setCategories(categoryData)
      setTags(tagData)
      if (!categoryId && categoryData.length > 0) {
        setCategoryId(categoryData[0].id)
      }
    }

    void load()

    return () => {
      mounted = false
    }
  }, [categoryId])

  const slug = useMemo(() => toSlug(title), [title])

  const submit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    if (!categoryId) {
      showToast({ title: "Select category", variant: "warning" })
      return
    }

    try {
      await blogApi.upsertPost({
        title,
        slug,
        excerpt,
        body,
        featuredImage,
        categoryId,
        metaKeywordsJson: JSON.stringify(["blog", "codexsun"]),
        published: true,
        active: true,
        tagIds: selectedTagIds,
        images: featuredImage ? [{ imagePath: featuredImage, sortOrder: 0 }] : [],
      })
      showToast({ title: "Post saved", variant: "success" })
      const refreshed = await blogApi.getPosts({ page: 1, pageSize: 20, sort: "newest" })
      setPosts(refreshed.data)
      setTitle("")
      setExcerpt("")
      setBody("")
      setFeaturedImage("")
      setSelectedTagIds([])
    } catch {
      showToast({ title: "Save failed", variant: "error" })
    }
  }

  const remove = async (id: string) => {
    try {
      await blogApi.deletePost(id)
      setPosts((current) => current.filter((post) => post.id !== id))
      showToast({ title: "Post deleted", variant: "success" })
    } catch {
      showToast({ title: "Delete failed", variant: "error" })
    }
  }

  return (
    <section className="bg-background py-12">
      <div className="mx-auto grid max-w-7xl gap-6 px-5 lg:grid-cols-[420px_minmax(0,1fr)]">
        <form onSubmit={submit} className="space-y-3 rounded-xl border border-border bg-card p-4">
          <h1 className="text-xl font-semibold text-card-foreground">Blog Admin CRUD</h1>
          <input value={title} onChange={(event) => setTitle(event.target.value)} placeholder="Title" className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground" />
          <input value={slug} readOnly placeholder="Slug" className="w-full rounded-lg border border-border bg-muted px-3 py-2 text-sm text-muted-foreground" />
          <textarea value={excerpt} onChange={(event) => setExcerpt(event.target.value)} placeholder="Excerpt" rows={3} className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground" />
          <textarea value={body} onChange={(event) => setBody(event.target.value)} placeholder="Body" rows={6} className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground" />
          <input value={featuredImage} onChange={(event) => setFeaturedImage(event.target.value)} placeholder="Featured image URL" className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground" />

          <select value={categoryId} onChange={(event) => setCategoryId(event.target.value)} className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground">
            {categories.map((category) => (
              <option key={category.id} value={category.id}>
                {category.name}
              </option>
            ))}
          </select>

          <div className="flex flex-wrap gap-2">
            {tags.map((tag) => {
              const selected = selectedTagIds.includes(tag.id)
              return (
                <button
                  key={tag.id}
                  type="button"
                  onClick={() =>
                    setSelectedTagIds((current) => (selected ? current.filter((id) => id !== tag.id) : [...current, tag.id]))
                  }
                  className={`rounded-full border px-2 py-1 text-xs ${selected ? "border-primary bg-primary text-primary-foreground" : "border-border text-muted-foreground"}`}
                >
                  {tag.name}
                </button>
              )
            })}
          </div>

          <button type="submit" className="rounded-lg bg-primary px-4 py-2 text-sm font-medium text-primary-foreground transition hover:bg-primary/90">
            Save Post
          </button>
        </form>

        <div className="space-y-3">
          {posts.map((post) => (
            <div key={post.id} className="flex items-center justify-between gap-3 rounded-xl border border-border bg-card p-3">
              <div>
                <p className="text-sm font-medium text-card-foreground">{post.title}</p>
                <p className="text-xs text-muted-foreground">/{post.slug}</p>
              </div>
              <button
                type="button"
                onClick={() => {
                  void remove(post.id)
                }}
                className="rounded-lg border border-border bg-background px-3 py-1.5 text-xs text-foreground transition hover:bg-muted"
              >
                Delete
              </button>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}
