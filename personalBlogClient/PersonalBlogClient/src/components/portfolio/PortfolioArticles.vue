<template>
  <div v-if="posts.length" class="writing-showcase-wrapper">
    <!-- 1. Lead Featured Post (Prominent Hero Feature) -->
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
          <span class="meta-dot">·</span>
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
            <q-icon name="arrow_forward" size="14px" />
          </router-link>
        </div>
      </div>
    </article>

    <!-- 2. Secondary Editorial Articles Grid -->
    <div v-if="posts.length > 1" class="secondary-articles-grid">
      <article
        v-for="post in posts.slice(1)"
        :key="post.id"
        class="secondary-article-card"
      >
        <div class="secondary-card-meta">
          <span class="secondary-category">{{ post.category }}</span>
          <span class="meta-dot">·</span>
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
            <span>Read note</span>
            <q-icon name="arrow_forward" size="13px" />
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
  margin-top: 36px;
  display: flex;
  flex-direction: column;
  gap: 32px;
}

/* 1. Lead Featured Post */
.lead-feature-card {
  display: grid;
  grid-template-columns: 1.1fr 1.2fr;
  gap: 36px;
  align-items: center;
  background: rgba(15, 23, 42, 0.5);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 12px;
  padding: 24px;
  transition: all 0.3s ease;

  &:hover {
    border-color: rgba(70, 224, 175, 0.3);
    box-shadow: 0 16px 36px rgba(0, 0, 0, 0.3);
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
  border-radius: 8px;
  overflow: hidden;
  background: #091222;
}

.lead-thumbnail {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;

  .lead-feature-card:hover & {
    transform: scale(1.04);
  }
}

.lead-fallback-graphic {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #091222, #132238);
  color: #46e0af;
}

.lead-category-badge {
  position: absolute;
  top: 14px;
  left: 14px;
  background: rgba(8, 17, 38, 0.85);
  backdrop-filter: blur(6px);
  color: #46e0af;
  border: 1px solid rgba(70, 224, 175, 0.3);
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.06em;
  padding: 4px 10px;
  border-radius: 4px;
}

.lead-info-column {
  display: flex;
  flex-direction: column;
}

.lead-meta-row {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.75rem;
  margin-bottom: 12px;
}

.lead-meta-kicker {
  font-weight: 700;
  letter-spacing: 0.08em;
  color: #46e0af;
}

.meta-dot {
  color: #475569;
}

.lead-date {
  color: #94a3b8;
}

.lead-headline {
  font-size: clamp(1.4rem, 2.5vw, 1.85rem);
  font-weight: 800;
  line-height: 1.25;
  margin: 0 0 14px;
  letter-spacing: -0.02em;
}

.headline-link {
  color: #f8fafc;
  text-decoration: none;
  transition: color 0.2s ease;

  &:hover {
    color: #46e0af;
  }
}

.lead-excerpt {
  font-size: 0.95rem;
  line-height: 1.6;
  color: #94a3b8;
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
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 0.875rem;
  font-weight: 700;
  color: #46e0af;
  text-decoration: none;
  transition: transform 0.2s ease;

  &:hover {
    transform: translateX(4px);
  }
}

/* 2. Secondary Articles Grid */
.secondary-articles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(min(100%, 280px), 1fr));
  gap: 20px;
}

.secondary-article-card {
  display: flex;
  flex-direction: column;
  background: rgba(15, 23, 42, 0.35);
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-radius: 8px;
  padding: 22px;
  transition: all 0.25s ease;

  &:hover {
    border-color: rgba(70, 224, 175, 0.25);
    background: rgba(15, 23, 42, 0.5);
  }
}

.secondary-card-meta {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 0.75rem;
  margin-bottom: 10px;
}

.secondary-category {
  color: #46e0af;
  font-weight: 700;
}

.secondary-date {
  color: #64748b;
}

.secondary-headline {
  font-size: 1.12rem;
  font-weight: 700;
  line-height: 1.35;
  margin: 0 0 10px;
}

.secondary-title-link {
  color: #f1f5f9;
  text-decoration: none;
  transition: color 0.2s ease;

  &:hover {
    color: #46e0af;
  }
}

.secondary-excerpt {
  font-size: 0.875rem;
  line-height: 1.55;
  color: #94a3b8;
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
  display: inline-flex;
  align-items: center;
  gap: 4px;
  font-size: 0.8rem;
  font-weight: 600;
  color: #64748b;
  text-decoration: none;
  transition: all 0.2s ease;

  &:hover {
    color: #46e0af;
    transform: translateX(3px);
  }
}

/* Empty State */
.empty-writing-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 48px 20px;
  color: #64748b;
  gap: 12px;
  text-align: center;
}

@media (max-width: 860px) {
  .writing-showcase-wrapper {
    gap: 24px;
    margin-top: 24px;
  }
  .lead-feature-card {
    grid-template-columns: 1fr;
    gap: 20px;
    padding: 18px;
  }
  .lead-visual-frame {
    height: clamp(180px, 45vw, 220px);
  }
}

@media (max-width: 500px) {
  .lead-feature-card {
    padding: 14px;
    border-radius: 8px;
  }
  .lead-headline {
    font-size: 1.25rem;
    margin-bottom: 10px;
  }
  .lead-excerpt {
    font-size: 0.88rem;
    margin-bottom: 14px;
  }
  .secondary-article-card {
    padding: 16px;
  }
}
</style>

<!-- Global Light Mode overrides -->
<style lang="scss">
body.portfolio-light {
  .lead-feature-card {
    background: #ffffff !important;
    border-color: #e2e8f0 !important;
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05) !important;

    &:hover {
      border-color: rgba(15, 159, 116, 0.35) !important;
      box-shadow: 0 16px 36px rgba(0, 0, 0, 0.08) !important;
    }
  }

  .lead-category-badge {
    background: #ffffff !important;
    color: #0f9f74 !important;
    border-color: rgba(15, 159, 116, 0.3) !important;
  }

  .lead-meta-kicker {
    color: #0f9f74 !important;
  }

  .headline-link {
    color: #0f172a !important;
    &:hover {
      color: #0f9f74 !important;
    }
  }

  .lead-excerpt {
    color: #475569 !important;
  }

  .read-essay-btn {
    color: #0f9f74 !important;
  }

  .secondary-article-card {
    background: #ffffff !important;
    border-color: #e2e8f0 !important;

    &:hover {
      border-color: rgba(15, 159, 116, 0.3) !important;
      background: #f8fafc !important;
    }
  }

  .secondary-category {
    color: #0f9f74 !important;
  }

  .secondary-title-link {
    color: #0f172a !important;
    &:hover {
      color: #0f9f74 !important;
    }
  }

  .secondary-excerpt {
    color: #475569 !important;
  }

  .secondary-read-link {
    color: #64748b !important;
    &:hover {
      color: #0f9f74 !important;
    }
  }
}
</style>
