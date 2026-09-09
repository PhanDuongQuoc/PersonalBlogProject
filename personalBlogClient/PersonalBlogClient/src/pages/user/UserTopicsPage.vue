<template>
  <q-page class="portfolio-page">
    <main class="portfolio-content">
      <!-- 1. Hero Header Section -->
      <header class="topics-header-hero">
        <div class="category-pill-badge">
          <span class="green-dot"></span>
          <span>{{ text.browseByCategory.toUpperCase() }}</span>
        </div>

        <h1 class="main-hero-title">{{ text.topicsCatalog }}</h1>
        <p class="main-hero-desc">{{ text.topicsCatalogDesc }}</p>

        <!-- 3 Stats Overview Cards -->
        <div class="stats-cards-grid">
          <!-- Card 1: Topics -->
          <div class="metric-card">
            <div class="metric-icon-box">
              <q-icon name="folder" size="20px" />
            </div>
            <div class="metric-info">
              <span class="metric-number">{{ topics.length }}</span>
              <span class="metric-label">{{ text.topicsExploredLabel }}</span>
            </div>
          </div>

          <!-- Card 2: Published Posts -->
          <div class="metric-card">
            <div class="metric-icon-box">
              <q-icon name="article" size="20px" />
            </div>
            <div class="metric-info">
              <span class="metric-number">{{ totalAllPosts }}</span>
              <span class="metric-label">{{ text.publishedPostsLabel }}</span>
            </div>
          </div>

          <!-- Card 3: Total Views -->
          <div class="metric-card">
            <div class="metric-icon-box">
              <q-icon name="visibility" size="20px" />
            </div>
            <div class="metric-info">
              <span class="metric-number">{{ totalAllViews.toLocaleString() }}</span>
              <span class="metric-label">{{ text.totalViewsLabel }}</span>
            </div>
          </div>
        </div>
      </header>

      <!-- 2. Integrated Search & Filter Bar -->
      <section class="unified-filter-bar">
        <!-- Search Input -->
        <div class="search-input-box">
          <q-icon name="search" size="18px" class="search-ico" />
          <input
            v-model="searchKeyword"
            type="text"
            class="filter-search-field"
            :placeholder="text.searchTopicsPlaceholder"
          />
          <button
            v-if="searchKeyword"
            type="button"
            class="search-clear-btn"
            @click="searchKeyword = ''"
          >
            <q-icon name="close" size="14px" />
          </button>
        </div>

        <!-- Quick Category Tabs -->
        <div class="category-quick-tabs">
          <button
            type="button"
            class="tab-btn"
            :class="{ active: selectedCategoryFilter === 'all' }"
            @click="selectedCategoryFilter = 'all'"
          >
            {{ text.allTab || 'Tất cả' }} ({{ topics.length }})
          </button>
          <button
            v-for="topic in topCategories"
            :key="topic.id"
            type="button"
            class="tab-btn"
            :class="{ active: selectedCategoryFilter === topic.slug }"
            @click="selectedCategoryFilter = topic.slug"
          >
            {{ topic.name }}
          </button>
        </div>

        <!-- Sort & View Controls -->
        <div class="controls-right-group">
          <div class="sort-select-wrap">
            <span class="sort-prefix-text">{{ text.sortBy }}:</span>
            <q-select
              v-model="sortByOption"
              dense
              borderless
              emit-value
              map-options
              :options="sortOptions"
              class="inline-sort-dropdown"
            />
          </div>

          <div class="view-mode-toggle">
            <button
              type="button"
              class="view-btn"
              :class="{ active: viewMode === 'grid' }"
              aria-label="Grid View"
              @click="viewMode = 'grid'"
            >
              <q-icon name="grid_view" size="17px" />
            </button>
            <button
              type="button"
              class="view-btn"
              :class="{ active: viewMode === 'list' }"
              aria-label="List View"
              @click="viewMode = 'list'"
            >
              <q-icon name="view_list" size="17px" />
            </button>
          </div>
        </div>
      </section>

      <!-- 3. Loading State -->
      <div v-if="loading" class="load-state">
        <q-spinner color="teal-4" size="44px" />
        <p>{{ text.loading }}</p>
      </div>

      <!-- Error State -->
      <div v-else-if="errorMessage" class="load-state error-state">
        <q-icon name="error_outline" size="44px" color="negative" />
        <p>{{ errorMessage }}</p>
        <q-btn unelevated no-caps color="teal-5" label="Thử lại" @click="fetchTopics" />
      </div>

      <!-- 4. Topics Cards Grid / List -->
      <section
        v-else-if="filteredTopics.length > 0"
        class="topics-cards-container"
        :class="{ 'list-view-layout': viewMode === 'list' }"
      >
        <article
          v-for="topic in filteredTopics"
          :key="topic.id"
          class="exact-topic-card"
        >
          <!-- Top Row: Icon, Title & Slug -->
          <div class="card-head-row">
            <div class="card-icon-square">
              <q-icon :name="getTopicIcon(topic.slug)" size="20px" />
            </div>
            <div class="card-head-title-block">
              <h3 class="topic-item-title">
                <router-link :to="`/topics/${topic.slug}`">
                  {{ topic.name }}
                </router-link>
              </h3>
              <span class="topic-item-slug">/topics/{{ topic.slug }}</span>
            </div>
          </div>

          <!-- Description -->
          <p class="topic-item-desc">
            {{ topic.description || getDefaultDescription(topic.name) }}
          </p>

          <!-- Badges Pills Row -->
          <div class="topic-pills-row">
            <!-- Posts Count -->
            <div class="pill-chip">
              <q-icon name="description" size="13px" />
              <span>{{ topic.postCount }} {{ text.topicArticlesCount }}</span>
            </div>

            <!-- Views Count -->
            <div class="pill-chip">
              <q-icon name="visibility" size="13px" />
              <span>{{ topic.totalViews }} {{ text.viewsCount }}</span>
            </div>

            <!-- Date or Coming Soon Status -->
            <div v-if="topic.postCount > 0 && topic.latestPublishedAt" class="pill-chip">
              <q-icon name="schedule" size="13px" />
              <span>{{ formatDate(topic.latestPublishedAt) }}</span>
            </div>
            <div v-else class="pill-chip coming-soon-chip">
              <span class="yellow-dot"></span>
              <span>{{ text.comingSoon }}</span>
            </div>
          </div>

          <!-- Recent Post Section -->
          <div class="card-recent-section">
            <div class="recent-header-tag">
              <q-icon name="trending_up" size="12px" />
              <span>{{ text.recent.toUpperCase() }}</span>
            </div>

            <div v-if="topic.recentPosts && topic.recentPosts.length > 0" class="recent-post-row">
              <span class="post-green-dot">•</span>
              <router-link :to="`/posts/${topic.recentPosts[0].slug}`" class="recent-post-link">
                {{ topic.recentPosts[0].title }}
              </router-link>
            </div>
            <div v-else class="recent-empty-text">
              {{ text.draftingFirstPost }}
            </div>
          </div>

          <!-- Bottom Action Row -->
          <div class="card-bottom-row">
            <router-link :to="`/topics/${topic.slug}`" class="explore-topic-link">
              <span>{{ text.exploreTopic }}</span>
              <q-icon name="arrow_forward" size="14px" class="arrow-ico" />
            </router-link>
            <span class="post-count-mono">{{ topic.postCount }} {{ text.topicArticlesCount.slice(0, 3) }}</span>
          </div>
        </article>
      </section>

      <!-- Empty State -->
      <div v-else class="empty-state-box">
        <q-icon name="search_off" size="48px" class="empty-ico" />
        <h3>{{ text.noTopicsFound }}</h3>
        <p>{{ text.clearFilters }}</p>
        <q-btn
          unelevated
          no-caps
          class="reset-filters-btn"
          @click="resetFilters"
        >
          <q-icon name="restart_alt" size="16px" class="q-mr-xs" />
          {{ text.clearFilters }}
        </q-btn>
      </div>

      <!-- 5. Bottom Newsletter Section (Tech Dispatch) -->
      <section class="newsletter-cta-banner">
        <div class="nl-badge">
          <span>TECH DISPATCH NEWSLETTER</span>
        </div>

        <h2 class="nl-title">{{ text.newsletterTopicTitle }}</h2>
        <p class="nl-desc">{{ text.newsletterTopicDesc }}</p>

        <form class="nl-form" @submit.prevent="handleSubscribe">
          <input
            v-model="newsletterEmail"
            type="email"
            required
            class="nl-input"
            :placeholder="text.newsletterInputPlaceholder"
          />
          <button type="submit" class="nl-submit-btn" :disabled="isSubscribing">
            {{ isSubscribing ? text.postingComment : text.subscribeNow }}
          </button>
        </form>

        <p class="nl-trust-note">{{ text.newsletterTrust }}</p>
      </section>
    </main>

    <!-- Footer -->
    <portfolio-footer :email="authorEmail" />
  </q-page>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import { useQuasar } from 'quasar';
import api from '@/boot/ApiGateway/axios';
import PortfolioFooter from '@/components/portfolio/PortfolioFooter.vue';
import { usePortfolioLocale } from '@/composables/usePortfolioLocale';
import type { PublicTopicSummaryResponse } from '@/types/public-topic';

const $q = useQuasar();
const { locale, text } = usePortfolioLocale();
const route = useRoute();

const topics = ref<PublicTopicSummaryResponse[]>([]);
const loading = ref(true);
const errorMessage = ref('');
const searchKeyword = ref('');
const selectedCategoryFilter = ref<string>('all');
const sortByOption = ref<'default' | 'posts' | 'views' | 'name'>('default');
const viewMode = ref<'grid' | 'list'>('grid');
const newsletterEmail = ref('');
const isSubscribing = ref(false);
const authorEmail = ref<string | null>(null);

const sortOptions = computed(() => [
  { label: locale.value === 'vi' ? 'Mặc định / Nổi bật' : 'Default / Featured', value: 'default' },
  { label: locale.value === 'vi' ? 'Nhiều bài viết nhất' : 'Most Articles', value: 'posts' },
  { label: locale.value === 'vi' ? 'Lượt xem nhiều nhất' : 'Most Viewed', value: 'views' },
  { label: locale.value === 'vi' ? 'Theo tên (A - Z)' : 'Alphabetical (A - Z)', value: 'name' }
]);

const fetchTopics = async () => {
  loading.value = true;
  errorMessage.value = '';
  try {
    const username = typeof route.query.username === 'string' ? route.query.username : undefined;
    const response = await api.get<PublicTopicSummaryResponse[]>('/public/topics', {
      params: { username }
    });
    topics.value = response.data || [];
  } catch {
    errorMessage.value = text.value.loadFailed;
  } finally {
    loading.value = false;
  }
};

const topCategories = computed(() => {
  return topics.value.slice(0, 4);
});

const totalAllPosts = computed(() => {
  return topics.value.reduce((sum, t) => sum + (t.postCount || 0), 0);
});

const totalAllViews = computed(() => {
  return topics.value.reduce((sum, t) => sum + (t.totalViews || 0), 0);
});

const filteredTopics = computed(() => {
  let list = [...topics.value];

  // 1. Filter by category tab
  if (selectedCategoryFilter.value !== 'all') {
    list = list.filter((t) => t.slug === selectedCategoryFilter.value);
  }

  // 2. Filter by search input
  if (searchKeyword.value.trim()) {
    const kw = searchKeyword.value.trim().toLowerCase();
    list = list.filter(
      (t) =>
        t.name.toLowerCase().includes(kw) ||
        t.slug.toLowerCase().includes(kw) ||
        (t.description && t.description.toLowerCase().includes(kw))
    );
  }

  // 3. Sorting
  if (sortByOption.value === 'posts') {
    list.sort((a, b) => b.postCount - a.postCount);
  } else if (sortByOption.value === 'views') {
    list.sort((a, b) => b.totalViews - a.totalViews);
  } else if (sortByOption.value === 'name') {
    list.sort((a, b) => a.name.localeCompare(b.name));
  }

  return list;
});

const resetFilters = () => {
  searchKeyword.value = '';
  selectedCategoryFilter.value = 'all';
  sortByOption.value = 'default';
};

const getTopicIcon = (slug: string): string => {
  const s = (slug || '').toLowerCase();
  if (s.includes('learning') || s.includes('note') || s.includes('ghi-chep')) {
    return 'folder_open';
  }
  if (s.includes('thought') || s.includes('personal') || s.includes('suy-nghi')) {
    return 'folder';
  }
  if (s.includes('web') || s.includes('frontend') || s.includes('vue') || s.includes('react')) {
    return 'code';
  }
  if (s.includes('lap-trinh') || s.includes('code') || s.includes('net') || s.includes('csharp')) {
    return 'terminal';
  }
  if (s.includes('devops') || s.includes('cloud') || s.includes('docker') || s.includes('k8s')) {
    return 'cloud';
  }
  if (s.includes('design') || s.includes('ui') || s.includes('ux') || s.includes('system')) {
    return 'layers';
  }
  return 'folder_open';
};

const getDefaultDescription = (name: string): string => {
  if (locale.value === 'vi') {
    return `Tổng hợp các bài viết, ghi chú và bài học chuyên sâu về ${name}.`;
  }
  return `Curated technical essays, notes, and lessons exploring ${name}.`;
};

const formatDate = (dateStr: string | null) => {
  if (!dateStr) return '';
  const date = new Date(dateStr);
  return date.toLocaleDateString(locale.value === 'vi' ? 'vi-VN' : 'en-US', {
    day: 'numeric',
    month: 'short',
    year: 'numeric'
  });
};

const handleSubscribe = () => {
  if (!newsletterEmail.value) return;
  isSubscribing.value = true;
  setTimeout(() => {
    isSubscribing.value = false;
    newsletterEmail.value = '';
    $q.notify({
      type: 'positive',
      message: text.value.subscribeSuccessMsg || 'Cảm ơn bạn đã đăng ký nhận bản tin!',
      position: 'top',
      timeout: 3000
    });
  }, 600);
};

onMounted(() => {
  window.scrollTo({ top: 0, behavior: 'instant' });
  fetchTopics();
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

/* ==========================================================================
   1. HERO HEADER SECTION
   ========================================================================== */
.topics-header-hero {
  margin-bottom: 32px;

  .category-pill-badge {
    display: inline-flex;
    align-items: center;
    gap: 8px;
    padding: 5px 14px;
    background: var(--accent-primary-container);
    border: 1px solid rgba(16, 185, 129, 0.25);
    border-radius: var(--radius-pill);
    color: var(--accent-primary);
    font-family: var(--font-mono);
    font-size: 0.72rem;
    font-weight: 700;
    letter-spacing: 0.08em;
    margin-bottom: 14px;

    .green-dot {
      width: 6px;
      height: 6px;
      background: var(--accent-primary);
      border-radius: 50%;
      box-shadow: 0 0 8px var(--accent-primary);
      animation: pulse-glow 2s infinite;
    }
  }

  .main-hero-title {
    font-family: var(--font-headline);
    font-size: clamp(2rem, 4vw, 2.6rem);
    font-weight: 800;
    line-height: 1.15;
    color: var(--text-primary);
    letter-spacing: -0.03em;
    margin: 0 0 10px;
  }

  .main-hero-desc {
    font-family: var(--font-body);
    font-size: 0.98rem;
    line-height: 1.6;
    color: var(--text-secondary);
    max-width: 780px;
    margin: 0 0 28px;
  }
}

/* 3 Stat Cards Grid */
.stats-cards-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 18px;
  max-width: 600px;
}

.metric-card {
  display: flex;
  align-items: center;
  gap: 14px;
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-lg);
  padding: 14px 18px;
  box-shadow: var(--shadow-card);
  transition: all 0.2s ease;

  &:hover {
    border-color: rgba(16, 185, 129, 0.3);
  }

  .metric-icon-box {
    width: 40px;
    height: 40px;
    background: var(--accent-primary-container);
    color: var(--accent-primary);
    border-radius: var(--radius-md);
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
  }

  .metric-info {
    display: flex;
    flex-direction: column;

    .metric-number {
      font-family: var(--font-mono);
      font-size: 1.25rem;
      font-weight: 800;
      color: var(--text-primary);
      line-height: 1.1;
    }

    .metric-label {
      font-family: var(--font-mono);
      font-size: 0.68rem;
      font-weight: 600;
      color: var(--text-muted);
      letter-spacing: 0.04em;
      text-transform: uppercase;
      margin-top: 2px;
    }
  }
}

/* ==========================================================================
   2. UNIFIED FILTER & SEARCH BAR
   ========================================================================== */
.unified-filter-bar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 14px;
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-lg);
  padding: 10px 14px;
  box-shadow: var(--shadow-card);
  margin-bottom: 30px;
}

.search-input-box {
  position: relative;
  flex: 1;
  max-width: 330px;
  display: flex;
  align-items: center;

  .search-ico {
    position: absolute;
    left: 12px;
    color: var(--text-muted);
    pointer-events: none;
  }

  .filter-search-field {
    width: 100%;
    padding: 8px 30px 8px 36px;
    background: var(--bg-surface-high);
    border: 1px solid var(--border-subtle);
    border-radius: var(--radius-md);
    font-size: 0.84rem;
    color: var(--text-primary);
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

  .search-clear-btn {
    position: absolute;
    right: 8px;
    background: none;
    border: none;
    color: var(--text-muted);
    cursor: pointer;
    padding: 2px;
    display: flex;
    align-items: center;
    justify-content: center;

    &:hover {
      color: var(--text-primary);
    }
  }
}

.category-quick-tabs {
  display: flex;
  align-items: center;
  gap: 6px;
  overflow-x: auto;
  scrollbar-width: none;
  &::-webkit-scrollbar {
    display: none;
  }

  .tab-btn {
    font-family: var(--font-headline);
    background: transparent;
    border: 1px solid transparent;
    border-radius: var(--radius-md);
    padding: 6px 12px;
    font-size: 0.82rem;
    font-weight: 600;
    color: var(--text-secondary);
    cursor: pointer;
    white-space: nowrap;
    transition: all 0.15s ease;

    &:hover {
      color: var(--text-primary);
      background: var(--bg-surface-high);
    }

    &.active {
      background: var(--accent-primary-container);
      color: var(--accent-primary);
      border-color: rgba(16, 185, 129, 0.3);
    }
  }
}

.controls-right-group {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-shrink: 0;
}

.sort-select-wrap {
  display: flex;
  align-items: center;
  gap: 6px;

  .sort-prefix-text {
    font-family: var(--font-mono);
    font-size: 0.76rem;
    color: var(--text-muted);
    white-space: nowrap;
  }

  .inline-sort-dropdown {
    font-size: 0.82rem;
    font-weight: 600;
    min-width: 145px;

    :deep(.q-field__native) {
      padding: 0;
      color: var(--text-primary);
      font-weight: 600;
    }
    :deep(.q-field__control) {
      padding: 0 4px;
      min-height: 32px;
    }
  }
}

.view-mode-toggle {
  display: flex;
  align-items: center;
  background: var(--bg-surface-high);
  border-radius: var(--radius-sm);
  padding: 2px;

  .view-btn {
    background: transparent;
    border: none;
    border-radius: var(--radius-sm);
    padding: 4px 6px;
    color: var(--text-muted);
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: all 0.15s ease;

    &:hover {
      color: var(--text-primary);
    }

    &.active {
      background: var(--bg-surface-low);
      color: var(--accent-primary);
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    }
  }
}

/* ==========================================================================
   3. TOPICS CARDS GRID
   ========================================================================== */
.topics-cards-container {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 22px;
  margin-bottom: 48px;

  &.list-view-layout {
    grid-template-columns: 1fr;
  }
}

.exact-topic-card {
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-top: 3px solid var(--accent-primary);
  border-radius: var(--radius-lg);
  padding: 22px 20px 18px;
  display: flex;
  flex-direction: column;
  transition: all 0.25s ease;
  box-shadow: var(--shadow-card);

  &:hover {
    transform: translateY(-3px);
    box-shadow: var(--shadow-card-hover);
    border-color: rgba(16, 185, 129, 0.35);
    border-top-color: var(--accent-primary);

    .topic-item-title a {
      color: var(--accent-primary);
    }

    .arrow-ico {
      transform: translateX(4px);
      color: var(--accent-primary);
    }
  }
}

.card-head-row {
  display: flex;
  align-items: center;
  gap: 14px;
  margin-bottom: 14px;
}

.card-icon-square {
  width: 42px;
  height: 42px;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  background: var(--accent-primary-container);
  color: var(--accent-primary);
  border: 1px solid rgba(16, 185, 129, 0.25);
}

.card-head-title-block {
  display: flex;
  flex-direction: column;
  gap: 2px;
  overflow: hidden;

  .topic-item-title {
    font-family: var(--font-headline);
    font-size: 1.15rem;
    font-weight: 700;
    line-height: 1.25;
    margin: 0;

    a {
      color: var(--text-primary);
      text-decoration: none;
      transition: color 0.2s ease;
    }
  }

  .topic-item-slug {
    font-family: var(--font-mono);
    font-size: 0.74rem;
    color: var(--text-muted);
  }
}

.topic-item-desc {
  font-family: var(--font-body);
  font-size: 0.88rem;
  line-height: 1.55;
  color: var(--text-secondary);
  margin: 0 0 16px;
  min-height: 42px;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Pills Row */
.topic-pills-row {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 6px;
  margin-bottom: 16px;

  .pill-chip {
    font-family: var(--font-mono);
    display: inline-flex;
    align-items: center;
    gap: 5px;
    background: var(--bg-surface-high);
    color: var(--text-secondary);
    font-size: 0.72rem;
    font-weight: 600;
    padding: 3px 9px;
    border-radius: var(--radius-sm);
    border: 1px solid var(--border-hairline);

    &.coming-soon-chip {
      background: rgba(245, 158, 11, 0.1);
      color: #f59e0b;
      border-color: rgba(245, 158, 11, 0.25);

      .yellow-dot {
        width: 5px;
        height: 5px;
        background: #f59e0b;
        border-radius: 50%;
      }
    }
  }
}

/* Recent Section */
.card-recent-section {
  border-top: 1px solid var(--border-hairline);
  padding-top: 12px;
  margin-bottom: 16px;

  .recent-header-tag {
    display: flex;
    align-items: center;
    gap: 5px;
    font-family: var(--font-mono);
    font-size: 0.68rem;
    font-weight: 700;
    color: var(--text-muted);
    letter-spacing: 0.05em;
    margin-bottom: 6px;
  }

  .recent-post-row {
    display: flex;
    align-items: center;
    gap: 6px;
    font-size: 0.84rem;

    .post-green-dot {
      color: var(--accent-primary);
      font-size: 1rem;
      line-height: 1;
    }

    .recent-post-link {
      font-family: var(--font-headline);
      color: var(--text-primary);
      text-decoration: none;
      font-weight: 600;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      transition: color 0.2s ease;

      &:hover {
        color: var(--accent-primary);
      }
    }
  }

  .recent-empty-text {
    font-family: var(--font-body);
    font-size: 0.8rem;
    font-style: italic;
    color: var(--text-muted);
  }
}

/* Bottom Action Row */
.card-bottom-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-top: auto;
  border-top: 1px solid var(--border-hairline);
  padding-top: 12px;

  .explore-topic-link {
    font-family: var(--font-headline);
    display: inline-flex;
    align-items: center;
    gap: 6px;
    color: var(--text-primary);
    text-decoration: none;
    font-size: 0.84rem;
    font-weight: 700;
    transition: color 0.2s ease;

    .arrow-ico {
      color: var(--text-muted);
      transition: all 0.2s ease;
    }

    &:hover {
      color: var(--accent-primary);
    }
  }

  .post-count-mono {
    font-family: var(--font-mono);
    font-size: 0.74rem;
    color: var(--text-muted);
  }
}

/* ==========================================================================
   4. BOTTOM NEWSLETTER CTA BANNER
   ========================================================================== */
.newsletter-cta-banner {
  background: var(--bg-surface-low);
  border: 1px solid var(--border-hairline);
  border-radius: var(--radius-xl);
  padding: 48px 32px;
  text-align: center;
  color: var(--text-primary);
  margin-top: 40px;
  box-shadow: var(--shadow-card);

  .nl-badge {
    display: inline-flex;
    align-items: center;
    padding: 5px 14px;
    background: var(--accent-primary-container);
    border: 1px solid rgba(16, 185, 129, 0.25);
    border-radius: var(--radius-pill);
    color: var(--accent-primary);
    font-family: var(--font-mono);
    font-size: 0.72rem;
    font-weight: 700;
    letter-spacing: 0.08em;
    margin-bottom: 16px;
  }

  .nl-title {
    font-family: var(--font-headline);
    font-size: 1.85rem;
    font-weight: 800;
    color: var(--text-primary);
    letter-spacing: -0.02em;
    margin: 0 0 10px;
    line-height: 1.3;
  }

  .nl-desc {
    font-family: var(--font-body);
    font-size: 0.95rem;
    line-height: 1.6;
    color: var(--text-secondary);
    max-width: 640px;
    margin: 0 auto 24px;
  }

  .nl-form {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 12px;
    max-width: 520px;
    margin: 0 auto 14px;
  }

  .nl-input {
    flex: 1;
    padding: 12px 18px;
    background: var(--bg-surface-high);
    border: 1px solid var(--border-subtle);
    border-radius: var(--radius-md);
    color: var(--text-primary);
    font-size: 0.9rem;
    outline: none;
    transition: all 0.2s ease;

    &:focus {
      border-color: var(--accent-primary);
      box-shadow: 0 0 0 2px rgba(16, 185, 129, 0.2);
    }

    &::placeholder {
      color: var(--text-muted);
    }
  }

  .nl-submit-btn {
    font-family: var(--font-headline);
    padding: 12px 22px;
    background: var(--accent-primary);
    color: var(--accent-on-primary);
    font-size: 0.9rem;
    font-weight: 700;
    border: none;
    border-radius: var(--radius-md);
    cursor: pointer;
    white-space: nowrap;
    transition: all 0.2s ease;

    &:hover {
      background: var(--accent-primary-hover);
    }

    &:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
  }

  .nl-trust-note {
    font-family: var(--font-mono);
    font-size: 0.74rem;
    color: var(--text-muted);
    margin: 0;
  }
}

/* ==========================================================================
   5. STATES (Empty & Loading)
   ========================================================================== */
.load-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 20px;
  gap: 16px;
  color: var(--text-secondary);

  &.error-state {
    color: #ef4444;
  }
}

.empty-state-box {
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
    margin-bottom: 12px;
  }

  h3 {
    font-family: var(--font-headline);
    font-size: 1.15rem;
    font-weight: 700;
    color: var(--text-primary);
    margin: 0 0 6px;
  }

  p {
    font-family: var(--font-body);
    font-size: 0.88rem;
    color: var(--text-secondary);
    margin: 0 0 16px;
  }

  .reset-filters-btn {
    background: var(--accent-primary-container);
    color: var(--accent-primary);
    border: 1px solid rgba(16, 185, 129, 0.3);
    font-weight: 700;
  }
}

/* ==========================================================================
   6. RESPONSIVE QUERIES
   ========================================================================== */
@media (max-width: 1024px) {
  .topics-cards-container {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 860px) {
  .portfolio-content {
    padding: 24px 18px 60px;
  }

  .stats-cards-grid {
    grid-template-columns: 1fr;
    max-width: 100%;
  }

  .unified-filter-bar {
    flex-direction: column;
    align-items: stretch;
    gap: 12px;
  }

  .search-input-box {
    max-width: 100%;
  }

  .controls-right-group {
    justify-content: space-between;
  }

  .topics-cards-container {
    grid-template-columns: 1fr;
  }

  .nl-form {
    flex-direction: column;
  }

  .nl-submit-btn {
    width: 100%;
  }
}
</style>
