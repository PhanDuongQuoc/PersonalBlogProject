<template>
  <q-page class="portfolio-page">
    <main class="portfolio-content">
      <!-- 1. Breadcrumb -->
      <nav class="topic-detail-breadcrumb">
        <router-link to="/home" class="bc-link">
          <q-icon name="arrow_back" size="16px" />
          <span>{{ text.home }}</span>
        </router-link>
        <span class="bc-separator">/</span>
        <router-link to="/topics" class="bc-link">
          <span>{{ text.topics }}</span>
        </router-link>
        <span class="bc-separator">/</span>
        <span class="bc-current">{{ detail?.topic.name || text.loading }}</span>
      </nav>

      <!-- 2. Loading State -->
      <div v-if="loading" class="load-state">
        <q-spinner color="teal-4" size="44px" />
        <p>{{ text.loading }}</p>
      </div>

      <!-- 3. Error / 404 State -->
      <div v-else-if="errorMessage || !detail" class="load-state error-state">
        <q-icon name="error_outline" size="50px" color="negative" />
        <p>{{ errorMessage || text.loadFailed }}</p>
        <router-link to="/topics" class="back-topics-btn">
          <q-icon name="arrow_back" size="16px" class="q-mr-xs" />
          {{ text.viewAllTopics }}
        </router-link>
      </div>

      <!-- 4. Main Topic Detail Content -->
      <div v-else class="topic-detail-body">
        <!-- Topic Hero Banner -->
        <header class="topic-hero-banner">
          <div class="banner-top-row">
            <div class="topic-eyebrow">
              <q-icon :name="getTopicIcon(detail.topic.slug)" size="18px" class="eyebrow-icon" />
              <span>TOPIC · {{ detail.topic.totalPosts }} {{ text.topicArticlesCount }}</span>
            </div>
            <span class="topic-slug-badge">/topics/{{ detail.topic.slug }}</span>
          </div>

          <h1 class="topic-main-title">{{ detail.topic.name }}</h1>

          <p class="topic-main-desc">
            {{ detail.topic.description || (locale === 'vi' ? 'Tổng hợp tất cả các bài viết, ghi chú chuyên sâu và phân tích thực tiễn thuộc chủ đề ' + detail.topic.name + '.' : 'All technical articles, notes and best practices filed under ' + detail.topic.name + '.') }}
          </p>

          <div class="topic-stats-row">
            <div class="stat-badge">
              <q-icon name="article" size="16px" />
              <span><strong>{{ detail.topic.totalPosts }}</strong> {{ text.publishedPostsLabel }}</span>
            </div>
            <div class="stat-badge">
              <q-icon name="visibility" size="16px" />
              <span><strong>{{ detail.topic.totalViews.toLocaleString() }}</strong> {{ text.totalViewsLabel }}</span>
            </div>
          </div>
        </header>

        <!-- Search & Filter Controls -->
        <section class="topic-filter-toolbar">
          <div class="toolbar-search-box">
            <q-icon name="search" size="18px" class="search-ico" />
            <input
              v-model="searchQuery"
              type="text"
              class="toolbar-input"
              :placeholder="text.searchPostsInTopicPlaceholder"
              @keyup.enter="handleFilterChange"
            />
            <button
              v-if="searchQuery"
              class="clear-search-btn"
              type="button"
              @click="clearSearch"
            >
              <q-icon name="close" size="14px" />
            </button>
          </div>

          <div class="toolbar-sort-box">
            <span class="sort-title">{{ text.sortBy }}:</span>
            <q-select
              v-model="sortBy"
              dense
              outlined
              emit-value
              map-options
              :options="sortOptions"
              class="sort-dropdown"
              @update:model-value="handleFilterChange"
            />
          </div>
        </section>

        <!-- Tag Filter Chips -->
        <section v-if="detail.availableTags && detail.availableTags.length > 0" class="tag-filter-section">
          <div class="tag-filter-list">
            <button
              type="button"
              class="tag-chip-btn"
              :class="{ active: selectedTag === '' }"
              @click="selectTag('')"
            >
              {{ text.allTags }}
            </button>
            <button
              v-for="tag in detail.availableTags"
              :key="tag.id"
              type="button"
              class="tag-chip-btn"
              :class="{ active: selectedTag === tag.slug }"
              @click="selectTag(tag.slug)"
            >
              #{{ tag.name }}
            </button>
          </div>
        </section>

        <!-- Articles Grid -->
        <section v-if="detail.posts.items.length > 0" class="articles-grid-container">
          <div class="posts-grid">
            <article
              v-for="post in detail.posts.items"
              :key="post.id"
              class="article-card"
            >
              <!-- Thumbnail -->
              <router-link :to="`/posts/${post.slug}`" class="article-thumb-wrapper">
                <img
                  v-if="post.thumbnailUrl"
                  :src="post.thumbnailUrl"
                  :alt="post.title"
                  class="article-thumb-img"
                  loading="lazy"
                />
                <div v-else class="article-thumb-placeholder">
                  <q-icon :name="getTopicIcon(detail.topic.slug)" size="40px" class="thumb-icon" />
                  <span class="thumb-topic-name">{{ detail.topic.name }}</span>
                </div>
              </router-link>

              <div class="article-card-body">
                <!-- Meta Row -->
                <div class="article-meta-row">
                  <span v-if="post.publishedAt" class="meta-date">
                    <q-icon name="event" size="13px" />
                    {{ formatDate(post.publishedAt) }}
                  </span>
                  <span class="meta-dot">/</span>
                  <span class="meta-views">
                    <q-icon name="visibility" size="13px" />
                    {{ post.viewCount }} {{ text.viewsCount }}
                  </span>
                </div>

                <!-- Title -->
                <h3 class="article-title">
                  <router-link :to="`/posts/${post.slug}`" class="article-link">
                    {{ post.title }}
                  </router-link>
                </h3>

                <!-- Excerpt -->
                <p v-if="post.excerpt" class="article-excerpt">
                  {{ post.excerpt }}
                </p>

                <!-- Tags List -->
                <div v-if="post.tags && post.tags.length > 0" class="article-tags-wrap">
                  <span
                    v-for="tag in post.tags"
                    :key="tag.id"
                    class="tag-pill"
                    @click.stop="selectTag(tag.slug)"
                  >
                    #{{ tag.name }}
                  </span>
                </div>

                <!-- Action Footer -->
                <div class="article-card-footer">
                  <router-link :to="`/posts/${post.slug}`" class="read-more-link">
                    <span>{{ text.readArticle }}</span>
                    <q-icon name="arrow_forward" size="14px" class="read-icon" />
                  </router-link>
                </div>
              </div>
            </article>
          </div>

          <!-- Pagination -->
          <div v-if="detail.posts.totalPages > 1" class="pagination-wrapper">
            <q-pagination
              v-model="currentPage"
              :max="detail.posts.totalPages"
              :max-pages="6"
              direction-links
              boundary-links
              color="teal-4"
              active-color="teal-4"
              class="custom-pagination"
              @update:model-value="onPageChange"
            />
          </div>
        </section>

        <!-- Empty Results in Topic -->
        <div v-else class="topic-empty-box">
          <q-icon name="article" size="48px" class="empty-ico" />
          <h3>{{ text.noPostsInTopic }}</h3>
          <p>{{ text.clearFilters }}</p>
          <q-btn
            v-if="searchQuery || selectedTag"
            unelevated
            no-caps
            class="reset-filters-btn"
            @click="resetAllFilters"
          >
            <q-icon name="restart_alt" size="16px" class="q-mr-xs" />
            {{ text.clearFilters }}
          </q-btn>
        </div>

        <!-- Other Topics Suggestion Grid -->
        <section v-if="detail.otherTopics && detail.otherTopics.length > 0" class="other-topics-section">
          <div class="other-header">
            <div>
              <h2 class="other-title">{{ text.otherTopics }}</h2>
              <p class="other-desc">{{ text.otherTopicsDesc }}</p>
            </div>
            <router-link to="/topics" class="view-all-link">
              <span>{{ text.viewAllTopics }}</span>
              <q-icon name="arrow_forward" size="16px" class="link-arrow" />
            </router-link>
          </div>

          <div class="other-topics-grid">
            <router-link
              v-for="other in detail.otherTopics"
              :key="other.id"
              :to="`/topics/${other.slug}`"
              class="other-topic-card"
            >
              <div class="other-icon-wrap">
                <q-icon :name="getTopicIcon(other.slug)" size="18px" />
              </div>
              <div class="other-info">
                <h4>{{ other.name }}</h4>
                <span>{{ other.postCount }} {{ text.topicArticlesCount }}</span>
              </div>
              <q-icon name="chevron_right" size="18px" class="other-arrow" />
            </router-link>
          </div>
        </section>
      </div>
    </main>

    <portfolio-footer :email="authorEmail" />
  </q-page>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useRoute } from 'vue-router';
import api from '@/boot/ApiGateway/axios';
import PortfolioFooter from '@/components/portfolio/PortfolioFooter.vue';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
import type { PublicTopicDetailResponse } from '@/types/public-topic';

const route = useRoute();
const { locale, text } = usePortfolioLocale();

const detail = ref<PublicTopicDetailResponse | null>(null);
const loading = ref(true);
const errorMessage = ref('');

const searchQuery = ref('');
const sortBy = ref<'latest' | 'popular' | 'oldest'>('latest');
const selectedTag = ref('');
const currentPage = ref(1);
const pageSize = 6;
const authorEmail = ref<string | null>(null);

const sortOptions = computed(() => [
  { label: locale.value === 'vi' ? 'Mới nhất' : 'Newest first', value: 'latest' },
  { label: locale.value === 'vi' ? 'Nhiều lượt xem nhất' : 'Most viewed', value: 'popular' },
  { label: locale.value === 'vi' ? 'Cũ nhất' : 'Oldest first', value: 'oldest' }
]);

const fetchTopicDetail = async () => {
  const slug = route.params.slug as string;
  if (!slug) return;

  loading.value = true;
  errorMessage.value = '';

  try {
    const username = typeof route.query.username === 'string' ? route.query.username : undefined;
    const response = await api.get<PublicTopicDetailResponse>(`/public/topics/${slug}`, {
      params: {
        username,
        page: currentPage.value,
        pageSize,
        sortBy: sortBy.value,
        search: searchQuery.value.trim() || undefined,
        tag: selectedTag.value || undefined
      }
    });
    detail.value = response.data;
  } catch {
    errorMessage.value = text.value.loadFailed;
  } finally {
    loading.value = false;
  }
};

const handleFilterChange = () => {
  currentPage.value = 1;
  fetchTopicDetail();
};

const clearSearch = () => {
  searchQuery.value = '';
  handleFilterChange();
};

const selectTag = (tagSlug: string) => {
  selectedTag.value = tagSlug;
  handleFilterChange();
};

const resetAllFilters = () => {
  searchQuery.value = '';
  selectedTag.value = '';
  sortBy.value = 'latest';
  handleFilterChange();
};

const onPageChange = (newPage: number) => {
  currentPage.value = newPage;
  fetchTopicDetail();
  window.scrollTo({ top: 350, behavior: 'smooth' });
};

const getTopicIcon = (slug: string): string => {
  const s = (slug || '').toLowerCase();
  if (s.includes('vue') || s.includes('react') || s.includes('angular') || s.includes('frontend') || s.includes('web')) {
    return 'code';
  }
  if (s.includes('net') || s.includes('csharp') || s.includes('backend') || s.includes('api') || s.includes('node') || s.includes('java')) {
    return 'terminal';
  }
  if (s.includes('sql') || s.includes('data') || s.includes('db') || s.includes('postgres') || s.includes('mongo')) {
    return 'storage';
  }
  if (s.includes('devops') || s.includes('docker') || s.includes('cloud') || s.includes('k8s') || s.includes('ci')) {
    return 'cloud';
  }
  if (s.includes('architecture') || s.includes('pattern') || s.includes('system') || s.includes('design')) {
    return 'hub';
  }
  if (s.includes('security') || s.includes('auth') || s.includes('jwt')) {
    return 'security';
  }
  if (s.includes('mobile') || s.includes('ios') || s.includes('android')) {
    return 'smartphone';
  }
  return 'folder_open';
};

const formatDate = (dateStr: string | null) => {
  if (!dateStr) return '';
  const date = new Date(dateStr);
  return date.toLocaleDateString(locale.value === 'vi' ? 'vi-VN' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  });
};

watch(
  () => route.params.slug,
  (newSlug) => {
    if (newSlug) {
      currentPage.value = 1;
      searchQuery.value = '';
      selectedTag.value = '';
      fetchTopicDetail();
      window.scrollTo({ top: 0, behavior: 'instant' });
    }
  }
);

onMounted(() => {
  window.scrollTo({ top: 0, behavior: 'instant' });
  fetchTopicDetail();
});
</script>

<style scoped lang="scss">
.portfolio-page {
  background: transparent;
  min-height: 100vh;
  overflow-x: hidden;
}

.portfolio-content {
  margin: 0 auto;
  max-width: 1140px;
  padding: 40px 32px 80px;
  box-sizing: border-box;
}

/* 1. Breadcrumbs */
.topic-detail-breadcrumb {
  display: flex;
  align-items: center;
  gap: 10px;
  font-family: var(--font-body);
  font-size: 0.85rem;
  color: var(--text-secondary);
  margin-bottom: 32px;

  .bc-link {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    color: var(--text-secondary);
    text-decoration: none;
    transition: color 0.2s ease;

    &:hover {
      color: var(--accent-primary);
    }
  }

  .bc-separator {
    color: var(--text-muted);
  }

  .bc-current {
    color: var(--accent-primary);
    font-weight: 600;
  }
}

/* 2. Hero Banner */
.topic-hero-banner {
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-xl);
  padding: 36px 32px;
  margin-bottom: 36px;
  position: relative;
  overflow: hidden;
  box-shadow: var(--shadow-card);

  .banner-top-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 16px;
  }

  .topic-eyebrow {
    display: inline-flex;
    align-items: center;
    gap: 8px;
    color: var(--accent-primary);
    font-family: var(--font-mono);
    font-size: 0.76rem;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: 0.08em;

    .eyebrow-icon {
      color: var(--accent-primary);
    }
  }

  .topic-slug-badge {
    font-family: var(--font-mono);
    font-size: 0.74rem;
    color: var(--text-muted);
    background: var(--bg-surface-high);
    border: 1px solid var(--border-subtle);
    padding: 3px 10px;
    border-radius: var(--radius-sm);
  }

  .topic-main-title {
    font-family: var(--font-headline);
    font-size: clamp(2rem, 4vw, 2.75rem);
    font-weight: 800;
    color: var(--text-primary);
    letter-spacing: -0.03em;
    line-height: 1.15;
    margin: 0 0 14px;
  }

  .topic-main-desc {
    font-family: var(--font-body);
    font-size: 1rem;
    line-height: 1.65;
    color: var(--text-secondary);
    max-width: 780px;
    margin: 0 0 24px;
  }

  .topic-stats-row {
    display: flex;
    align-items: center;
    gap: 16px;

    .stat-badge {
      font-family: var(--font-mono);
      display: inline-flex;
      align-items: center;
      gap: 8px;
      font-size: 0.82rem;
      color: var(--text-secondary);
      background: var(--bg-surface-high);
      border: 1px solid var(--border-subtle);
      padding: 6px 14px;
      border-radius: var(--radius-md);

      strong {
        color: var(--accent-primary);
      }
    }
  }
}

/* 3. Toolbar (Search & Sort) */
.topic-filter-toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 20px;
  margin-bottom: 20px;
  padding: 12px 18px;
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-card);
}

.toolbar-search-box {
  position: relative;
  flex: 1;
  max-width: 480px;
  display: flex;
  align-items: center;

  .search-ico {
    position: absolute;
    left: 14px;
    color: var(--text-muted);
    pointer-events: none;
  }

  .toolbar-input {
    width: 100%;
    padding: 9px 36px 9px 40px;
    background: var(--bg-surface-high);
    border: 1px solid var(--border-subtle);
    border-radius: var(--radius-md);
    color: var(--text-primary);
    font-size: 0.88rem;
    outline: none;
    transition: all 0.2s ease;

    &:focus {
      border-color: var(--accent-primary);
      box-shadow: 0 0 0 2px rgba(16, 185, 129, 0.15);
    }

    &::placeholder {
      color: var(--text-muted);
    }
  }

  .clear-search-btn {
    position: absolute;
    right: 10px;
    background: transparent;
    border: 0;
    color: var(--text-muted);
    cursor: pointer;
    padding: 4px;
    display: flex;
    align-items: center;
    justify-content: center;

    &:hover {
      color: var(--text-primary);
    }
  }
}

.toolbar-sort-box {
  display: flex;
  align-items: center;
  gap: 12px;

  .sort-title {
    font-family: var(--font-mono);
    font-size: 0.78rem;
    color: var(--text-muted);
    white-space: nowrap;
  }

  .sort-dropdown {
    min-width: 175px;

    :deep(.q-field__control) {
      background: var(--bg-surface-high);
      border-radius: var(--radius-md);
      font-size: 0.84rem;
      min-height: 36px;
    }
    :deep(.q-field__native) {
      color: var(--text-primary);
      font-weight: 600;
    }
  }
}

/* 4. Tag Filter Chips */
.tag-filter-section {
  margin-bottom: 28px;
}

.tag-filter-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.tag-chip-btn {
  font-family: var(--font-mono);
  background: var(--bg-surface-high);
  border: 1px solid var(--border-subtle);
  border-radius: var(--radius-pill);
  color: var(--text-secondary);
  font-size: 0.76rem;
  font-weight: 600;
  padding: 5px 12px;
  cursor: pointer;
  transition: all 0.2s ease;

  &:hover {
    color: var(--text-primary);
    border-color: rgba(16, 185, 129, 0.35);
  }

  &.active {
    background: var(--accent-primary);
    border-color: var(--accent-primary);
    color: var(--accent-on-primary);
    font-weight: 700;
  }
}

/* 5. Articles Grid */
.posts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 24px;
  margin-bottom: 40px;
}

.article-card {
  display: flex;
  flex-direction: column;
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-lg);
  overflow: hidden;
  box-shadow: var(--shadow-card);
  transition: all 0.25s ease;

  &:hover {
    border-color: rgba(16, 185, 129, 0.35);
    transform: translateY(-3px);
    box-shadow: var(--shadow-card-hover);

    .article-thumb-img {
      transform: scale(1.03);
    }

    .article-title .article-link {
      color: var(--accent-primary);
    }

    .read-icon {
      transform: translateX(4px);
      color: var(--accent-primary);
    }
  }
}

.article-thumb-wrapper {
  position: relative;
  width: 100%;
  height: 180px;
  overflow: hidden;
  background: var(--bg-surface-lowest);
  display: block;
}

.article-thumb-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.35s ease;
}

.article-thumb-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, var(--bg-surface-lowest), var(--bg-surface-high));
  gap: 8px;

  .thumb-icon {
    color: var(--accent-primary);
    opacity: 0.5;
  }

  .thumb-topic-name {
    font-family: var(--font-mono);
    font-size: 0.8rem;
    font-weight: 700;
    color: var(--text-muted);
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }
}

.article-card-body {
  padding: 20px;
  display: flex;
  flex-direction: column;
  flex: 1;
}

.article-meta-row {
  display: flex;
  align-items: center;
  gap: 8px;
  font-family: var(--font-mono);
  font-size: 0.72rem;
  color: var(--text-muted);
  margin-bottom: 10px;

  .meta-date,
  .meta-views {
    display: inline-flex;
    align-items: center;
    gap: 4px;
  }
}

.article-title {
  font-family: var(--font-headline);
  font-size: 1.15rem;
  font-weight: 700;
  line-height: 1.35;
  margin: 0 0 10px;
  letter-spacing: -0.015em;

  .article-link {
    color: var(--text-primary);
    text-decoration: none;
    transition: color 0.2s ease;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }
}

.article-excerpt {
  font-family: var(--font-body);
  font-size: 0.88rem;
  line-height: 1.6;
  color: var(--text-secondary);
  margin: 0 0 14px;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
  flex: 1;
}

.article-tags-wrap {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  margin-bottom: 16px;

  .tag-pill {
    font-family: var(--font-mono);
    font-size: 0.72rem;
    color: var(--accent-secondary-text);
    background: var(--accent-secondary-container);
    border: 1px solid var(--accent-secondary-border);
    padding: 2px 8px;
    border-radius: var(--radius-sm);
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover {
      border-color: var(--accent-primary);
      color: var(--accent-primary);
    }
  }
}

.article-card-footer {
  border-top: 1px solid var(--border-hairline);
  padding-top: 12px;
  margin-top: auto;
}

.read-more-link {
  font-family: var(--font-headline);
  display: inline-flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  color: var(--text-secondary);
  text-decoration: none;
  font-size: 0.84rem;
  font-weight: 700;
  transition: color 0.2s ease;

  .read-icon {
    color: var(--text-muted);
    transition: transform 0.2s ease, color 0.2s ease;
  }

  &:hover {
    color: var(--accent-primary);

    .read-icon {
      color: var(--accent-primary);
    }
  }
}

/* Pagination */
.pagination-wrapper {
  display: flex;
  justify-content: center;
  margin: 30px 0;

  .custom-pagination {
    :deep(.q-btn) {
      font-family: var(--font-mono);
      font-weight: 700;
      border-radius: var(--radius-sm);
      margin: 0 2px;
    }
  }
}

/* Empty Box */
.topic-empty-box {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 70px 20px;
  text-align: center;
  background: var(--bg-surface-low);
  border: 1px dashed var(--border-subtle);
  border-radius: var(--radius-lg);
  margin-bottom: 40px;

  .empty-ico {
    color: var(--text-muted);
    margin-bottom: 14px;
  }

  h3 {
    font-family: var(--font-headline);
    font-size: 1.2rem;
    font-weight: 700;
    color: var(--text-primary);
    margin: 0 0 8px;
  }

  p {
    font-family: var(--font-body);
    font-size: 0.88rem;
    color: var(--text-secondary);
    margin: 0 0 18px;
  }

  .reset-filters-btn {
    background: var(--accent-primary-container);
    color: var(--accent-primary);
    border: 1px solid rgba(16, 185, 129, 0.3);
    font-weight: 700;
  }
}

/* 6. Other Topics Section */
.other-topics-section {
  border-top: 1px solid var(--border-hairline);
  padding-top: 48px;
  margin-top: 40px;

  .other-header {
    display: flex;
    align-items: flex-end;
    justify-content: space-between;
    margin-bottom: 24px;

    .other-title {
      font-family: var(--font-headline);
      font-size: 1.45rem;
      font-weight: 800;
      color: var(--text-primary);
      margin: 0 0 6px;
    }

    .other-desc {
      font-family: var(--font-body);
      font-size: 0.9rem;
      color: var(--text-secondary);
      margin: 0;
    }

    .view-all-link {
      font-family: var(--font-headline);
      display: inline-flex;
      align-items: center;
      gap: 6px;
      color: var(--accent-primary);
      text-decoration: none;
      font-size: 0.85rem;
      font-weight: 700;
      white-space: nowrap;

      .link-arrow {
        transition: transform 0.2s ease;
      }

      &:hover .link-arrow {
        transform: translateX(3px);
      }
    }
  }
}

.other-topics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 16px;
}

.other-topic-card {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 14px 16px;
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-md);
  text-decoration: none;
  box-shadow: var(--shadow-card);
  transition: all 0.2s ease;

  .other-icon-wrap {
    width: 36px;
    height: 36px;
    border-radius: var(--radius-sm);
    background: var(--accent-primary-container);
    border: 1px solid rgba(16, 185, 129, 0.2);
    display: flex;
    align-items: center;
    justify-content: center;
    color: var(--accent-primary);
    flex-shrink: 0;
  }

  .other-info {
    display: flex;
    flex-direction: column;
    flex: 1;
    overflow: hidden;

    h4 {
      font-family: var(--font-headline);
      font-size: 0.92rem;
      font-weight: 700;
      color: var(--text-primary);
      margin: 0 0 2px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    span {
      font-family: var(--font-mono);
      font-size: 0.72rem;
      color: var(--text-muted);
    }
  }

  .other-arrow {
    color: var(--text-muted);
    transition: transform 0.2s ease, color 0.2s ease;
  }

  &:hover {
    border-color: rgba(16, 185, 129, 0.3);
    transform: translateX(3px);

    .other-info h4 {
      color: var(--accent-primary);
    }

    .other-arrow {
      color: var(--accent-primary);
      transform: translateX(2px);
    }
  }
}

/* Loading & error states */
.load-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 100px 20px;
  gap: 16px;
  color: var(--text-secondary);

  &.error-state {
    color: #f87171;
  }

  .back-topics-btn {
    display: inline-flex;
    align-items: center;
    padding: 10px 20px;
    background: var(--accent-primary-container);
    color: var(--accent-primary);
    border: 1px solid rgba(16, 185, 129, 0.3);
    border-radius: var(--radius-md);
    font-weight: 700;
    text-decoration: none;
    font-size: 0.88rem;
    margin-top: 10px;
  }
}

@media (max-width: 860px) {
  .portfolio-content {
    padding: 30px 20px 60px;
  }

  .topic-hero-banner {
    padding: 28px 20px;

    .topic-main-title {
      font-size: 1.95rem;
    }
  }

  .topic-filter-toolbar {
    flex-direction: column;
    align-items: stretch;
  }

  .toolbar-search-box {
    max-width: 100%;
  }

  .toolbar-sort-box {
    justify-content: space-between;
  }

  .other-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
}

@media (max-width: 500px) {
  .portfolio-content {
    padding: 20px 16px 40px;
  }

  .posts-grid {
    grid-template-columns: 1fr;
  }

  .other-topics-grid {
    grid-template-columns: 1fr;
  }
}
</style>
