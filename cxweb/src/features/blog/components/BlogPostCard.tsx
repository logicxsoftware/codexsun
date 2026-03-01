import { motion } from "framer-motion"
import { Link } from "react-router"

import type { BlogPostListItem } from "@/features/blog/types/blog-types"

type BlogPostCardProps = {
  post: BlogPostListItem
}

export default function BlogPostCard({ post }: BlogPostCardProps) {
  return (
    <motion.article
      initial={{ opacity: 0, y: 18 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true, amount: 0.2 }}
      transition={{ duration: 0.35, ease: "easeOut" }}
      className="overflow-hidden rounded-xl border border-border bg-card"
    >
      <Link to={`/blog/${post.slug}`} className="block">
        {post.featuredImage ? <img src={post.featuredImage} alt={post.title} className="h-48 w-full object-cover" loading="lazy" /> : null}
        <div className="space-y-3 p-4">
          <p className="text-xs text-muted-foreground">{post.categoryName}</p>
          <h3 className="text-lg font-semibold text-card-foreground">{post.title}</h3>
          {post.excerpt ? <p className="text-sm text-muted-foreground line-clamp-3">{post.excerpt}</p> : null}
          <div className="flex flex-wrap gap-2">
            {post.tags.map((tag) => (
              <span key={tag} className="rounded-full border border-border px-2 py-1 text-xs text-muted-foreground">
                {tag}
              </span>
            ))}
          </div>
        </div>
      </Link>
    </motion.article>
  )
}
