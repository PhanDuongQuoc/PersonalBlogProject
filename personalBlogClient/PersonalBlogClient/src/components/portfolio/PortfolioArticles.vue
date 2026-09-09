<template>
  <div v-if="posts.length" class="writing-showcase-wrapper">
    <!-- 1. Lead Featured Essay Card -->
    <article v-if="posts[0]" class="lead-feature-card">
      <router-link :to="`/posts/${posts[0].slug}`" class="lead-media-column">
        <div class="lead-visual-frame">
          <img
            v-if="posts[0].thumbnailUrl"
            :src="posts[0].thumbnailUrl"
            :alt="posts[0].title"
            class="lead-thumbnail"
          />
          <div v-else class="lead-fallback-graphic">
            <q-icon name="terminal" size="48px" class="graphic-icon" />
          </div>
          <span class="lead-category-badge">{{ posts[0].category }}</span>
        </div>
      </router-link>

      <div class="lead-info-column">
        <div class="lead-meta-row">
          <span class="lead-meta-kicker">FEATURED ESSAY</span>
          <span class="meta-dot">/</span>
          <span class="lead-date">{{ formatDate(posts[0].publishedAt) }}</span>
        </div>

        <h3 class="lead-headline">
          <router-link :to="`/posts/${posts[0].slug}`" class="headline-link">
            {{ posts[0].title }}
          </router-link>
        </h3>

        <p class="lead-excerpt">
          {{ posts[0].excerpt || text.readLatest }}
        </p>

        <div class="lead-action-row">
          <router-link :to="`/posts/${posts[0].slug}`" class="read-essay-btn">
            <span>{{ text.viewPosts }}</span>
            <q-icon name="arrow_forward" size="14px" class="btn-arrow" />
          </router-link>
        </div>
      </div>
    </article>

    <!-- 2. Secondary Editorial Notes Grid -->
    <div v-if="posts.length > 1" class="secondary-articles-grid">
      <article
        v-for="post in posts.slice(1)"
        :key="post.id"
        class="secondary-article-card"
      >
        <div class="secondary-card-meta">
          <span class="secondary-category">{{ post.category }}</span>
          <span class="meta-dot">/</span>
          <span class="secondary-date">{{ formatDate(post.publishedAt) }}</span>
        </div>

        <h4 class="secondary-headline">
          <router-link :to="`/posts/${post.slug}`" class="secondary-title-link">
            {{ post.title }}
          </router-link>
        </h4>

        <p class="secondary-excerpt">
          {{ post.excerpt || text.readLatest }}
        </p>

        <div class="secondary-footer">
          <router-link :to="`/posts/${post.slug}`" class="secondary-read-link">
            <span>{{ text.readArticle || 'Read note' }}</span>
            <q-icon name="arrow_forward" size="13px" class="card-arrow" />
          </router-link>
        </div>
      </article>
    </div>
  </div>

  <div v-else class="empty-writing-state">
    <q-icon name="edit_note" size="36px" />
    <p>{{ text.articlesPreparing }}</p>
  </div>
</template>

<script setup lang="ts">
import type { PublicPost } from '@/types/public-landing';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';

defineProps<{ posts: PublicPost[] }>();
const { locale, text } = usePortfolioLocale();

const formatDate = (date: string | null) =>
  date
    ? new Intl.DateTimeFormat(locale.value === 'vi' ? 'vi-VN' : 'en', {
        month: 'short',
        day: 'numeric',
        year: 'numeric'
      }).format(new Date(date))
    : text.value.recent;
</script>

<style scoped lang="scss">
.writing-showcase-wrapper {
  margin-top: 32px;
  display: flex;
  flex-direction: column;
  gap: 28px;
}

/* 1. Lead Featured Post */
.lead-feature-card {
  display: grid;
  grid-template-columns: 1.1fr 1.2fr;
  gap: 36px;
  align-items: center;
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-xl);
  padding: 24px;
  box-shadow: var(--shadow-card);
  transition: all 0.3s ease;

  &:hover {
    border-color: rgba(16, 185, 129, 0.35);
    box-shadow: var(--shadow-card-hover);
  }
}

.lead-media-column {
  display: block;
  text-decoration: none;
}

.lead-visual-frame {
  position: relative;
  width: 100%;
  height: 240px;
  border-radius: var(--radius-lg);
  overflow: hidden;
  background: var(--bg-surface-lowest);
}

.lead-thumbnail {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;

  .lead-feature-card:hover & {
    transform: scale(1.03);
  }
}

.lead-fallback-graphic {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, var(--bg-surface-lowest), var(--bg-surface-container));
  color: var(--accent-primary);
}

.lead-category-badge {
  position: absolute;
  top: 14px;
  left: 14px;
  font-family: var(--font-mono);
  background: rgba(11, 19, 38, 0.85);
  backdrop-filter: blur(8px);
  color: var(--accent-primary);
  border: 1px solid rgba(16, 185, 129, 0.3);
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.06em;
  padding: 4px 10px;
  border-radius: var(--radius-sm);
  text-transform: uppercase;
}

.lead-info-column {
  display: flex;
  flex-direction: column;
}

.lead-meta-row {
  display: flex;
  align-items: center;
  gap: 8px;
  font-family: var(--font-mono);
  font-size: 0.72rem;
  margin-bottom: 12px;
}

.lead-meta-kicker {
  font-weight: 700;
  letter-spacing: 0.08em;
  color: var(--accent-primary);
}

.meta-dot {
  color: var(--text-muted);
}

.lead-date {
  color: var(--text-secondary);
}

.lead-headline {
  font-family: var(--font-headline);
  font-size: clamp(1.4rem, 2.5vw, 1.85rem);
  font-weight: 800;
  line-height: 1.25;
  margin: 0 0 14px;
  letter-spacing: -0.02em;
}

.headline-link {
  color: var(--text-primary);
  text-decoration: none;
  transition: color 0.2s ease;

  &:hover {
    color: var(--accent-primary);
  }
}

.lead-excerpt {
  font-family: var(--font-body);
  font-size: 0.95rem;
  line-height: 1.65;
  color: var(--text-secondary);
  margin: 0 0 20px;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
}

.lead-action-row {
  display: flex;
}

.read-essay-btn {
  font-family: var(--font-headline);
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 0.88rem;
  font-weight: 700;
  color: var(--accent-primary);
  text-decoration: none;
  transition: all 0.2s ease;

  .btn-arrow {
    transition: transform 0.2s ease;
  }

  &:hover {
    color: var(--accent-primary-hover);

    .btn-arrow {
      transform: translateX(4px);
    }
  }
}

/* 2. Secondary Articles Grid */
.secondary-articles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(min(100%, 300px), 1fr));
  gap: 20px;
}

.secondary-article-card {
  display: flex;
  flex-direction: column;
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-lg);
  padding: 22px;
  box-shadow: var(--shadow-card);
  transition: all 0.25s ease;

  &:hover {
    border-color: rgba(16, 185, 129, 0.3);
    transform: translateY(-2px);
    box-shadow: var(--shadow-card-hover);
  }
}

.secondary-card-meta {
  display: flex;
  align-items: center;
  gap: 6px;
  font-family: var(--font-mono);
  font-size: 0.72rem;
  margin-bottom: 10px;
}

.secondary-category {
  color: var(--accent-primary);
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}

.secondary-date {
  color: var(--text-muted);
}

.secondary-headline {
  font-family: var(--font-headline);
  font-size: 1.12rem;
  font-weight: 700;
  line-height: 1.35;
  margin: 0 0 10px;
  letter-spacing: -0.015em;
}

.secondary-title-link {
  color: var(--text-primary);
  text-decoration: none;
  transition: color 0.2s ease;

  &:hover {
    color: var(--accent-primary);
  }
}

.secondary-excerpt {
  font-family: var(--font-body);
  font-size: 0.88rem;
  line-height: 1.6;
  color: var(--text-secondary);
  margin: 0 0 16px;
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.secondary-footer {
  display: flex;
  justify-content: flex-end;
}

.secondary-read-link {
  font-family: var(--font-headline);
  display: inline-flex;
  align-items: center;
  gap: 4px;
  font-size: 0.82rem;
  font-weight: 600;
  color: var(--text-muted);
  text-decoration: none;
  transition: all 0.2s ease;

  .card-arrow {
    transition: transform 0.2s ease;
  }

  &:hover {
    color: var(--accent-primary);

    .card-arrow {
      transform: translateX(3px);
    }
  }
}

/* Empty State */
.empty-writing-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 48px 20px;
  color: var(--text-muted);
  gap: 12px;
  text-align: center;
}

@media (max-width: 860px) {
  .lead-feature-card {
    grid-template-columns: 1fr;
    gap: 20px;
    padding: 18px;
  }
  .lead-visual-frame {
    height: clamp(180px, 45vw, 220px);
  }
}
</style>
